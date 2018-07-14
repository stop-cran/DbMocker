using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTests
    {
        [TestMethod]
        public void Mock_CheckBiDimenstionalObject_Test()
        {
            var x = new object[,] 
            {
                { "Col1", "Col2" },
                { "abc" , "def" },
                { "ghi" , "jkl" }
            };
            var y = new object[,]
            {
                { },
            };
            var z = new object[,] 
            {
            };

            Assert.AreEqual(2, x.Rank);
            Assert.AreEqual(6, x.Length);
            Assert.AreEqual(3, x.GetLength(0));
            Assert.AreEqual(2, x.GetLength(1));
            Assert.AreEqual("Col1", x[0, 0]);
            Assert.AreEqual("Col2", x[0, 1]);

            Assert.AreEqual(2, y.Rank);
            Assert.AreEqual(0, y.Length);
            Assert.AreEqual(1, y.GetLength(0));
            Assert.AreEqual(0, y.GetLength(1));

            Assert.AreEqual(2, z.Rank);
            Assert.AreEqual(0, z.Length);
            Assert.AreEqual(0, z.GetLength(0));
            Assert.AreEqual(0, z.GetLength(1));
        }

        [TestMethod]
        public void Mock_ReturnsMockTable_Properties_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsTable(new MockTable()
                {
                    Columns = new[] { "X" },
                    Rows = new object[,] 
                    {
                        { 14 }
                    }
                });

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsMockTable_Constructor_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsTable(new MockTable(columns: new[] { "X" },
                                       rows: new object[,]
                                            {
                                                { 14 }
                                            }));
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarValue_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarValueFromFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => 14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarValueFromParameter_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => 2 * (c.Parameters.First(p => p.ParameterName == "MyParam").Value as int?));

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            cmd.Parameters.Add(new MockDbParameter() { ParameterName = "MyParam", Value = 7 });
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_SecondMockFound_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("NO"))
                .ReturnsScalar(99);

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => c.CommandText.Length);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ...";     // This string contains 10 chars
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(10, result);
        }

    }
}
