using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using DapperExtensions.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Sql
{
    [TestClass]
    public class SqliteDialectFixture
    {
        public abstract class SqliteDialectFixtureBase
        {
            protected SqliteDialect Dialect;

            [TestInitialize]
            public void Setup()
            {
                Dialect = new SqliteDialect();
            }
        }

        [TestClass]
        public class Properties : SqliteDialectFixtureBase
        {
            [TestMethod]
            public void CheckSettings()
            {
                Assert.AreEqual('"', Dialect.OpenQuote);
                Assert.AreEqual('"', Dialect.CloseQuote);
                Assert.AreEqual(";" + Environment.NewLine, Dialect.BatchSeperator);
                Assert.AreEqual('@', Dialect.ParameterPrefix);
                Assert.IsTrue(Dialect.SupportsMultipleStatements);
            }
        }

        [TestClass]
        public class GetPagingSqlMethod : SqliteDialectFixtureBase
        {
            [TestMethod]
            public void NullSql_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetPagingSql(null, 0, 10, new Dictionary<string, object>()));
                Assert.AreEqual("SQL", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void EmptySql_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetPagingSql(string.Empty, 0, 10, new Dictionary<string, object>()));
                Assert.AreEqual("SQL", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void NullParameters_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetPagingSql("SELECT [schema].[column] FROM [schema].[table]", 0, 10, null));
                Assert.AreEqual("Parameters", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void Select_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT [column] FROM [schema].[table] LIMIT @Offset, @Count";
                var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table]", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@Offset"], 0);
                Assert.AreEqual(parameters["@Count"], 10);
            }
        }
    }
}