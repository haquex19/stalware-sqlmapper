using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class SelectBuilderTests
    {
        [TestMethod]
        public void RegularSelectTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id, x.FirstName })
                .Build();

            var expected = "SELECT x.Id, x.FirstName " +
                "FROM Users AS x";

            Assert.AreEqual(expected, result.Query);

            result = new SelectBuilder<Users>()
                .Select(x => new { x.CreatedAt, x.ModifiedAt, x.Id, x.LastName })
                .Build();

            expected = "SELECT x.CreatedAt, x.ModifiedAt, x.Id, x.LastName " +
                "FROM Users AS x";

            Assert.AreEqual(expected, result.Query);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Build();

            expected = "SELECT x.* FROM Users AS x";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SelectExceptionTest()
        {
            var query = new SelectBuilder<Users>()
                .Select(x => new { })
                .Select(x => new { })
                .Build();
        }

        [TestMethod]
        public void SelectWithWhereTest()
        {
            var s = new Users
            {
                FirstName = "Fosh"
            };
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.Id == 5 && x.FirstName == "Mahbubul")
                .Build();

            var expected = "SELECT x.Id " +
                "FROM Users AS x " +
                "WHERE ((x.Id = @PARAM0) AND (x.FirstName = @PARAM1))";            

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual("Mahbubul", result.Parameters["PARAM1"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { x.Id, x.FirstName })
                .Where(x => x.Email == "mhaquex19@gmail.com" || x.FirstName == s.FirstName)
                .Build();

            expected = "SELECT x.Id, x.FirstName " +
                "FROM Users AS x " +
                "WHERE ((x.Email = @PARAM0) OR (x.FirstName = @PARAM1))";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("mhaquex19@gmail.com", result.Parameters["PARAM0"]);
            Assert.AreEqual("Fosh", result.Parameters["PARAM1"]);
        }

        [TestMethod]
        public void SelectWithWhereWithBooleanTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => !x.Active && x.FirstName == "Fosh")
                .Build();

            var expected = "SELECT x.Id " +
                "FROM Users AS x " +
                "WHERE (x.Active = 0 AND (x.FirstName = @PARAM0))";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.Active)
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE x.Active = 1";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void SelectWithWhereWithBooleanOnRightHandSideTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.FirstName == "Fosh" && x.Active)
                .Build();

            var expected = "SELECT x.Id FROM Users AS x " +
                "WHERE ((x.FirstName = @PARAM0) AND x.Active = 1)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.FirstName == "Fosh" && !x.Active)
                .Build();

            expected = "SELECT x.Id FROM Users AS x " +
                "WHERE ((x.FirstName = @PARAM0) AND x.Active = 0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        public void SelectWithWhereAndOrderByTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.FirstName == "Mahbubul")
                .OrderBy(x => x.Id)
                .Build();

            var expected = "SELECT x.Id " +
                "FROM Users AS x " +
                "WHERE (x.FirstName = @PARAM0) " +
                "ORDER BY x.Id";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Mahbubul", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { x.Id, x.FirstName })
                .Where(x => x.FirstName.ToUpper() == "MAHBUBUL" && (x.Id == 5 || x.LastName == "Fosh"))
                .OrderBy(x => x.Id)
                .OrderByMultiple(x => new { x.FirstName, x.LastName })
                .OrderByDesc(x => x.CreatedAt)
                .Build();

            expected = "SELECT x.Id, x.FirstName " +
                "FROM Users AS x " +
                "WHERE ((UPPER(x.FirstName) = @PARAM0) AND ((x.Id = @PARAM1) OR (x.LastName = @PARAM2))) " +
                "ORDER BY x.Id, x.FirstName, x.LastName, x.CreatedAt DESC";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("MAHBUBUL", result.Parameters["PARAM0"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM1"]));
            Assert.AreEqual("Fosh", result.Parameters["PARAM2"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .OrderByMultipleDesc(x => new { x.FirstName, x.LastName })
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "ORDER BY x.FirstName DESC, x.LastName DESC";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void OrderByExceptionTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .OrderBy(x => x.Id > 5)
                .Build();

        }

        [TestMethod]
        public void MemberExpressionTests()
        {
            var f = new Users
            {
                FirstName = "eee"
            };

            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.FirstName == f.FirstName.ToUpper())
                .Build();

            var expected = "SELECT x.Id " +
                "FROM Users AS x " +
                "WHERE (x.FirstName = @PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("EEE", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.FirstName.Contains("f"))
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE x.FirstName LIKE @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("%f%", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.FirstName == "Mahbubul" && x.LastName.Contains(f.FirstName))
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE ((x.FirstName = @PARAM0) AND x.LastName LIKE @PARAM1)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Mahbubul", result.Parameters["PARAM0"]);
            Assert.AreEqual("%eee%", result.Parameters["PARAM1"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.FirstName.StartsWith(f.FirstName) && x.LastName.Trim() == "Fosh")
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE (x.FirstName LIKE @PARAM0 AND (LTRIM(RTRIM(x.LastName)) = @PARAM1))";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("eee%", result.Parameters["PARAM0"]);
            Assert.AreEqual("Fosh", result.Parameters["PARAM1"]);
        }
        
        [TestMethod]
        public void WhereOrTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.FirstName })
                .Where(x => x.Id > 5)
                .WhereOr(x => x.FirstName == "Fosh")
                .Build();

            var expected = "SELECT x.FirstName " +
                "FROM Users AS x " +
                "WHERE (x.Id > @PARAM0) OR (x.FirstName = @PARAM1)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual("Fosh", result.Parameters["PARAM1"]);
        }

        [TestMethod]
        public void WhereChainTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.FirstName == "Fosh")
                .WhereOr(x => x.LastName == "Test")
                .Where(x => x.Id <= 5)
                .WhereOr(x => x.Balance >= 5)
                .OrderBy(x => x.Id)
                .Build();

            var expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE (x.FirstName = @PARAM0) " +
                "OR (x.LastName = @PARAM1) " +
                "AND (x.Id <= @PARAM2) " +
                "OR (x.Balance >= @PARAM3) " +
                "ORDER BY x.Id";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);
            Assert.AreEqual("Test", result.Parameters["PARAM1"]);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM2"]));
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM3"]));
        }

        [TestMethod]
        public void JoinTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(user => new { user.FirstName })
                .Join<BankAccounts>((user, bank) => user.Id == bank.UserId, bank => new { bank.Id, bank.Balance })
                .Where(user => user.LastName == "Fosh")
                .Build();

            var expected = "SELECT user.FirstName, " +
                "bank.Id, bank.Balance " +
                "FROM Users AS user " +
                "JOIN BankAccounts AS bank ON (user.Id = bank.UserId) " +
                "WHERE (user.LastName = @PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);

            result = new SelectBuilder<Users>()
                .Select(user => new { user.Id })
                .LeftJoin<BankAccounts>((user, bank) => user.FirstName == bank.Name, bank => new { })
                .Where(bank => bank.Balance > 5)
                .OrderBy(user => user.Id)
                .Build();

            expected = "SELECT user.Id, bank.* " +
                "FROM Users AS user " +
                "LEFT JOIN BankAccounts AS bank ON (user.FirstName = bank.Name) " +
                "WHERE (bank.Balance > @PARAM0) " +
                "ORDER BY user.Id";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));

            result = new SelectBuilder<Users>()
                .Select(user => new { })
                .RightJoin<BankAccounts>((user, foshBank) => user.Id == foshBank.UserId && user.FirstName == foshBank.Name, foshBank => new { })
                .OrderBy(user => user.FirstName)
                .OrderByDesc(user => user.CreatedAt)
                .Build();

            expected = "SELECT user.*, foshBank.* " +
                "FROM Users AS user " +
                "RIGHT JOIN BankAccounts AS foshBank ON ((user.Id = foshBank.UserId) AND (user.FirstName = foshBank.Name)) " +
                "ORDER BY user.FirstName, user.CreatedAt DESC";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void ChainJoinTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(user => new { user.Id })
                .Join<BankAccounts>((user, bank) => user.Id == bank.UserId, bank => new { })
                .Join<CreditCards>((user, card) => user.Id == card.UserId, card => new { })
                .Build();

            var expected = "SELECT user.Id, bank.*, card.* " +
                "FROM Users AS user " +
                "JOIN BankAccounts AS bank ON (user.Id = bank.UserId) " +
                "JOIN CreditCards AS card ON (user.Id = card.UserId)";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void RightHandSideColumnExpressionTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.FirstName })
                .Where(x => x.Active && x.FirstName.Contains(x.LastName))
                .Build();

            var expected = "SELECT x.FirstName " +
                "FROM Users AS x " +
                "WHERE (x.Active = 1 AND x.FirstName LIKE '%' + x.LastName + '%')";

            Assert.AreEqual(expected, result.Query);

            var c = new CreditCards
            {
                BankAccountId = 6
            };

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => !x.Active && x.FirstName.StartsWith(c.BankAccountId.ToString()))
                .OrderBy(x => x.LastName)
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE (x.Active = 0 AND x.FirstName LIKE @PARAM0) " +
                "ORDER BY x.LastName";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("6%", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        public void MultiJoinTableTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Join<BankAccounts>((x, bank) => x.Id == bank.UserId, (bank) => new { bank.Id, bank.Balance })
                .Join<BankAccounts, CreditCards>((bank, card) => bank.Id == card.BankAccountId, (card) => new { })
                .Where(x => x.Id > 5)
                .Build();

            var expected = "SELECT x.*, bank.Id, bank.Balance, card.* " +
                "FROM Users AS x " +
                "JOIN BankAccounts AS bank ON (x.Id = bank.UserId) " +
                "JOIN CreditCards AS card ON (bank.Id = card.BankAccountId) " +
                "WHERE (x.Id > @PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));

            result = new SelectBuilder<BankAccounts>()
                .Select(bank => new { })
                .LeftJoin<Users>((bank, user) => bank.Id == user.Id, (user) => new { })
                .Join<Users>((bank, user2) => bank.UserId == user2.Balance, (user2) => new { })
                .Join<BankAccounts, CreditCards>((bank, card) => bank.Id >= card.BankAccountId, (card) => new { })
                .OrderBy(bank => bank.Id)
                .Build();

            expected = "SELECT bank.*, user.*, user2.*, card.* " +
                "FROM BankAccounts AS bank " +
                "LEFT JOIN Users AS user ON (bank.Id = user.Id) " +
                "JOIN Users AS user2 ON (bank.UserId = user2.Balance) " +
                "JOIN CreditCards AS card ON (bank.Id >= card.BankAccountId) " +
                "ORDER BY bank.Id";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void OtherThanOriginalWhereTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Join<BankAccounts>((x, bank) => x.Id == bank.UserId, (bank) => new { })
                .Where(x => x.Id > 5)
                .Where<BankAccounts>(bank => bank.Name == "Fosh")
                .Build();

            var expected = "SELECT x.*, bank.* " +
                "FROM Users AS x " +
                "JOIN BankAccounts AS bank ON (x.Id = bank.UserId) " +
                "WHERE (x.Id > @PARAM0) AND (bank.Name = @PARAM1)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual("Fosh", result.Parameters["PARAM1"]);
        }

        [TestMethod]
        public void OtherThanOriginalOrderByTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Join<CreditCards>((x, card) => x.Id == card.UserId, card => new { })
                .Join<CreditCards, BankAccounts>((card, bank) => card.Id == bank.Id, bank => new { bank.Name })
                .OrderBy(x => x.Id)
                .OrderBy<CreditCards>(card => card.UserId)
                .OrderByDesc<BankAccounts>(bank => bank.Id)
                .Build();

            var expected = "SELECT x.Id, card.*, bank.Name " +
                "FROM Users AS x " +
                "JOIN CreditCards AS card ON (x.Id = card.UserId) " +
                "JOIN BankAccounts AS bank ON (card.Id = bank.Id) " +
                "ORDER BY x.Id, card.UserId, bank.Id DESC";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void OtherThanOriginalOrderByMultipleTests()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Join<BankAccounts>((x, bank) => x.FirstName.ToLower() == bank.Name.ToLower(), bank => new { })
                .Join<CreditCards>((x, card) => x.Id == card.UserId, card => new { card.UserId })
                .Where<BankAccounts>(bank => bank.Name == "Fosh")
                .OrderByMultiple<BankAccounts>(bank => new { bank.Id, bank.Balance })
                .OrderByMultipleDesc<Users>(x => new { x.Id, x.FirstName })
                .Build();

            var expected = "SELECT x.*, bank.*, card.UserId " +
                "FROM Users AS x " +
                "JOIN BankAccounts AS bank ON (LOWER(x.FirstName) = LOWER(bank.Name)) " +
                "JOIN CreditCards AS card ON (x.Id = card.UserId) " +
                "WHERE (bank.Name = @PARAM0) " +
                "ORDER BY bank.Id, bank.Balance, x.Id DESC, x.FirstName DESC";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Fosh", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        public void SelectClearTest()
        {
            var builder = new SelectBuilder<Users>()
                .Select(x => new { x.Id })
                .Where(x => x.Active && x.FirstName == "Fosh")
                .OrderBy(x => x.FirstName);

            builder.Clear();
            var result = builder
                .Select(x => new { x.FirstName, x.LastName })
                .Join<SmallClass>((x, s) => x.FirstName == s.FirstName, s => new { })
                .Where(x => x.Active && x.LastName == "Stuff")
                .OrderBy<SmallClass>(s => s.FirstName)
                .Build();

            var expected = "SELECT x.FirstName, x.LastName, s.* " +
                "FROM Users AS x " +
                "JOIN SmallClass AS s ON (x.FirstName = s.FirstName) " +
                "WHERE (x.Active = 1 AND (x.LastName = @PARAM0)) " +
                "ORDER BY s.FirstName";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Stuff", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        public void ExpandoObjectColumnTest()
        {
            dynamic expando = new ExpandoObject();
            var dict = (IDictionary<string, object>)expando;

            dict.Add("FirstName", null);
            dict.Add("LastName", null);

            var result = new SelectBuilder<Users>()
                .Select(x => expando)
                .Build();

            var expected = "SELECT x.FirstName, x.LastName " +
                "FROM Users AS x";

            Assert.AreEqual(expected, result.Query);

            expando = new ExpandoObject();
            dict = (IDictionary<string, object>)expando;

            dict.Add("CreditCard", null);
            dict.Add("TestFosh", null);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Join<BankAccounts>((x, bank) => x.Id == bank.UserId, bank => expando)
                .Build();

            expected = "SELECT x.*, bank.CreditCard, bank.TestFosh " +
                "FROM Users AS x " +
                "JOIN BankAccounts AS bank ON (x.Id = bank.UserId)";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        public void WhereInTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.Id == 5)
                .In(x => x.FirstName, new[] { "Ganondorf", "Zelda" })
                .Build();

            var expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE (x.Id = @PARAM0) AND x.FirstName IN (@PARAM1, @PARAM2)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual("Ganondorf", result.Parameters["PARAM1"]);
            Assert.AreEqual("Zelda", result.Parameters["PARAM2"]);

            result = new SelectBuilder<Users>()
                .Select(x => new { })
                .In(x => new { x.LastName }, new[] { "Dragmire" })
                .Build();

            expected = "SELECT x.* " +
                "FROM Users AS x " +
                "WHERE x.LastName IN (@PARAM0)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Dragmire", result.Parameters["PARAM0"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhereInExceptionOnMultiPropObjectTest()
        {
            new SelectBuilder<Users>()
                .Select(x => new { })
                .In(x => new { x.FirstName, x.LastName }, "Dragmire")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhereInExceptionOnNonMemberExpressionTest()
        {
            new SelectBuilder<Users>()
                .Select(x => new { })
                .In(x => x.FirstName == x.LastName, "Dragmire")
                .Build();
        }

        [TestMethod]
        public void CustomWhereClauseTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .WhereCustomSql($"WHERE x.{nameof(Users.FirstName)} != @Something", new Dictionary<string, object> { { "Something", "Test" } })
                .Build();

            var expected = "SELECT x.* FROM Users AS x WHERE x.FirstName != @Something";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual("Test", result.Parameters["Something"]);
        }

        [TestMethod]
        public void CustomWhereClauseResetsTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .Where(x => x.Id == 5)
                .WhereCustomSql("WHERE x.Id = '5'", new Dictionary<string, object>())
                .Build();

            var expected = "SELECT x.* FROM Users AS x WHERE x.Id = '5'";

            Assert.AreEqual(expected, result.Query);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CustomWhereClauseExceptionTest()
        {
            var result = new SelectBuilder<Users>()
                .Select(x => new { })
                .WhereCustomSql("Id != 5", new Dictionary<string, object>())
                .Build();
        }
    }
}
