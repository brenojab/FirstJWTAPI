using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstJWTAPI.Models;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Diagnostics;

namespace FirstJWTAPI.Controllers
{
  [Route("api/[controller]")]
  public class AuthController : Controller
  {
    private ILogger<AuthController> _logger;
    private string _tok;
    private readonly string ENDERECO_PAGINA = "theworld20171022.azurewebsites.net";

    public AuthController(ILogger<AuthController> logger)
    {
      _logger = logger;
    }


    public IActionResult Index()
    {
      return View();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody]CredentialModel model)
    {
      if (string.IsNullOrEmpty(_tok))
      {
        if (UsuarioValido(model))
        {

          try
          {
            var claims = new[]
            {
          new Claim(JwtRegisteredClaimNames.Sub, model.Usuario),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ENDERECO_PAGINA + model.Usuario));

            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
              (
              issuer: ENDERECO_PAGINA,
              audience: ENDERECO_PAGINA,
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(15),
              signingCredentials: creds

              );

            var tok = new
            {
              token = new JwtSecurityTokenHandler().WriteToken(token),
              expiration = token.ValidTo
            };

            _tok = tok.token;

            return Ok("Token gerado");

          }
          catch (Exception ex)
          {

            throw;
          }
        }
        else
        {
          return BadRequest("Não foi possível fazer o login.");
        }
      }
      else
      {
        return Ok("Token já gerado!");
      }

      return BadRequest("Ocorreu algum erro na autenticação...");
    }

    private bool UsuarioValido(CredentialModel model)
    {
      return model.Usuario == "Breno" && model.Senha == "123";
    }

    [HttpPost]
    public IActionResult CreateToken()
    {
      return View();
    }
  }
}