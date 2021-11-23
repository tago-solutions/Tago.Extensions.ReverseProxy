using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Tago.Extensions.ReverseProxy.Abstractions;

namespace Tago.Infra.Proxy
{
    public class MyIEncryptionHandler2 : IEncryptionHandler, IDigestHandler
    {
        public MyIEncryptionHandler2(ILogger<MyIEncryptionHandler2> logger)
        {
            //var ms = Org.BouncyCastle.Utilities.Date.DateTimeUtilities.CurrentUnixMs();
        }

        public string Create(Stream payload)
        {
            return "test digest";
        }

        public Task<string> EncryptyAsync(Stream inputStream, IProxyEndpointSettings endpoint)
        {
            return Task.FromResult("");
        }
    }
}
