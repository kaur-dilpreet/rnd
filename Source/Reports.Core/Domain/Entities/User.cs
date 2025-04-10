using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Entities
{
    public class User : Entity
    {
        public virtual Guid UniqueId { get; set; }
        public virtual Byte RoleId { get; set; }
        public virtual String Email { get; set; }
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual String NTID { get; set; }
        public virtual Boolean AskBIAccess { get; set; }
        public virtual Boolean SDRAIAccess { get; set; }
         public virtual Boolean CMOChatAccess { get; set; }
        public virtual Boolean ChatGPIAccess { get; set; }
        public virtual Int32 TimezoneOffset { get; set; }
        public virtual Boolean IsApproved { get; set; }
        public virtual DateTime? LastActivityDateTime { get; set; }
        public virtual Int64? CreatedByUserId { get; set; }
        public virtual Int64? LastModifiedByUserId { get; set; }
	}
}
