using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class UpdateBuilderTests
    {
        [TestMethod]
        public void UpdateAllTest()
        {
            var result = new UpdateBuilder<SmallClass>(Utils.GetMockSmallClass())
                .UpdateAll()
                .Build();

            var expected = "UPDATE SmallClass " +
                "SET FirstName = @FirstName " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Daphnes", result.Parameters["FirstName"]);
            Assert.AreEqual(4, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateAllWithWhereTest()
        {
            var result = new UpdateBuilder<SmallClass>(Utils.GetMockSmallClass())
                .UpdateAll()
                .Where(x => x.FirstName == "Test")
                .Build();

            var expected = "UPDATE SmallClass " +
                "SET FirstName = @FirstName " +
                "WHERE (FirstName = @PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Daphnes", result.Parameters["FirstName"]);
            Assert.AreEqual("Test", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        public void UpdateAllExceptTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateAllExcept(x => new { x.CreatedAt, x.ModifiedAt, x.Email, x.Balance })
                .Build();

            var expected = "UPDATE Users " +
                "SET FirstName = @FirstName, LastName = @LastName, Active = @Active " +
                "WHERE Id = @PARAM0";

            TestUpdateAllExcept(result, expected);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateAllExceptWithWhereTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateAllExcept(x => new { x.CreatedAt, x.ModifiedAt, x.Email, x.Balance })
                .Where(x => x.Active && x.FirstName == x.LastName || x.Id >= 15)
                .Build();

            var expected = "UPDATE Users " +
                "SET FirstName = @FirstName, LastName = @LastName, Active = @Active " +
                "WHERE ((Active = 1 AND (FirstName = LastName)) OR (Id >= @PARAM0))";

            TestUpdateAllExcept(result, expected);
            Assert.AreEqual(15, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateOnlyTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.Id, x.FirstName, x.LastName })
                .Build();

            var expected = "UPDATE Users " +
                "SET Id = @Id, FirstName = @FirstName, LastName = @LastName " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["Id"]));
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateOnlyWithWhereTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName })
                .Where(x => x.Active && x.Id == 1)
                .Build();

            var expected = "UPDATE Users " +
                "SET FirstName = @FirstName " +
                "WHERE (Active = 1 AND (Id = @PARAM0))";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual(1, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateWithWhereChainTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName })
                .Where(x => x.Active)
                .Where(x => x.Id == 1)
                .Build();

            var expected = "UPDATE Users " +
                "SET FirstName = @FirstName " +
                "WHERE Active = 1 AND (Id = @PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual(1, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void DoNotSkipIdColumnOnUpdateTest()
        {
            var result = new UpdateBuilder<SmallClass>(Utils.GetMockSmallClass())
                .UpdateAll(false)
                .Build();

            var expected = "UPDATE SmallClass " +
                "SET Id = @Id, FirstName = @FirstName " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(4, Convert.ToInt32(result.Parameters["Id"]));
            Assert.AreEqual("Daphnes", result.Parameters["FirstName"]);
            Assert.AreEqual(4, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void PreventAutoWhereIdTest()
        {
            var result = new UpdateBuilder<SmallClass>(Utils.GetMockSmallClass())
                .UpdateAll()
                .PreventWhereOnIdAutoAdd()
                .Build();

            var expected = "UPDATE SmallClass " +
                "SET FirstName = @FirstName";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Daphnes", result.Parameters["FirstName"]);
        }

        [TestMethod]
        public void UpdateClearTest()
        {
            var builder = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName })
                .Where(x => x.Active);

            builder.Clear();
            var result = builder
                .UpdateOnly(x => new { x.LastName })
                .Where(x => !x.Active)
                .Build();

            var expected = "UPDATE Users " +
                "SET LastName = @LastName " +
                "WHERE Active = 0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
        }

        [TestMethod]
        public void UpdateClearWithPreventWhereOnIdAutoAddTest()
        {
            var builder = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName })
                .PreventWhereOnIdAutoAdd();

            builder.Clear();
            var result = builder
                .UpdateOnly(x => new { x.LastName })
                .Build();

            var expected = "UPDATE Users " +
                "SET LastName = @LastName " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void ExpandoObjectUpdateTest()
        {
            dynamic expando = new ExpandoObject();
            var dict = (IDictionary<string, object>)expando;

            dict.Add("FirstName", null);
            dict.Add("LastName", null);

            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => expando)
                .Build();

            var expected = "UPDATE Users " +
                "SET FirstName = @FirstName, LastName = @LastName " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));

            expando = new ExpandoObject();
            dict = (IDictionary<string, object>)expando;

            dict.Add("FirstName", null);
            dict.Add("LastName", null);
            dict.Add("Email", null);
            dict.Add("Balance", null);
            dict.Add("Active", null);

            result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateAllExcept(x => expando)
                .Build();

            expected = "UPDATE Users " +
                "SET CreatedAt = @CreatedAt, ModifiedAt = @ModifiedAt " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(DateTime.Today, result.Parameters["CreatedAt"]);
            Assert.AreEqual(DateTime.Today, result.Parameters["ModifiedAt"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void UpdateWhereInTest()
        {
            var result = new UpdateBuilder<Users>(new Users { Active = true })
                .UpdateOnly(x => new { x.Active })
                .In(x => x.Id, new[] { 5, 8, 1, 2 })
                .Build();

            var expected = "UPDATE Users SET Active = @Active WHERE Id IN (@PARAM0, @PARAM1, @PARAM2, @PARAM3)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(true, result.Parameters["Active"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual(8, Convert.ToInt32(result.Parameters["PARAM1"]));
            Assert.AreEqual(1, Convert.ToInt32(result.Parameters["PARAM2"]));
            Assert.AreEqual(2, Convert.ToInt32(result.Parameters["PARAM3"]));
        }

        [TestMethod]
        public void CustomWhereClauseOnUpdateTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName})
                .WhereCustomSql($"WHERE x.{nameof(Users.FirstName)} != @Something", new Dictionary<string, object> { { "Something", "Test" } })
                .Build();

            var expected = "UPDATE Users SET FirstName = @FirstName WHERE x.FirstName != @Something";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Test", result.Parameters["Something"]);
        }

        [TestMethod]
        public void CustomWhereClauseResetsOnUpdateTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName})
                .Where(x => x.Id == 5)
                .WhereCustomSql("WHERE x.Id = '5'", new Dictionary<string, object>())
                .Build();

            var expected = "UPDATE Users SET FirstName = @FirstName WHERE x.Id = '5'";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CustomWhereClauseExceptionOnUpdateTest()
        {
            var result = new UpdateBuilder<Users>(Utils.GetMockUser())
                .UpdateOnly(x => new { x.FirstName})
                .WhereCustomSql("Id != 5", new Dictionary<string, object>())
                .Build();
        }

        private void TestUpdateAllExcept(SqlMapperResult result, string expected)
        {
            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Ganondorf", result.Parameters["FirstName"]);
            Assert.AreEqual("Dragmire", result.Parameters["LastName"]);
            Assert.AreEqual(true, result.Parameters["Active"]);
        }
    }
}
