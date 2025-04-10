using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client.Publishing;
using StackExchange.Redis;

namespace Reports.BLL.Providers
{
    public class RedisProvider
    {
        public static String GetCache(String key)
        {
            string connectionString = "mktai-dev-elastic-cache-otrox9.serverless.use1.cache.amazonaws.com:6379";

            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.Ssl = true;

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);

            IDatabase db = redis.GetDatabase();

            // Get the value associated with the key
            string value = db.StringGet(key);

            return value;
        }
    }
}
