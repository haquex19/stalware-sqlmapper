using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Insertions;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class InsertBuilderTests
    {
        [TestMethod]
        public void AllPropertiesTest()
        {
            var result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertAll()
                .Build();

            var expected = "INSERT INTO Users (CreatedAt, ModifiedAt, FirstName, LastName, Email, Balance, Active) " +
                "VALUES (@CreatedAt, @ModifiedAt, @FirstName, @LastName, @Email, @Balance, @Active)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(DateTime.Today, result.Parameters["CreatedAt"]);
            Assert.AreEqual(DateTime.Today, result.Parameters["ModifiedAt"]);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
            Assert.AreEqual("ganon@gerudo.valley", result.Parameters["Email"]);
            Assert.AreEqual(9999, Convert.ToInt32(result.Parameters["Balance"]));
            Assert.AreEqual(true, result.Parameters["Active"]);
        }

        [TestMethod]
        public void AllExceptTest()
        {
            var result = new MySqlInsertBuilder<Users>(Utils.GetMockUser())
                .InsertAllExcept(x => new { x.LastName, x.Email })
                .Build();

            var expected = "INSERT INTO Users (CreatedAt, ModifiedAt, FirstName, Balance, Active) " +
                "VALUES (@CreatedAt, @ModifiedAt, @FirstName, @Balance, @Active)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(DateTime.Today, result.Parameters["CreatedAt"]);
            Assert.AreEqual(DateTime.Today, result.Parameters["ModifiedAt"]);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual(9999, Convert.ToInt32(result.Parameters["Balance"]));
            Assert.AreEqual(true, result.Parameters["Active"]);
        }

        [TestMethod]
        public void OnlyColumnsTest()
        {
            var result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => new { x.FirstName, x.LastName })
                .Build();

            var expected = "INSERT INTO Users (FirstName, LastName) " +
                "VALUES (@FirstName, @LastName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }

        [TestMethod]
        public void IncludeIdTest()
        {
            var result = new SqlServerInsertBuilder<SmallClass>(Utils.GetMockSmallClass())
                .InsertAll(true)
                .Build();

            var expected = "INSERT INTO SmallClass (Id, FirstName) " +
                "VALUES (@Id, @FirstName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(4, Convert.ToInt32(result.Parameters["Id"]));
            Assert.AreEqual("Daphnes", result.Parameters["FirstName"]);

            result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => new { x.Id, x.FirstName }, true)
                .Build();

            expected = "INSERT INTO Users (Id, FirstName) " +
                "VALUES (@Id, @FirstName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["Id"]));
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void ExpandoObjectInsertTest()
        {
            dynamic expando = new ExpandoObject();
            var dict = (IDictionary<string, object>)expando;

            dict.Add("FirstName", null);
            dict.Add("LastName", null);

            var result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertOnly(x => expando)
                .Build();

            var expected = "INSERT INTO Users (FirstName, LastName) " +
                "VALUES (@FirstName, @LastName)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);

            expando = new ExpandoObject();
            dict = (IDictionary<string, object>)expando;

            dict.Add("FirstName", null);
            dict.Add("LastName", null);
            dict.Add("Email", null);
            dict.Add("Balance", null);
            dict.Add("Active", null);

            result = new SqlServerInsertBuilder<Users>(Utils.GetMockUser())
                .InsertAllExcept(x => expando)
                .Build();

            expected = "INSERT INTO Users (CreatedAt, ModifiedAt) " +
                "VALUES (@CreatedAt, @ModifiedAt)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(DateTime.Today, result.Parameters["CreatedAt"]);
            Assert.AreEqual(DateTime.Today, result.Parameters["ModifiedAt"]);
        }
    }
}
