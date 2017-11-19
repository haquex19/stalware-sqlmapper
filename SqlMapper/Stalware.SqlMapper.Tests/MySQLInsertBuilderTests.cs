using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class MySQLInsertBuilderTests
    {
        [TestMethod]
        public void AddServerGuidTest()
        {
            var result = new MySQLInsertBuilder<Users>(Utils.GetMockUser())
                .AddServerGuidIdStatement()
                .InsertOnly(x => new { x.LastName })
                .Build();

            var expected = "INSERT INTO Users (Id, LastName) " +
                "VALUES (UUID(), @LastName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("LastName", result.Parameters[0].Key);
            Assert.AreEqual("Dragmire", result.Parameters[0].Value);
        }

        [TestMethod]
        public void GetInsertedUUIDTest()
        {
            var builder = new MySQLInsertBuilder<Users>(Utils.GetMockUser());
            builder
                .InsertOnly(x => new { x.LastName })
                .AddServerGuidIdStatement();
            var result = builder
                .GetInsertedUUID()
                .Build();

            var expected = "SET @temp = SELECT UUID(); " +
                "INSERT INTO Users (LastName, Id) " +
                "VALUES (@LastName, @temp); " +
                "SELECT @temp;";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("LastName", result.Parameters[0].Key);
            Assert.AreEqual("Dragmire", result.Parameters[0].Value);
        }

        [TestMethod]
        public void GetLastAutoIncrementIdTest()
        {
            var result = new MySQLInsertBuilder<Users>(Utils.GetMockUser())
                .GetLastAutoIncrementId()
                .InsertOnly(x => new { x.FirstName })
                .Build();

            var expected = "INSERT INTO Users (FirstName) " +
                "VALUES (@FirstName); SELECT LAST_INSERT_ID();";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("FirstName", result.Parameters[0].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[0].Value);
        }
    }
}
