using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Entities
{
    public class CMOChatHistory : Entity
    {
        public virtual Guid UniqueId { get; set; }
        public virtual Int64 QuestionId { get; set; }
        public virtual String Question { get; set; }
        public virtual String Answer { get; set; }
        public virtual String DateText { get; set; }
        public virtual String NLText { get; set; }
        public virtual String SQLQuery { get; set; }
        public virtual String Message { get; set; }
        public virtual Boolean? IsCorrect { get; set; }
        public virtual Boolean TicketOpened { get; set; }
        public virtual Byte Status { get; set; }
        public virtual Guid SessionId { get; set; }
    }
}
