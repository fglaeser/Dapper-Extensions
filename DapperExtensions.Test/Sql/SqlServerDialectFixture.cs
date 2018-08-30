using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using DapperExtensions.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Sql
{
    [TestClass]
    public class SqlServerDialectFixture
    {
        public abstract class SqlServerDialectFixtureBase
        {
            protected SqlServerDialect Dialect;

            [TestInitialize]
            public void Setup()
            {
                Dialect = new SqlServerDialect();
            }
        }

        [TestClass]
        public class Properties : SqlServerDialectFixtureBase
        {
            [TestMethod]
            public void CheckSettings()
            {
                Assert.AreEqual('[', Dialect.OpenQuote);
                Assert.AreEqual(']', Dialect.CloseQuote);
                Assert.AreEqual(";" + Environment.NewLine, Dialect.BatchSeperator);
                Assert.AreEqual('@', Dialect.ParameterPrefix);
                Assert.IsTrue(Dialect.SupportsMultipleStatements);
            }
        }

        [TestClass]
        public class GetPagingSqlMethod : SqlServerDialectFixtureBase
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
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table]", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(1, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
            }

            [TestMethod]
            public void SelectDistinct_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                var result = Dialect.GetPagingSql("SELECT DISTINCT [column] FROM [schema].[table]", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(1, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
            }

            [TestMethod]
            public void SelectOrderBy_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY [column] DESC) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table] ORDER BY [column] DESC", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(1, parameters.Count);
                Assert.AreEqual(parameters["@_pageStartRow"], 1);
            }
        }

        [TestClass]
        public class GetOrderByClauseMethod : SqlServerDialectFixtureBase
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