using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperExtensions.Test.Helpers
{
  public class DatabaseConnection
  {
    protected IDbConnection Connection;
    protected IDapperImplementor Impl;

    [TestInitialize]
    public virtual void Setup()
    {
      string databaseName = string.Format("db_{0}.s3db", Guid.NewGuid().ToString());
      TestHelpers.LoadDatabase(databaseName);
      Connection = TestHelpers.GetConnection(databaseName);
      Impl = new DapperImplementor(TestHelpers.GetGenerator());
    }

    [TestCleanup]
    public virtual void Teardown()
    {
      string db = Connection.Database;
      Connection.Close();
      Connection.Dispose();
      TestHelpers.DeleteDatabase(db);
    }

  }
}