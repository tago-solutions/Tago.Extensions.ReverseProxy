using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tago.Extensions.Http;

namespace cookies_backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DefaultController : ControllerBase
    {

        private readonly ILogger<DefaultController> _logger;
        private readonly IRestClientFactory restHttpClientFactory;

        public DefaultController(ILogger<DefaultController> logger, IRestClientFactory restHttpClientFactory)
        {
            _logger = logger;
            this.restHttpClientFactory = restHttpClientFactory;
        }

        [HttpGet]
        public string Get()
        {
            return "Hi there!";
        }

        public class Test
        {
            public string Data { get; set; }
        }


        [AllowAnonymous]
        [HttpGet("test")]
        public string GetTest([FromBody] Test model)
        {
            return "Hi there!";
        }
    }
}
