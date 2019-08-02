using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using ApiREST.Config;
using ApiREST.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRestController : ControllerBase
    {
        /// <summary>
        /// Objeto serializado
        /// </summary>
        private string serializedObject;
      
        /// <summary>
        /// Retorno do Token de validação
        /// </summary>
        /// <param name="obj">Objeto enviado na requisição</param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("gerartoken")]
        public object GerarToken([FromBody]object obj,
                           [FromServices]SignInConfigurations signingConfigurations,
                           [FromServices]TokenConfigurations tokenConfigurations)
        {         

            if (!obj.IsObjectNull())
            {
                serializedObject = obj.SerializeObject();

                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(serializedObject, "Login"),
                    new []{
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))                       
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

                return new
                {
                    authenticated = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Falha ao autenticar"
                };
            }
        }
    }
}
