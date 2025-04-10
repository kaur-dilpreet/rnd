using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Reports.Core.Domain.Models
{
    public class AskBIModel
    {
        public AskBIModel() 
        {
            this.SessionId = Guid.NewGuid();
        }
        
        [Display(Name = "Question")]
        [Required(ErrorMessage = "Question is required.")]
        [StringLength(1000, ErrorMessage = "Maximum 1000 characters are allowed.")]
        public String Question { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required.")]
        public String Category { get; set; }

        [Display(Name = "Display SQL")]
        public Boolean DisplaySQL { get; set; }

        public List<AskBIQuestion> History { get; set; }
        public AskBIVersionModel Version { get; set; }

        public Guid SessionId { get; set; }

        public String ErrorMessage { get; set; }
    }

    public class AskBIQuestion
    {
        public Guid UniqueId { get; set; }
        public String Question { get; set; }
        public String Answer { get; set; }
        public String Data { get; set; }
        public String SQL { get; set; }
        public String Message { get; set; }
        public Boolean? IsCorrect { get; set; }
        public Boolean TicketOpened { get; set; }
        public String Status { get; set; }
        public DateTime CreationDateTime { get; set; }
    }

    public class AskBIQuestionModel : GenAIQuestionModel
    {
        
    }

    public class AskBIFeedbackModel
    {
        [JsonProperty("qid")]
        public Int64 qid { get; set; }

        [JsonProperty("uid")]
        public String UserId { get; set; }

        [JsonProperty("question")]
        public String Question { get; set; }

        [JsonProperty("radio")]
        public Boolean Radio { get; set; }

        [JsonProperty("usecase")]
        public String Usecase { get; set; }
    }

    public class AskBIQuestionResponse
    {
        public Int64? qid { get; set; }
        public String uid { get; set; }
        public String answer { get; set; }
        public String sql { get; set; }
        public String message { get; set; }
        public String status { get; set; }
        public List<List<List<object>>> data { get; set; }
    }

    public class AskBIVersionModel
    {
        [JsonProperty("model_version")]
        public String ModelVersion { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("release_version")]
        public String ReleaseVersion { get; set; }

        [JsonProperty("table_categories")]
        public List<String> TableCategories { get; set; }

        [JsonProperty("data_until_dates")]
        public List<AskBIUntilDate> DataUntilDates { get; set; }

        [JsonProperty("message")]
        public String Message { get; set; }
    }

    public class AskBIUntilDate
    {
        [JsonProperty("source")]
        public String Source { get; set; }

        [JsonProperty("date")]
        public String Date { get; set; }
    }
}
