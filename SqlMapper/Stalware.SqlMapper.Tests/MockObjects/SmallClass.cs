using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Tests.MockObjects
{
    public class SmallClass
    {
        [IdColumn]
        public long Id { get; set; }
        public string FirstName { get; set; }
    }
}
