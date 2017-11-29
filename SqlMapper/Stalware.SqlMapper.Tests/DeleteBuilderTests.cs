using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    [TestClass]
    public class DeleteBuilderTests
    {
        [TestMethod]
        public void DeleteTest()
        {
            var result = new DeleteBuilder<Users>(Utils.GetMockUser())
                .Build();

            var expected = "DELETE FROM Users " +
                "WHERE Id = @PARAM0";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
        }

        [TestMethod]
        public void DeleteByPreventWhereOnId()
        {
            var result = new DeleteBuilder<Users>(Utils.GetMockUser())
                .PreventWhereOnIdAutoAdd()
                .Build();

            var expected = "DELETE FROM Users";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(0, result.Parameters.Count);
        }

        [TestMethod]
        public void DeleteWithWhereTest()
        {
            var result = new DeleteBuilder<Users>(Utils.GetMockUser())
                .Where(x => x.Balance == 0 || (x.Active && x.Id != 5))
                .Build();

            var expected = "DELETE FROM Users " +
                "WHERE ((Balance = @PARAM0) OR (Active = 1 AND (Id != @PARAM1)))";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(0, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM1"]));
        }

        [TestMethod]
        public void DeleteWithWhereChainTest()
        {
            var result = new DeleteBuilder<Users>(Utils.GetMockUser())
                .Where(x => x.Id == 5)
                .Where(x => x.Id == 4)
                .Build();

            var expected = "DELETE FROM Users " +
                "WHERE (Id = @PARAM0) AND (Id = @PARAM1)";

            Assert.AreEqual(expected, result.Query);
            Assert.AreEqual(5, Convert.ToInt32(result.Parameters["PARAM0"]));
            Assert.AreEqual(4, Convert.ToInt32(result.Parameters["PARAM1"]));
        }

        [TestMethod]
        public void DeleteClearTest()
        {
            var builder = new DeleteBuilder<Users>(Utils.GetMockUser())
                .Where(x => x.Active)
                .Where(x => x.FirstName == "Ganondorf")
                .Clear();

            var result = builder
                .PreventWhereOnIdAutoAdd()
                .Build();

            var expected = "DELETE FROM Users";
            Assert.AreEqual(expected, result.Query);
        }
    }
}
