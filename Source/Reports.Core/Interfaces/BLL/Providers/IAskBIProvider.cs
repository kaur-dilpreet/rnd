using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface IAskBIProvider
    {
        Core.Domain.Models.AskBIModel GetAskBIModel(Int64 userId, Int32 skip);
        Core.Domain.Models.AskBIQuestion AskQuestion(Int64 userId, Guid sessionId, Core.Domain.Enumerations.GenAIUsecases usecase, String question, String tableCategory, Boolean displaySQL);
        Core.Domain.Models.AskBIQuestion CheckQuestion(Int64 userId, Guid questionId);
        void Feedback(Int64 userId, Guid questionUniqueId, Boolean isCorrect);
        void CreateTicket(Int64 userId, Core.Domain.Models.SharePointTicketModel model);
        List<Core.Domain.Models.AskBIQuestion> ListSesstionQuestions(Int64 userId, Guid sessionId);
    }
}
