using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using DapperExtensions.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Sql
{
    [TestClass]
    public class DB2DialectFixture
    {
        public abstract class DB2DialectFixtureBase
        {
            protected DB2Dialect Dialect;

            [TestInitialize]
            public void Setup()
            {
                Dialect = new DB2Dialect();
            }
        }

        [TestClass]
        public class Properties : DB2DialectFixtureBase
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
        public class GetPagingSqlMethod : DB2DialectFixtureBase
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
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetPagingSql("SELECT \"SCHEMA\".\"COLUMN\" FROM \"SCHEMA\".\"TABLE\"", 0, 10, null));
                Assert.AreEqual("Parameters", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void Select_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT \"_TEMP\".\"COLUMN\" FROM (SELECT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS \"_ROW_NUMBER\", \"COLUMN\" FROM \"SCHEMA\".\"TABLE\") AS \"_TEMP\" WHERE \"_TEMP\".\"_ROW_NUMBER\" BETWEEN @_pageStartRow AND @_pageEndRow";
                var result = Dialect.GetPagingSql("SELECT \"COLUMN\" FROM \"SCHEMA\".\"TABLE\"", 1, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
                Assert.AreEqual(parameters["@_pageEndRow"], 10);
            }

            [TestMethod]
            public void SelectDistinct_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT \"_TEMP\".\"COLUMN\" FROM (SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS \"_ROW_NUMBER\", \"COLUMN\" FROM \"SCHEMA\".\"TABLE\") AS \"_TEMP\" WHERE \"_TEMP\".\"_ROW_NUMBER\" BETWEEN @_pageStartRow AND @_pageEndRow";
                var result = Dialect.GetPagingSql("SELECT DISTINCT \"COLUMN\" FROM \"SCHEMA\".\"TABLE\"", 1, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
                Assert.AreEqual(parameters["@_pageEndRow"], 10);
            }

            [TestMethod]
            public void SelectOrderBy_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT \"_TEMP\".\"COLUMN\" FROM (SELECT ROW_NUMBER() OVER(ORDER BY \"COLUMN\" DESC) AS \"_ROW_NUMBER\", \"COLUMN\" FROM \"SCHEMA\".\"TABLE\") AS \"_TEMP\" WHERE \"_TEMP\".\"_ROW_NUMBER\" BETWEEN @_pageStartRow AND @_pageEndRow";
                var result = Dialect.GetPagingSql("SELECT \"COLUMN\" FROM \"SCHEMA\".\"TABLE\" ORDER BY \"COLUMN\" DESC", 1, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
                Assert.AreEqual(parameters["@_pageEndRow"], 10);
            }
        }

        [TestClass]
        public class GetOrderByClauseMethod : DB2DialectFixtureBase
        {
            [TestMethod]
            public void NoOrderBy_Returns()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table");
                Assert.IsNull(result);
            }

            [TestMethod]
            public void OrderBy_ReturnsItemsAfterClause()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC");
                Assert.AreEqual("ORDER BY Column1 ASC, Column2 DESC", result);
            }

            [TestMethod]
            public void OrderByWithWhere_ReturnsOnlyOrderBy()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC WHERE Column1 = 'value'");
                Assert.AreEqual("ORDER BY Column1 ASC, Column2 DESC", result);
            }
        }
    }
}