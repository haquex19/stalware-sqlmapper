using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class InsertBuilderTests
    {
        [TestMethod]
        public void AllPropertiesTest()
        {
            var result = new SQLServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertAll()
                .Build();

            var expected = "INSERT INTO Users (CreatedAt, ModifiedAt, FirstName, LastName, Email, Balance, Active) " +
                "VALUES (@CreatedAt, @ModifiedAt, @FirstName, @LastName, @Email, @Balance, @Active)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("CreatedAt", result.Parameters[0].Key);
            Assert.AreEqual(DateTime.Today, result.Parameters[0].Value);
            Assert.AreEqual("ModifiedAt", result.Parameters[1].Key);
            Assert.AreEqual(DateTime.Today, result.Parameters[1].Value);
            Assert.AreEqual("FirstName", result.Parameters[2].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[2].Value);
            Assert.AreEqual("LastName", result.Parameters[3].Key);
            Assert.AreEqual("Dragmire", result.Parameters[3].Value);
            Assert.AreEqual("Email", result.Parameters[4].Key);
            Assert.AreEqual("ganon@gerudo.valley", result.Parameters[4].Value);
            Assert.AreEqual("Balance", result.Parameters[5].Key);
            Assert.AreEqual(9999, Convert.ToInt32(result.Parameters[5].Value));
            Assert.AreEqual("Active", result.Parameters[6].Key);
            Assert.AreEqual(true, result.Parameters[6].Value);
        }

        [TestMethod]
        public void AllExceptTest()
        {
            var result = new MySQLInsertBuilder<Users>(Utils.GetMockUser())
                .InsertAllExcept(x => new { x.LastName, x.Email })
                .Build();

            var expected = "INSERT INTO Users (CreatedAt, ModifiedAt, FirstName, Balance, Active) " +
                "VALUES (@CreatedAt, @ModifiedAt, @FirstName, @Balance, @Active)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("CreatedAt", result.Parameters[0].Key);
            Assert.AreEqual(DateTime.Today, result.Parameters[0].Value);
            Assert.AreEqual("ModifiedAt", result.Parameters[1].Key);
            Assert.AreEqual(DateTime.Today, result.Parameters[1].Value);
            Assert.AreEqual("FirstName", result.Parameters[2].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[2].Value);
            Assert.AreEqual("Balance", result.Parameters[3].Key);
            Assert.AreEqual(9999, Convert.ToInt32(result.Parameters[3].Value));
            Assert.AreEqual("Active", result.Parameters[4].Key);
            Assert.AreEqual(true, result.Parameters[4].Value);
        }

        [TestMethod]
        public void OnlyColumnsTest()
        {
            var result = new SQLServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => new { x.FirstName, x.LastName })
                .Build();

            var expected = "INSERT INTO Users (FirstName, LastName) " +
                "VALUES (@FirstName, @LastName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("FirstName", result.Parameters[0].Key);
            Assert.AreEqual("Ganondorf", result.Parameters[0].Value);
            Assert.AreEqual("LastName", result.Parameters[1].Key);
            Assert.AreEqual("Dragmire", result.Parameters[1].Value);
        }
    }
}
