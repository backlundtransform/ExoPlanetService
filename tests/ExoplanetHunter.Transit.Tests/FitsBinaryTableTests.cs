using System.Buffers.Binary;
using System.Text;
using ExoplanetHunter.Transit.Fits;

namespace ExoplanetHunter.Transit.Tests;

[TestClass]
public sealed class FitsBinaryTableTests
{
    /// <summary>Builds a minimal FITS file: empty primary HDU plus a BINTABLE with the given rows.</summary>
    internal static byte[] BuildFits(double[] time, float[] flux, float[] error, int[] quality)
    {
        var bytes = new List<byte>();
        AddHeader(bytes, ["SIMPLE  =                    T", "BITPIX  =                    8", "NAXIS   =                    0"]);

        // Columns: TIME (D), a 2-byte string we skip (2A), PDCSAP_FLUX (E), PDCSAP_FLUX_ERR (E), QUALITY (J), 9 bit flags (9X)
        const int rowLength = 8 + 2 + 4 + 4 + 4 + 2;
        AddHeader(bytes,
        [
            "XTENSION= 'BINTABLE'",
            "BITPIX  =                    8",
            "NAXIS   =                    2",
            $"NAXIS1  = {rowLength,20}",
            $"NAXIS2  = {time.Length,20}",
            "PCOUNT  =                    0",
            "GCOUNT  =                    1",
            "TFIELDS =                    6",
            "TTYPE1  = 'TIME    '", "TFORM1  = 'D       '",
            "TTYPE2  = 'LABEL   '", "TFORM2  = '2A      '",
            "TTYPE3  = 'PDCSAP_FLUX'", "TFORM3  = 'E       '",
            "TTYPE4  = 'PDCSAP_FLUX_ERR'", "TFORM4  = 'E       '",
            "TTYPE5  = 'QUALITY '", "TFORM5  = 'J       '",
            "TTYPE6  = 'FLAGS   '", "TFORM6  = '9X      '",
            "OBJECT  = 'TIC 1   '           / target",
        ]);

        var row = new byte[rowLength];
        for (int i = 0; i < time.Length; i++)
        {
            BinaryPrimitives.WriteDoubleBigEndian(row.AsSpan(0), time[i]);
            row[8] = (byte)'x';
            row[9] = (byte)'y';
            BinaryPrimitives.WriteSingleBigEndian(row.AsSpan(10), flux[i]);
            BinaryPrimitives.WriteSingleBigEndian(row.AsSpan(14), error[i]);
            BinaryPrimitives.WriteInt32BigEndian(row.AsSpan(18), quality[i]);
            row[22] = 0xFF;
            row[23] = 0x80;
            bytes.AddRange(row);
        }

        Pad(bytes, 0);
        return bytes.ToArray();
    }

    private static void AddHeader(List<byte> bytes, string[] cards)
    {
        foreach (var card in cards.Append("END"))
            bytes.AddRange(Encoding.ASCII.GetBytes(card.PadRight(80)));
        Pad(bytes, (byte)' ');
    }

    private static void Pad(List<byte> bytes, byte fill)
    {
        while (bytes.Count % 2880 != 0)
            bytes.Add(fill);
    }

    [TestMethod]
    public void Read_ScalarColumns_ReturnsValues()
    {
        var fits = BuildFits([1.5, 2.5, 3.5], [100f, float.NaN, 102f], [1f, 1f, 2f], [0, 128, 0]);

        var table = FitsBinaryTable.Read(fits);

        Assert.AreEqual(3, table.RowCount);
        CollectionAssert.AreEqual(new[] { 1.5, 2.5, 3.5 }, table.Column("TIME"));
        Assert.AreEqual(100.0, table.Column("PDCSAP_FLUX")[0]);
        Assert.IsTrue(double.IsNaN(table.Column("PDCSAP_FLUX")[1]));
        CollectionAssert.AreEqual(new[] { 0.0, 128.0, 0.0 }, table.Column("QUALITY"));
        Assert.AreEqual("TIC 1", table.Header["OBJECT"]);
    }

    [TestMethod]
    public void Read_StringAndBitColumns_AreSkippedButCounted()
    {
        var table = FitsBinaryTable.Read(BuildFits([1], [10f], [1f], [0]));

        Assert.IsFalse(table.HasColumn("LABEL"));
        Assert.IsFalse(table.HasColumn("FLAGS"));
        Assert.AreEqual(10.0, table.Column("PDCSAP_FLUX")[0]);
    }

    [TestMethod]
    public void Read_NotABinaryTable_Throws()
    {
        var bytes = new byte[2880 * 2];
        Encoding.ASCII.GetBytes("SIMPLE  =                    T".PadRight(80) + "BITPIX  =                    8".PadRight(80)
            + "NAXIS   =                    0".PadRight(80) + "END".PadRight(80)).CopyTo(bytes, 0);
        Encoding.ASCII.GetBytes("XTENSION= 'IMAGE   '".PadRight(80) + "END".PadRight(80)).CopyTo(bytes, 2880);

        Assert.ThrowsExactly<InvalidDataException>(() => FitsBinaryTable.Read(bytes));
    }

    [TestMethod]
    public void SectorLightCurve_Normalized_DropsBadPointsAndDividesByMedian()
    {
        var fits = BuildFits([1, 2, 3, 4], [200f, 100f, float.NaN, 300f], [2f, 2f, 2f, 2f], [0, 0, 0, 8]);

        var (time, flux, error) = SectorLightCurve.FromFits(fits, 7).Normalized();

        CollectionAssert.AreEqual(new[] { 1.0, 2.0 }, time);          // NaN and flagged point removed
        CollectionAssert.AreEqual(new[] { 200 / 150.0, 100 / 150.0 }, flux);
        Assert.AreEqual(2 / 150.0, error[0], 1e-12);
    }
}
