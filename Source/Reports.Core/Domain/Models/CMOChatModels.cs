using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reports.Core.Domain.Models
{
    public class CMOChatModel
    {
        [Display(Name = "Question")]
        [Required(ErrorMessage = "Question is required.")]
        [StringLength(1000, ErrorMessage = "Maximum 1000 characters are allowed.")]
        public String Question { get; set; }

        [Display(Name = "Display SQL")]
        public Boolean DisplaySQL { get; set; }

        public List<CMOChatQuestion> History { get; set; }
        public List<String> Suggestions { get; set; }
    }

    public class CMOChatQuestion
    {
        public Guid UniqueId { get; set; }
        public String Question { get; set; }
        public String Answer { get; set; }
        public String DateText { get; set; }
        public String NLText { get; set; }
        public String SQL { get; set; }
        public Boolean? IsCorrect { get; set; }
        public Boolean TicketOpened { get; set; }
        public String ResponseType { get; set; }
        public DateTime CreationDateTime { get; set; }
        public List<String> Suggestions { get; set; }
    }

    public class CMOChatQuestionModel : GenAIQuestionModel
    {
        
    }

    public class CMOChatFeedbackModel
    {
        public Int64 qid { get; set; }
        public String uid { get; set; }
        public Boolean radio { get; set; }
    }

    public class CMOChatQuestionResponse
    {
        public Int64? qid { get; set; }
        public String uid { get; set; }
        public object answer { get; set; }
        public String sql { get; set; }
        public String nl_text { get; set; }
        public String date_text { get; set; }
        public String response_type { get; set; }
        public List<String> suggestions { get; set; }
    }

    public class CMOChatCampaingsLeadIds
    {
        [JsonProperty("campaigns")]
        public List<String> Campaigns { get; set; }

        [JsonProperty("lead_ids")]
        public List<String> LeadIds { get; set; }
    }
}
