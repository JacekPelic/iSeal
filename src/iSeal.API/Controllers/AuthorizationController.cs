using iSeal.API.Configuration;
using iSeal.API.DTO;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iSeal.API.Controllers
{
    [Route("api/auth")]
    public class AuthorizationController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RootConfiguration _apiConfiguration;

        public AuthorizationController(
            UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher,
            RootConfiguration apiConfiguration
            )
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _apiConfiguration = apiConfiguration;
        }

        [HttpPost("token")]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel credentials)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(credentials.UserName);

                if (user != null)
                {
                    if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, credentials.Password) == PasswordVerificationResult.Success)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.Key));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _apiConfiguration.Issuer,
                            audience: _apiConfiguration.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(1),
                            signingCredentials: creds
                            );

                        return Ok(
                            new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expiration = token.ValidTo
                            }
                            );
                    }
                }
            }
            catch (Exception)
            {                
            }
            return BadRequest("Failed to generate token");
        }
    }
}
