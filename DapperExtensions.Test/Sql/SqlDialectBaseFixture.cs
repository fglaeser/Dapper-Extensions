using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Sql
{
    [TestClass]
    public class SqlDialectBaseFixture
    {
        public abstract class SqlDialectBaseFixtureBase
        {
            protected TestDialect Dialect;

            [TestInitialize]
            public void Setup()
            {
                Dialect = new TestDialect();
            } 
        }

        [TestClass]
        public class Properties : SqlDialectBaseFixtureBase
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
        public class IsQuotedMethod : SqlDialectBaseFixtureBase
        {
            [TestMethod]
            public void WithQuotes_ReturnsTrue()
            {
                Assert.IsTrue(Dialect.IsQuoted("\"foo\""));
            }

            [TestMethod]
            public void WithOnlyStartQuotes_ReturnsFalse()
            {
                Assert.IsFalse(Dialect.IsQuoted("\"foo"));
            }

            [TestMethod]
            public void WithOnlyCloseQuotes_ReturnsFalse()
            {
                Assert.IsFalse(Dialect.IsQuoted("foo\""));
            }
        }
        
        [TestClass]
        public class QuoteStringMethod : SqlDialectBaseFixtureBase
        {
            [TestMethod]
            public void WithNoQuotes_AddsQuotes()
            {
                Assert.AreEqual("\"foo\"", Dialect.QuoteString("foo"));
            }

            [TestMethod]
            public void WithStartQuote_AddsQuotes()
            {
                Assert.AreEqual("\"\"foo\"", Dialect.QuoteString("\"foo"));
            }

            [TestMethod]
            public void WithCloseQuote_AddsQuotes()
            {
                Assert.AreEqual("\"foo\"\"", Dialect.QuoteString("foo\""));
            }

            [TestMethod]
            public void WithBothQuote_DoesNotAddQuotes()
            {
                Assert.AreEqual("\"foo\"", Dialect.QuoteString("\"foo\""));
            }
        }

        [TestClass]
        public class UnQuoteStringMethod : SqlDialectBaseFixtureBase
        {
            [TestMethod]
            public void WithNoQuotes_AddsQuotes()
            {
                Assert.AreEqual("foo", Dialect.UnQuoteString("foo"));
            }

            [TestMethod]
            public void WithStartQuote_AddsQuotes()
            {
                Assert.AreEqual("\"foo", Dialect.UnQuoteString("\"foo"));
            }

            [TestMethod]
            public void WithCloseQuote_AddsQuotes()
            {
                Assert.AreEqual("foo\"", Dialect.UnQuoteString("foo\""));
            }

            [TestMethod]
            public void WithBothQuote_DoesNotAddQuotes()
            {
                Assert.AreEqual("foo", Dialect.UnQuoteString("\"foo\""));
            }
        }

        [TestClass]
        public class GetTableNameMethod : SqlDialectBaseFixtureBase
        {
            [TestMethod]
            public void NullTableName_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetTableName(null, null, null));
                Assert.AreEqual("TableName", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void EmptyTableName_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetTableName(null, string.Empty, null));
                Assert.AreEqual("TableName", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void TableNameOnly_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetTableName(null, "foo", null);
                Assert.AreEqual("\"foo\"", result);
            }

            [TestMethod]
            public void SchemaAndTable_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetTableName("bar", "foo", null);
                Assert.AreEqual("\"bar\".\"foo\"", result);
            }

            [TestMethod]
            public void AllParams_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetTableName("bar", "foo", "al");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }

            [TestMethod]
            public void ContainsQuotes_DoesNotAddExtraQuotes()
            {
                string result = Dialect.GetTableName("\"bar\"", "\"foo\"", "\"al\"");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }
        }

        [TestClass]
        public class GetColumnNameMethod : SqlDialectBaseFixtureBase
        {
            [TestMethod]
            public void NullColumnName_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetColumnName(null, null, null));
                Assert.AreEqual("ColumnName", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void EmptyColumnName_ThrowsException()
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(() => Dialect.GetColumnName(null, string.Empty, null));
                Assert.AreEqual("ColumnName", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [TestMethod]
            public void ColumnNameOnly_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetColumnName(null, "foo", null);
                Assert.AreEqual("\"foo\"", result);
            }

            [TestMethod]
            public void PrefixColumnName_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetColumnName("bar", "foo", null);
                Assert.AreEqual("\"bar\".\"foo\"", result);
            }

            [TestMethod]
            public void AllParams_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetColumnName("bar", "foo", "al");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }

            [TestMethod]
            public void ContainsQuotes_DoesNotAddExtraQuotes()
            {
                string result = Dialect.GetColumnName("\"bar\"", "\"foo\"", "\"al\"");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }
        }

        public class TestDialect : SqlDialectBase
        {
            public override string GetIdentitySql(string tableName)
            {
                throw new NotImplementedException();
            }

            public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
            {
                throw new NotImplementedException();
            }

            public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
            {
                throw new NotImplementedException();
            }
        }
    }
}