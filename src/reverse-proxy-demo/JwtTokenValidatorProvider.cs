using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tago.Extensions.ReverseProxy.Abstractions;

namespace Tago.Infra.Proxy
{
    internal class JwtTokenValidatorProvider : ITokenValidatorProvider
    {
        public string Scheme { get; set; }

        public string HeaderName { get; set; }

        public Task<ClaimsPrincipal> ValidateToken(string token)
        {
            //DEMO
            ClaimsPrincipal res = null;
            if (!Guid.TryParse(token, out var value))
            {
                var id = new ClaimsIdentity(null, "Jwt Bearer");
                id.AddClaim(new Claim("token", token));
                res = new ClaimsPrincipal(id);
                //res.add
            }

            return Task.FromResult(res);
        }
    }
}
