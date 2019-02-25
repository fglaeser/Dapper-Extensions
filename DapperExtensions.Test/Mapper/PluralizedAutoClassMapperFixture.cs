using System;
using DapperExtensions.Mapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Mapper
{
    [TestClass]
    public class PluralizedAutoClassMapperFixture
    {
        [TestClass]
        public class PluralizedAutoClassMapperTableName
        {
            [TestMethod]
            public void ReturnsProperPluralization()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("robot");
                Assert.AreEqual("robots", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordEndsWithY()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("penny");
                Assert.AreEqual("pennies", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordEndsWithS()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("mess");
                Assert.AreEqual("messes", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordEndsWithF()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("life");
                Assert.AreEqual("lives", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordWithFe()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("leaf");
                Assert.AreEqual("leaves", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordContainsF()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("profile");
                Assert.AreEqual("profiles", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperPluralizationWhenWordContainsFe()
            {
                PluralizedAutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("effect");
                Assert.AreEqual("effects", m.TableName);
            }

            private PluralizedAutoClassMapper<T> GetMapper<T>() where T : class
            {
                return new PluralizedAutoClassMapper<T>();
            }
        }

        [TestClass]
        public class CustomPluralizedMapperTableName
        {
            [TestMethod]
            public void ReturnsProperPluralization()
            {
                CustomPluralizedMapper<Foo> m = GetMapper<Foo>();
                m.Table("Dog");
                Assert.AreEqual("Dogs", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperResultsForExceptions()
            {
                CustomPluralizedMapper<Foo> m = GetMapper<Foo>();
                m.Table("Person");
                Assert.AreEqual("People", m.TableName);
            }

            private CustomPluralizedMapper<T> GetMapper<T>() where T : class
            {
                return new CustomPluralizedMapper<T>();
            }

            public class CustomPluralizedMapper<T> : PluralizedAutoClassMapper<T> where T : class
            {
                public override void Table(string tableName)
                {
                    if (tableName.Equals("Person", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TableName = "People";
                    }
                    else
                    {
                        base.Table(tableName);
                    }
                }
            }
        }

        private class Foo
        {
        }
    }
}
