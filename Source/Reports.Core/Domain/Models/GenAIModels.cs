using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class GenAIQuestionModel
    {
        [JsonProperty("uid")]
        public String UserId { get; set; }

        [JsonProperty("qid")]
        public String QuestionId { get; set; }

        [JsonProperty("question")]
        public String Question { get; set; }

        [JsonProperty("usecase")]
        public String Usecase { get; set; }

        [JsonProperty("session_id")]
        public String SessionId { get; set; }
    }

    public class GenAIAnswerModel
    {
        public Guid UniqueId { get; set; }
        public Guid QuestionId { get; set; }
        public String Question { get; set; }
        public String Answer { get; set; }
        public String Data { get; set; }
        public String SQL { get; set; }
        public String Message { get; set; }
        public Boolean? IsCorrect { get; set; }
        public Boolean TicketOpened { get; set; }
        public String Status { get; set; }
        public DateTime CreationDateTime { get; set; }
        public String Response {  get; set; }
        public T Object { get; set; }
    }

    public class GenAIAnswerResponse
    {
        [JsonProperty("qid")]
        public String QuestionId { get; set; }

        [JsonProperty("uid")]
        public String UserId { get; set; }

        [JsonProperty("answer")]
        public String Answer { get; set; }

        [JsonProperty("sql")]
        public String SQL { get; set; }

        [JsonProperty("message")]
        public String Message { get; set; }

        [JsonProperty("status")]
        public String Status { get; set; }

        [JsonProperty("data")]
        public List<List<List<object>>> Data { get; set; }

        [JsonProperty("usecase")]
        public String Usecase { get; set; }
    }

    public class GenAIFeedbackRequest
    {
        [JsonProperty("qid")]
        public String QuestionId { get; set; }

        [JsonProperty("uid")]
        public String UserId { get; set; }

        [JsonProperty("radio")]
        public Boolean Radio { get; set; }

        [JsonProperty("usecase")]
        public String Usecase { get; set; }
    }
}
