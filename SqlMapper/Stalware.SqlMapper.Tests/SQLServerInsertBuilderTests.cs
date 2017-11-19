using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class SQLServerInsertBuilderTests
    {
        [TestMethod]
        public void AddServerGuidTest()
        {
            var result = new SQLServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => new { x.FirstName })
                .AddServerGuidIdStatement()
                .Build();

            var expected = "INSERT INTO Users (FirstName, Id) " +
                "VALUES (@FirstName, NEWID())";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("FirstName", result.Parameters[0].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[0].Value);
        }

        [TestMethod]
        public void GetInsertedIdTest()
        {
            var builder = new SQLServerInsertBuilder<Users>(Utils.GetMockUser());
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
            Assert.AreEqual("FirstName", result.Parameters[0].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[0].Value);
        }

        [TestMethod]
        public void GetInsertedIdCustomTypeTest()
        {
            var builder = new SQLServerInsertBuilder<Users>(Utils.GetMockUser());
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
            Assert.AreEqual("FirstName", result.Parameters[0].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[0].Value);
        }
    }
}
