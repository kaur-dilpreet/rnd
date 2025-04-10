using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public class HomeProvider : IHomeProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.IUsersRepository UsersRepo;

        public HomeProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                             Core.Utilities.ISettings settings,
                             Core.Utilities.IUtilities utilities,
                             Data.Repositories.IUsersRepository usersRepo)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.UsersRepo = usersRepo;
        }

        public Core.Domain.Models.HomeModel GetHomeModel(Int64 userId)
        {
            try
            {
                Core.Domain.Models.HomeModel model = new Core.Domain.Models.HomeModel();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
