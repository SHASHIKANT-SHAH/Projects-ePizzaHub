using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ePizaaHub.API.Filters
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authorization = context.HttpContext.Request.Headers["Authorization"];
            if(!string.IsNullOrEmpty(authorization))
            {
                if(authorization.StartsWith("Bearer "))
                {
                    string token = authorization.Substring("Bearer ".Length).Trim();
                    if(!string.IsNullOrEmpty(token) )
                    {
                        var config = context.HttpContext.RequestServices.GetService<IConfiguration>();
                        string jwtKey = config.GetValue<string>("Jwt:Key");
                        string jwtIssuer = config.GetValue<string>("Jwt:Issuer");
                        string jwtAudience = config.GetValue<string>("Jwt:Audience");

                        SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
                        TokenValidationParameters validationParameters =
                                new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = jwtIssuer,
                                    ValidAudiences = new[] { jwtAudience },
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = key
                                };

                        SecurityToken validatedToken;
                        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                        var user = handler.ValidateToken(token, validationParameters, out validatedToken);
                        if (!user.Identity.IsAuthenticated)
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
