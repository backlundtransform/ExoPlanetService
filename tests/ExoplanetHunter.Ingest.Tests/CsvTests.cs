using ExoplanetHunter.Ingest.Nasa;

namespace ExoplanetHunter.Ingest.Tests;

[TestClass]
public sealed class CsvTests
{
    [TestMethod]
    public void ReadRows_QuotedFieldWithComma_IsOneField()
    {
        // The legacy importer split on every comma, shifting columns for rows like this one.
        var rows = Csv.ReadRows(new StringReader("a,\"K0 III, peculiar\",c\n")).ToArray();

        CollectionAssert.AreEqual(new[] { "a", "K0 III, peculiar", "c" }, rows.Single());
    }

    [TestMethod]
    public void ReadRows_EscapedQuote_IsUnescaped()
    {
        var rows = Csv.ReadRows(new StringReader("\"say \"\"hi\"\"\",x")).ToArray();

        CollectionAssert.AreEqual(new[] { "say \"hi\"", "x" }, rows.Single());
    }

    [TestMethod]
    public void ReadRecords_CrLfAndEmptyFields_MapsByHeader()
    {
        var records = Csv.ReadRecords(new StringReader("pl_name,pl_rade\r\nb,1.2\r\nc,\r\n")).ToArray();

        Assert.HasCount(2, records);
        Assert.AreEqual("1.2", records[0]["pl_rade"]);
        Assert.AreEqual("", records[1]["pl_rade"]);
    }

    [TestMethod]
    public void ReadRecords_WrongFieldCount_Throws()
    {
        Assert.ThrowsExactly<InvalidDataException>(() => Csv.ReadRecords(new StringReader("a,b\n1,2,3\n")).ToArray());
    }
}
