using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class MySqlInsertBuilderTests
    {
        [TestMethod]
        public void AddServerGuidTest()
        {
            var result = new MySqlInsertBuilder<Users>(Utils.GetMockUser())
                .AddServerGuidIdStatement()
                .InsertOnly(x => new { x.LastName })
                .Build();

            var expected = "INSERT INTO Users (Id, LastName) " +
                "VALUES (UUID(), @LastName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }

        [TestMethod]
        public void GetInsertedUUIDTest()
        {
            var builder = new MySqlInsertBuilder<Users>(Utils.GetMockUser());
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
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }

        [TestMethod]
        public void GetLastAutoIncrementIdTest()
        {
            var result = new MySqlInsertBuilder<Users>(Utils.GetMockUser())
                .GetLastAutoIncrementId()
                .InsertOnly(x => new { x.FirstName })
                .Build();

            var expected = "INSERT INTO Users (FirstName) " +
                "VALUES (@FirstName); SELECT LAST_INSERT_ID();";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void MySQLServerGuidClearExceptionTest()
        {
            var builder = new MySqlInsertBuilder<Users>(Utils.GetMockUser());
            var iBuilder = builder.AddServerGuidIdStatement();

            iBuilder.Clear();
            Assert.ThrowsException<InvalidOperationException>(() => builder.GetInsertedUUID());
        }

        [TestMethod]
        public void MySQLInsertClearTest()
        {
            var builder = new MySqlInsertBuilder<Users>(Utils.GetMockUser());
            builder.AddServerGuidIdStatement();
            builder
                .GetInsertedUUID()
                .InsertOnly(x => new { x.FirstName });

            builder.Clear();
            var result = builder
                .InsertOnly(x => new { x.LastName })
                .Build();

            var expected = "INSERT INTO Users (LastName) VALUES (@LastName)";
            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }
    }
}
