using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Entities
{
    public class AccessRequest : Entity
    {
        public virtual Guid UniqueId { get; set; }
        public virtual Boolean AskBIAccess { get; set; }
        public virtual Boolean CMOChatAccess { get; set; }
        public virtual Boolean SDRAIAccess { get; set; }
        public virtual Boolean ChatGPIAccess { get; set; }
        public virtual Boolean IsApproved { get; set; }
        public virtual Boolean IsDenied { get; set; }
        public virtual String DenyReason { get; set; }
        public virtual Int64? ResponseBy { get; set; }
        public virtual DateTime? ResponseDateTime { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User ResponseByUser { get; set; }
    }
}
