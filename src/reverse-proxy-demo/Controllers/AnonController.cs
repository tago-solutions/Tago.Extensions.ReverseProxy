using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tago.Extensions.Http;

namespace cookies_backend.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("anon")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AnonController : ControllerBase
    {
       
        private readonly ILogger<AnonController> _logger;

        public AnonController(ILogger<AnonController> logger)
        {
            _logger = logger;
        }

        //[Authorize]
        [HttpGet]
        [HttpPost]
        public string Get()
        {
            return "Hi from Anonymous method";
        }
    }
}
