using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests.MockObjects
{
    public class BankAccounts
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
