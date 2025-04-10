using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Entities
{
    public class CMOChatQuestion : Entity
    {
        public virtual String Question { get; set; }
        public virtual Int64 Rank { get; set; }
    }
}
