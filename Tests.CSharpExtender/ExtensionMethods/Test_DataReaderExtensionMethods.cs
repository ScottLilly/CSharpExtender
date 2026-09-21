using System.Data;
using CSharpExtender.ExtensionMethods;

namespace Tests.CSharpExtender.ExtensionMethods;

public class Test_DataReaderExtensionMethods
{
    [Fact]
    public void GetValue_ReturnsTheColumnValue()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal("Scott", reader.GetValue<string>("Name"));
        Assert.Equal(42, reader.GetValue<int>("Age"));
    }

    [Fact]
    public void GetValue_DBNull_ReturnsDefault()
    {
        using var reader = ReaderOverOneRow();

        Assert.Null(reader.GetValue<string>("NullableName"));
        Assert.Equal(0, reader.GetValue<int>("NullableAge"));
    }

    [Fact]
    public void GetValue_NullableTypeOverDBNull_ReturnsNull()
    {
        // Convert.ChangeType cannot target Nullable<T>, so this would throw if the
        // underlying type were not unwrapped first
        using var reader = ReaderOverOneRow();

        Assert.Null(reader.GetValue<int?>("NullableAge"));
        Assert.Null(reader.GetValue<DateTime?>("NullableAge"));
    }

    [Fact]
    public void GetValue_NullableTypeOverAValue_ReturnsTheValue()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal(42, reader.GetValue<int?>("Age"));
    }

    [Fact]
    public void GetValue_ConvertsBetweenTypes()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal("42", reader.GetValue<string>("Age"));
        Assert.Equal(42L, reader.GetValue<long>("Age"));
        Assert.Equal(42m, reader.GetValue<decimal>("Age"));
    }

    [Fact]
    public void GetValue_GuidStoredAsText_IsParsed()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal(Guid.Parse("11111111-2222-3333-4444-555555555555"),
            reader.GetValue<Guid>("GuidAsText"));
    }

    [Fact]
    public void GetValue_GuidStoredAsGuid_IsReturnedDirectly()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal(Guid.Parse("66666666-7777-8888-9999-000000000000"),
            reader.GetValue<Guid>("RealGuid"));
    }

    [Fact]
    public void GetValue_ByteArray_IsReturnedDirectly()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal(new byte[] { 1, 2, 3 }, reader.GetValue<byte[]>("Data"));
    }

    [Fact]
    public void GetValue_UnknownColumn_ThrowsIndexOutOfRangeNamingTheColumn()
    {
        // DataTableReader.GetOrdinal throws ArgumentException rather than the
        // IndexOutOfRangeException IDataRecord documents, so this also proves the
        // contract is the same whichever provider is behind the interface
        using var reader = ReaderOverOneRow();

        var exception = Assert.Throws<IndexOutOfRangeException>(
            () => reader.GetValue<string>("NoSuchColumn"));

        Assert.Contains("NoSuchColumn", exception.Message);
        Assert.Contains("Name", exception.Message);
    }

    [Fact]
    public void GetValue_UnconvertibleValue_ThrowsInvalidCastNamingTheColumn()
    {
        using var reader = ReaderOverOneRow();

        var exception = Assert.Throws<InvalidCastException>(
            () => reader.GetValue<int>("Name"));

        Assert.Contains("Name", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public void GetValue_ValueTooLargeForTheType_ThrowsInvalidCastNamingTheColumn()
    {
        using var reader = ReaderOverOneRow();

        var exception = Assert.Throws<InvalidCastException>(
            () => reader.GetValue<byte>("BigNumber"));

        Assert.Contains("BigNumber", exception.Message);
    }

    [Fact]
    public void GetValue_NullReader_ThrowsArgumentNullException()
    {
        IDataReader reader = null!;

        Assert.Throws<ArgumentNullException>(() => reader.GetValue<string>("Name"));
    }

    [Fact]
    public void GetValue_NullColumnName_ThrowsArgumentNullException()
    {
        using var reader = ReaderOverOneRow();

        Assert.Throws<ArgumentNullException>(() => reader.GetValue<string>(null!));
    }

    [Fact]
    public void TypedMethods_ReturnTheColumnValue()
    {
        using var reader = ReaderOverOneRow();

        Assert.Equal("Scott", reader.GetString("Name"));
        Assert.Equal(42, reader.GetInt32("Age"));
        Assert.True(reader.GetBoolean("IsActive"));
        Assert.Equal(19.99m, reader.GetDecimal("Price"));
        Assert.Equal(1.5, reader.GetDouble("Ratio"));
        Assert.Equal(2.5f, reader.GetFloat("Rate"));
        Assert.Equal(new DateTime(2026, 9, 15), reader.GetDateTime("Created"));
        Assert.Equal(Guid.Parse("66666666-7777-8888-9999-000000000000"),
            reader.GetGuid("RealGuid"));
        Assert.Equal(new byte[] { 1, 2, 3 }, reader.GetByteArray("Data"));
    }

    [Fact]
    public void TypedMethods_DBNull_ReturnDefault()
    {
        using var reader = ReaderOverOneRow();

        Assert.Null(reader.GetString("NullableName"));
        Assert.Equal(0, reader.GetInt32("NullableAge"));
        Assert.Null(reader.GetByteArray("NullableData"));
    }

    [Fact]
    public void TypedMethods_DoNotHideTheReadersOwnOrdinalOverloads()
    {
        // The names collide with IDataRecord's ordinal-based ones, and overload
        // resolution has to keep sending an int argument to the reader's own
        using var reader = ReaderOverOneRow();

        int ordinal = reader.GetOrdinal("Name");

        Assert.Equal("Scott", reader.GetString(ordinal));
        Assert.Equal("Scott", reader.GetString("Name"));
    }

    [Fact]
    public void GetValue_SecondRow_ReadsThatRowsValues()
    {
        // The reader is forward-only, so the methods must read whatever row it is on
        // rather than anything cached from the first
        using var reader = ReaderOverTwoRows();

        Assert.True(reader.Read());
        Assert.Equal("First", reader.GetString("Name"));

        Assert.True(reader.Read());
        Assert.Equal("Second", reader.GetString("Name"));
    }

    #region Readers for the tests

    private static IDataReader ReaderOverOneRow()
    {
        var table = new DataTable();

        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("Price", typeof(decimal));
        table.Columns.Add("Ratio", typeof(double));
        table.Columns.Add("Rate", typeof(float));
        table.Columns.Add("Created", typeof(DateTime));
        table.Columns.Add("RealGuid", typeof(Guid));
        table.Columns.Add("GuidAsText", typeof(string));
        table.Columns.Add("Data", typeof(byte[]));
        table.Columns.Add("BigNumber", typeof(int));
        table.Columns.Add("NullableName", typeof(string));
        table.Columns.Add("NullableAge", typeof(int));
        table.Columns.Add("NullableData", typeof(byte[]));

        table.Rows.Add(
            "Scott",
            42,
            true,
            19.99m,
            1.5,
            2.5f,
            new DateTime(2026, 9, 15),
            Guid.Parse("66666666-7777-8888-9999-000000000000"),
            "11111111-2222-3333-4444-555555555555",
            new byte[] { 1, 2, 3 },
            9999,
            DBNull.Value,
            DBNull.Value,
            DBNull.Value);

        var reader = table.CreateDataReader();

        reader.Read();

        return reader;
    }

    private static IDataReader ReaderOverTwoRows()
    {
        var table = new DataTable();

        table.Columns.Add("Name", typeof(string));

        table.Rows.Add("First");
        table.Rows.Add("Second");

        return table.CreateDataReader();
    }

    #endregion
}
