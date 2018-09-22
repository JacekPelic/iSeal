using AutoMapper;
using iSeal.API.Configuration;
using iSeal.API.DTO;
using iSeal.Dal.Contexts;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private readonly iSealDbContext _dbContext;

        public AuthorizationController(
            UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher,
            RootConfiguration apiConfiguration,
            iSealDbContext dbContext
            )
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _apiConfiguration = apiConfiguration;
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public string HelloMessage()
        {
            return "Hello user!";
        }

        [HttpPost("login")]
        public async Task<IActionResult> CreateToken([FromBody] UserCredential credentials)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(credentials.Email);

                if (user != null)
                {
                    if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, credentials.Password) == PasswordVerificationResult.Success)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
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
                                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                                expiration = token.ValidTo
                            }
                            );
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest("Wrong credentials!");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister registerUserModel)
        {
            if (registerUserModel == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }
            try
            {
                User userToCreate = Mapper.Map<User>(registerUserModel);

                Organization organization = _dbContext.Organizations.SingleOrDefault(cus => cus.Name == registerUserModel.Organization);
                if (organization != null)
                {
                    return Conflict("Already extists organization with this name.");
                }
                else
                {
                    organization = new Organization { Name = registerUserModel.Organization };
                    userToCreate.Organization = organization;
                }

                if ( await _userManager.FindByEmailAsync(registerUserModel.Email) != null)
                {
                    return Conflict("User with this email adress already exists!");
                }

                IdentityResult creationResult = await _userManager.CreateAsync(userToCreate, registerUserModel.Password);

                if (!creationResult.Succeeded)
                {
                    //Whenever I try to create user with not unique email it fails (that's desairable behaviour), 
                    //but throw both exeptions of not having email & userName unique
                    return BadRequest(creationResult.Errors);
                }

                return Ok();
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
