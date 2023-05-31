using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tago.Extensions.Http;
using Tago.Extensions.Jwt.Abstractions.Interfaces;

namespace cookies_backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AuthController : ControllerBase
    {
       
        private readonly ILogger<AuthController> _logger;
        private readonly IRestClientFactory restHttpClientFactory;
        private readonly ITokenGenerator tokenGenerator;

        public AuthController(ILogger<AuthController> logger, IRestClientFactory restHttpClientFactory, ITokenGenerator tokenGenerator)
        {
            _logger = logger;
            this.restHttpClientFactory = restHttpClientFactory;
            this.tokenGenerator = tokenGenerator;
        }

        [Authorize]
        [HttpGet]       
        public string Get()
        {
            return "Hi from authorized method";
        }

        [AllowAnonymous]
        [HttpGet("get1")]
        public string Get1([FromQuery] MyRequest req)
        {
            return "Hi from authorized method";
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            using(var sr = new StreamReader(Request.Body))
            {
                var res = await sr.ReadToEndAsync();

                return Ok(new { payload = res, headers = Request.Headers.ToDictionary((k)=> k.Key, v=> v.Value)});
            }            
        }

        [AllowAnonymous]
        [HttpPost("oauth")]
        public async Task<ActionResult> Oauth()
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var res = await sr.ReadToEndAsync();
                var jwt = tokenGenerator.Sign(new JwtPayload(), "A1E06B6586BF5203A7B3EC9C7D4CB53B862E4D19");
                return Ok(jwt);
            }
        }




        [Authorize]
        [HttpPost("post1")]
        public async Task<ActionResult> Post([FromBody] Test test)
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var res = await sr.ReadToEndAsync();

                return Ok(new { payload = res, headers = Request.Headers.ToDictionary((k) => k.Key, v => v.Value) });
            }
        }

        [Authorize]
        [HttpPost("post2")]
        public async Task<ActionResult> Post([FromQuery] MyRequest2 req)
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var res = await sr.ReadToEndAsync();

                return Ok(new { payload = res, headers = Request.Headers.ToDictionary((k) => k.Key, v => v.Value) });
            }
        }



        [AllowAnonymous]
        [HttpGet("anon")]
        public string GetAnonymous()
        {
            return "Hi from Anonymous method";
        }

    }


    public class Test
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public TestChild[] Child { get; set; }

    }

    public class TestChild
    {
        [Required]
        public string Name { get; set; }

        public TestChild Parent { get; set; }
    }


    public class MyRequest
    {
        [Required]
        [FromHeader(Name ="header-1")]
        public string Header1 { get; set; }


    }

    public class MyRequest2
    {
        [Required]
        [FromHeader(Name = "header-1")]
        public string Header1 { get; set; }


        [Required]
        [FromQuery]
        public string opt1 { get; set; }

        [FromBody]
        public PayLoad1 Payload { get; set; }

    }

    public class PayLoad1
    {
        [Required]
        public string Simple { get; set; }

        [Required]
        public Test Test { get; set; }

        public int?[] Array { get; set; }

        public long? Long { get; set; }
        public double? Double { get; set; }
        public float? Float { get; set; }
        public Int64? Int64 { get; set; }

        public bool? Bool { get; set; }

        public Guid? Guid { get; set; }

        public Boolean? Boolean { get; set; }
    }


}
