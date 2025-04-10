using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reports.Core.Domain.Models
{
    public class RequestAccessModel
    {
        public Boolean IsApproved { get; set; }
        public Boolean IsDenied { get; set; }
        public String DenyReason { get; set; }
        public Boolean HasPendingRequest { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public DateTime AccessRequestSubmitionDateTime { get; set; }
    }

    public class AccessRequestsModel
    { 
        public List<AccessRequestModel> Requests { get; set; }
    }

    public class AccessRequestModel
    {
        public Guid UniqueId { get; set; }
        public UserModel Requester { get; set; }
        public Boolean IsApproved { get; set; }
        public Boolean IsDenied { get; set; }
        public String DenyReason { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public UserModel Responder { get; set; }
    }

    public class DenyRequestModel
    {
        public Guid UniqueId { get; set; }
        public String RequesterFullName { get; set; }
        public String RequesterEmail { get; set; }

        [Display(Name = "Comment")]
        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters are allowed.")]
        public String Comment { get; set; }
    }

    public class ChangeAccessLevelModel
    {
        public Guid UniqueId { get; set; }
        public String RequesterFullName { get; set; }
        public String RequesterEmail { get; set; }

        [Display(Name = "Ask BI")]
        public Boolean AskBIAccess { get; set; }

        [Display(Name = "CMO Chat")]
        public Boolean CMOChatAccess { get; set; }

        [Display(Name = "SDR AI")]
        public Boolean SDRAIAccess { get; set; }

        [Display(Name = "Chat GPI")]
        public Boolean ChatGPIAccess { get; set; }

        public UserRequest UserRequest { get; set; }
    }

    public class UserRequest
    {
        [Display(Name = "Ask BI")]
        public Boolean AskBIAccess { get; set; }

        [Display(Name = "CMO Chat")]
        public Boolean CMOChatAccess { get; set; }

        [Display(Name = "SDR AI")]
        public Boolean SDRAIAccess { get; set; }

        [Display(Name = "Chat GPI")]
        public Boolean ChatGPIAccess { get; set; }
    }
}
