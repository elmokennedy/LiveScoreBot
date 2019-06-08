using System.Net;

namespace BotBLL.Infrastructure
{
    public class ClientConfigurator
    {
        public static WebClient CreateWebClient(string authToken)
        {
            var headerCollection = new WebHeaderCollection();
            headerCollection.Add("X-Auth-Token", authToken);

            return new WebClient
            {
                Headers = headerCollection
            };
        }
    }
}
