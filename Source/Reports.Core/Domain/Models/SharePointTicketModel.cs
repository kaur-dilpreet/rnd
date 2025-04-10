using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reports.Core.Domain.Models
{
    public class SharePointTicketModel
    {
        public Guid UniqueId { get; set; }

        [Display(Name = "Issue Description")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters are allowed.")]
        public String IssueDescription { get; set; }

        public String Question { get; set; }
        public String Response { get; set; }

        public String Controller { get; set; }
    }
}
