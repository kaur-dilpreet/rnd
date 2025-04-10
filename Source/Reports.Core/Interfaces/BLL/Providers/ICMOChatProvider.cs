using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface ICMOChatProvider
    {
        void LoadQuestionsData();
        Core.Domain.Models.CMOChatModel GetCMOChatModel(Int64 userId, Int32 skip);
        Core.Domain.Models.CMOChatQuestion AskQuestion(Int64 userId, String question, Boolean displaySQL, Boolean isPredefinedQuestion);
        void SetQuestionIsCorrect(Int64 userId, Guid questionUniqueId, Boolean isCorrect);
        List<String> ListQuestions(Int64 userId, String question);
        void CreateTicket(Int64 userId, Core.Domain.Models.SharePointTicketModel model);
        Core.Domain.Models.CMOChatCampaingsLeadIds ListCampaignsLeadIds();
    }
}
