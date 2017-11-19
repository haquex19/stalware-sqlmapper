using Stalware.SqlMapper.Tests.MockObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests
{
    public static class Utils
    {
        public static Users GetMockUser()
        {
            return new Users
            {
                Id = 5,
                CreatedAt = DateTime.Today,
                ModifiedAt = DateTime.Today,
                FirstName = "Ganondorf",
                LastName = "Dragmire",
                Email = "ganon@gerudo.valley",
                Balance = 9999,
                Active = true
            };
        }
    }
}
