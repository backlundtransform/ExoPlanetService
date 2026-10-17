using System.Text;

namespace ExoplanetHunter.Ingest.Nasa;

/// <summary>Minimal RFC 4180 reader: quoted fields, escaped quotes ("") and newlines inside quotes.</summary>
public static class Csv
{
    public static IEnumerable<IReadOnlyDictionary<string, string>> ReadRecords(TextReader reader)
    {
        string[]? header = null;
        foreach (var fields in ReadRows(reader))
        {
            if (header is null)
            {
                header = fields;
                continue;
            }

            if (fields.Length == 1 && fields[0].Length == 0)
                continue; // blank line

            if (fields.Length != header.Length)
                throw new InvalidDataException($"Row has {fields.Length} fields, header has {header.Length}.");

            var record = new Dictionary<string, string>(header.Length, StringComparer.Ordinal);
            for (int i = 0; i < header.Length; i++)
                record[header[i]] = fields[i];
            yield return record;
        }
    }

    public static IEnumerable<string[]> ReadRows(TextReader reader)
    {
        var fields = new List<string>();
        var field = new StringBuilder();
        bool inQuotes = false, any = false;

        int c;
        while ((c = reader.Read()) != -1)
        {
            any = true;
            char ch = (char)c;
            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (reader.Peek() == '"')
                    {
                        field.Append('"');
                        reader.Read();
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(ch);
                }
            }
            else if (ch == '"')
            {
                inQuotes = true;
            }
            else if (ch == ',')
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else if (ch is '\n' or '\r')
            {
                if (ch == '\r' && reader.Peek() == '\n')
                    reader.Read();
                fields.Add(field.ToString());
                field.Clear();
                yield return fields.ToArray();
                fields.Clear();
                any = false;
            }
            else
            {
                field.Append(ch);
            }
        }

        if (any)
        {
            fields.Add(field.ToString());
            yield return fields.ToArray();
        }
    }
}
