using System.Data;
using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods
{
    public class Test_DataSetExtensionMethods
    {
        [Fact]
        public void Test_GetValue()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            DataRow dataRow = dataTable.NewRow();
            dataRow["Id"] = 1;
            dataRow["Name"] = "Scott";

            Assert.Equal(1, dataRow.Get<int>("Id"));
            Assert.Equal("Scott", dataRow.Get<string>("Name"));
        }

        [Fact]
        public void Test_HasColumn()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            DataSet dataSet = new DataSet("Employees");
            dataSet.Tables.Add(dataTable);

            Assert.False(dataSet.HasColumn("PhoneNumber"));
            Assert.True(dataSet.HasColumn("Id"));
            Assert.True(dataSet.HasColumn("Name"));
            Assert.True(dataSet.HasColumn("name"));
        }
    }
}
