using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class SqlServerInsertBuilderTests
    {
        [TestMethod]
        public void AddServerGuidTest()
        {
            var result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => new { x.FirstName })
                .AddServerGuidIdStatement()
                .Build();

            var expected = "INSERT INTO Users (FirstName, Id) " +
                "VALUES (@FirstName, NEWID())";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void GetInsertedIdTest()
        {
            var builder = new SqlServerInsertBuilder<Users>(Utils.GetMockUser());
            builder
                .InsertOnly(x => new { x.FirstName })
                .AddServerGuidIdStatement();
            var result = builder
                .GetInsertedId<long>()
                .Build();

            var expected = "CREATE TABLE #temp (Id BIGINT); " +
                "INSERT INTO Users (FirstName, Id) OUTPUT INSERTED.Id INTO #temp " +
                "VALUES (@FirstName, NEWID()); " +
                "SELECT Id FROM #temp;";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void GetInsertedIdCustomTypeTest()
        {
            var builder = new SqlServerInsertBuilder<Users>(Utils.GetMockUser());
            builder
                .InsertOnly(x => new { x.FirstName })
                .AddServerGuidIdStatement();
            var result = builder
                .GetInsertedId("SomeType")
                .Build();

            var expected = "CREATE TABLE #temp (Id SomeType); " +
                "INSERT INTO Users (FirstName, Id) OUTPUT INSERTED.Id INTO #temp " +
                "VALUES (@FirstName, NEWID()); " +
                "SELECT Id FROM #temp;";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void SQLServerInsertClearTest()
        {
            var builder = new SqlServerInsertBuilder<Users>(Utils.GetMockUser());
            builder.GetInsertedId<long>()
                .InsertOnly(x => new { x.FirstName })
                .AddServerGuidIdStatement();

            builder.Clear();
            var result = builder
                .GetInsertedId<Guid>()
                .InsertOnly(x => new { x.LastName })
                .Build();

            var expected = "CREATE TABLE #temp (Id UNIQUEIDENTIFIER); " +
                "INSERT INTO Users (LastName) OUTPUT INSERTED.Id INTO #temp " +
                "VALUES (@LastName); SELECT Id FROM #temp;";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }
    }
}
