using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Sql
{
    public class SqlCeDialectFixture
    {
        public abstract class SqlCeDialectFixtureBase
        {
            protected SqlCeDialect Dialect;

            [TestInitialize]
            public void Setup()
            {
                Dialect = new SqlCeDialect();
            }
        }

        [TestClass]
        public class Properties : SqlCeDialectFixtureBase
        {
            [TestMethod]
            public void CheckSettings()
            {
                Assert.AreEqual('[', Dialect.OpenQuote);
                Assert.AreEqual(']', Dialect.CloseQuote);
                Assert.AreEqual(";" + Environment.NewLine, Dialect.BatchSeperator);
                Assert.AreEqual('@', Dialect.ParameterPrefix);
                Assert.IsFalse(Dialect.SupportsMultipleStatements);
            }
        }

        [TestClass]
        public class GetTableNameMethod : SqlCeDialectFixtureBase
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
                Assert.AreEqual("[foo]", result);
            }

            [TestMethod]
            public void SchemaAndTable_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetTableName("bar", "foo", null);
                Assert.AreEqual("[bar_foo]", result);
            }

            [TestMethod]
            public void AllParams_ReturnsProperlyQuoted()
            {
                string result = Dialect.GetTableName("bar", "foo", "al");
                Assert.AreEqual("[bar_foo] AS [al]", result);
            }
        }
    }
}