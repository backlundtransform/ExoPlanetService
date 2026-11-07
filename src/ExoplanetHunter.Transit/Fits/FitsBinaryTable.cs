using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace ExoplanetHunter.Transit.Fits;

/// <summary>
/// Minimal reader for the first binary-table extension (XTENSION = 'BINTABLE') of a FITS file,
/// enough for TESS and Kepler light-curve products. Supports scalar numeric columns
/// (TFORM B, I, J, K, E, D with repeat count 1); other columns are skipped.
/// </summary>
public sealed class FitsBinaryTable
{
    private const int BlockSize = 2880;
    private const int CardSize = 80;

    private readonly byte[] _data;
    private readonly int _dataOffset;
    private readonly int _rowLength;
    private readonly Dictionary<string, (int Offset, char Type)> _columns;

    private FitsBinaryTable(byte[] data, int dataOffset, int rowLength, int rowCount,
        Dictionary<string, (int, char)> columns, IReadOnlyDictionary<string, string> header)
    {
        _data = data;
        _dataOffset = dataOffset;
        _rowLength = rowLength;
        RowCount = rowCount;
        _columns = columns;
        Header = header;
    }

    public int RowCount { get; }

    /// <summary>Header keywords of the table extension, with string quotes removed.</summary>
    public IReadOnlyDictionary<string, string> Header { get; }

    public IEnumerable<string> ColumnNames => _columns.Keys;

    public static FitsBinaryTable Read(byte[] fits)
    {
        int offset = 0;

        // Primary HDU: header, then (for light curves, normally empty) data.
        var primary = ReadHeader(fits, ref offset);
        offset += Padded(DataSize(primary));

        var header = ReadHeader(fits, ref offset);
        if (header.GetValueOrDefault("XTENSION") != "BINTABLE")
            throw new InvalidDataException("The first extension is not a binary table.");

        int rowLength = Int(header, "NAXIS1");
        int rowCount = Int(header, "NAXIS2");
        int fields = Int(header, "TFIELDS");

        var columns = new Dictionary<string, (int, char)>(StringComparer.OrdinalIgnoreCase);
        int column = 0;
        for (int i = 1; i <= fields; i++)
        {
            string form = header.GetValueOrDefault($"TFORM{i}") ?? throw new InvalidDataException($"TFORM{i} missing.");
            var (repeat, type) = ParseForm(form);
            string? name = header.GetValueOrDefault($"TTYPE{i}");
            if (name is not null && repeat == 1 && "BIJKED".Contains(type))
                columns[name] = (column, type);
            column += Width(repeat, type);
        }

        if (column != rowLength)
            throw new InvalidDataException($"Column widths ({column}) do not add up to NAXIS1 ({rowLength}).");
        if (offset + (long)rowLength * rowCount > fits.Length)
            throw new InvalidDataException("The file is shorter than its table.");

        return new FitsBinaryTable(fits, offset, rowLength, rowCount, columns, header);
    }

    public bool HasColumn(string name) => _columns.ContainsKey(name);

    /// <summary>Column values as doubles; integer columns are converted, IEEE NaNs are kept.</summary>
    public double[] Column(string name)
    {
        if (!_columns.TryGetValue(name, out var column))
            throw new KeyNotFoundException($"Column '{name}' not found or not a scalar numeric column.");

        var values = new double[RowCount];
        for (int row = 0; row < RowCount; row++)
        {
            var span = _data.AsSpan(_dataOffset + row * _rowLength + column.Offset);
            values[row] = column.Type switch
            {
                'B' => span[0],
                'I' => BinaryPrimitives.ReadInt16BigEndian(span),
                'J' => BinaryPrimitives.ReadInt32BigEndian(span),
                'K' => BinaryPrimitives.ReadInt64BigEndian(span),
                'E' => BinaryPrimitives.ReadSingleBigEndian(span),
                'D' => BinaryPrimitives.ReadDoubleBigEndian(span),
                _ => throw new NotSupportedException(),
            };
        }

        return values;
    }

    private static Dictionary<string, string> ReadHeader(byte[] fits, ref int offset)
    {
        var header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (true)
        {
            if (offset + BlockSize > fits.Length)
                throw new InvalidDataException("Unexpected end of file in a FITS header.");

            bool end = false;
            for (int card = 0; card < BlockSize / CardSize && !end; card++)
            {
                string text = Encoding.ASCII.GetString(fits, offset + card * CardSize, CardSize);
                string key = text[..8].Trim();
                if (key == "END")
                    end = true;
                else if (text.Length > 10 && text[8] == '=' && key.Length > 0 && !header.ContainsKey(key))
                    header[key] = ParseValue(text[10..]);
            }

            offset += BlockSize;
            if (end)
                return header;
        }
    }

    private static string ParseValue(string field)
    {
        field = field.TrimStart();
        if (field.StartsWith('\''))
        {
            int close = field.IndexOf('\'', 1);
            return (close > 0 ? field[1..close] : field[1..]).TrimEnd();
        }

        int comment = field.IndexOf('/');
        return (comment >= 0 ? field[..comment] : field).Trim();
    }

    private static long DataSize(IReadOnlyDictionary<string, string> header)
    {
        int axes = Int(header, "NAXIS");
        if (axes == 0)
            return 0;

        long size = Math.Abs(Int(header, "BITPIX")) / 8;
        for (int i = 1; i <= axes; i++)
            size *= Int(header, $"NAXIS{i}");
        return size;
    }

    private static int Padded(long size) => (int)((size + BlockSize - 1) / BlockSize * BlockSize);

    private static int Int(IReadOnlyDictionary<string, string> header, string key) =>
        header.TryGetValue(key, out var value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n)
            ? n
            : throw new InvalidDataException($"Header keyword {key} missing or not an integer.");

    private static (int Repeat, char Type) ParseForm(string form)
    {
        int i = 0;
        while (i < form.Length && char.IsDigit(form[i]))
            i++;
        int repeat = i == 0 ? 1 : int.Parse(form[..i], CultureInfo.InvariantCulture);
        return (repeat, char.ToUpperInvariant(form[i]));
    }

    /// <summary>Bytes a column occupies in a row.</summary>
    private static int Width(int repeat, char type) => type switch
    {
        'X' => (repeat + 7) / 8, // bit array
        'L' or 'B' or 'A' => repeat,
        'I' => 2 * repeat,
        'J' or 'E' => 4 * repeat,
        'K' or 'D' or 'C' or 'P' => 8 * repeat,
        'M' or 'Q' => 16 * repeat,
        _ => throw new NotSupportedException($"TFORM type '{type}' is not supported."),
    };
}
