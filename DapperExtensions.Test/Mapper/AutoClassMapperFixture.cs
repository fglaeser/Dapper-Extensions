using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Mapper
{
    [TestClass]
    public class AutoClassMapperFixture
    {
        [TestClass]
        public class AutoClassMapperTableName
        {
            [TestMethod]
            public void Constructor_ReturnsProperName()
            {
                AutoClassMapper<Foo> m = GetMapper<Foo>();
                Assert.AreEqual("Foo", m.TableName);
            }

            [TestMethod]
            public void SettingTableName_ReturnsProperName()
            {
                AutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("Barz");
                Assert.AreEqual("Barz", m.TableName);
            }

            [TestMethod]
            public void Sets_IdPropertyToKeyWhenFirstProperty()
            {
                AutoClassMapper<IdIsFirst> m = GetMapper<IdIsFirst>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "Id");
            }

            [TestMethod]
            public void Sets_IdPropertyToKeyWhenFoundInClass()
            {
                AutoClassMapper<IdIsSecond> m = GetMapper<IdIsSecond>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "Id");
            }

            [TestMethod]
            public void Sets_IdFirstPropertyEndingInIdWhenNoIdPropertyFound()
            {
                AutoClassMapper<IdDoesNotExist> m = GetMapper<IdDoesNotExist>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "SomeId");
            }
            
            private AutoClassMapper<T> GetMapper<T>() where T : class
            {
                return new AutoClassMapper<T>();
            }
        }

        [TestClass]
        public class CustomAutoMapperTableName
        {
            [TestMethod]
            public void ReturnsProperPluralization()
            {
                CustomAutoMapper<Foo> m = GetMapper<Foo>();
                Assert.AreEqual("Foo", m.TableName);
            }

            [TestMethod]
            public void ReturnsProperResultsForExceptions()
            {
                CustomAutoMapper<Foo2> m = GetMapper<Foo2>();
                Assert.AreEqual("TheFoo", m.TableName);
            }

            private CustomAutoMapper<T> GetMapper<T>() where T : class
            {
                return new CustomAutoMapper<T>();
            }

            public class CustomAutoMapper<T> : AutoClassMapper<T> where T : class
            {
                public override void Table(string tableName)
                {
                    if (tableName.Equals("Foo2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TableName = "TheFoo";
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
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class Foo2
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }


        private class IdIsFirst
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class IdIsSecond
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }

        private class IdDoesNotExist
        {
            public Guid SomeId { get; set; }
            public Guid ParentId { get; set; }
        }
    }
}
