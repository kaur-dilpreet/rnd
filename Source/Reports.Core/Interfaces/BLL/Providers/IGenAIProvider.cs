using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface IGenAIProvider
    {
        Core.Domain.Models.AskBIModel GetAskBIModel(Int64 userId, Int32 skip);
        Core.Domain.Models.GenAIAnswerModel AskQuestion(Int64 userId, Guid sessionId, Core.Domain.Enumerations.GenAIUsecases usecase, String question);
        Core.Domain.Models.GenAIAnswerModel CheckQuestion(Int64 userId, Guid questionUniqueId);
        void Feedback(Int64 userId, Core.Domain.Enumerations.GenAIUsecases usecase, Guid questionUniqueId, Boolean isCorrect);
        void CreateTicket(Int64 userId, Core.Domain.Enumerations.GenAIUsecases usecase, Core.Domain.Models.SharePointTicketModel model);
    }
}
