using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests.MockObjects
{
    public class CreditCards
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public long UserId { get; set; }
        public long BankAccountId { get; set; }
        public long CardNumber { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Active { get; set; }
    }
}
