using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Entities
{
    public class ChatGPIMessage : Entity
    {
        public virtual Guid UniqueId { get; set; }
        public virtual String Message { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
    }
}
