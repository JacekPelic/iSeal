using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using iSeal.API.DTO;
using iSeal.Dal.Contexts;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace iSeal.API.Controllers
{
    [Authorize]
    [Route("api/organization")]
    public class ManageOrganizationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly iSealDbContext _dbContext;

        public ManageOrganizationController(UserManager<User> userManager, iSealDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        
        [HttpGet("Users/{organizationName}")]
        public IActionResult GetUsers(string organizationName)
        {
            if (organizationName == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var users = _userManager.Users.Where(user => user.Organization.Name == organizationName);
            IEnumerable<OrganizationUser> organizationUsers = Mapper.Map<IEnumerable<OrganizationUser>>(users);

            return Ok(organizationUsers);
        }
        
        [HttpPost()]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationRegister organizationToRegister)
        {
            if (organizationToRegister == null)
            {
                return BadRequest();
            }

            Organization organization = _dbContext.Organizations.FirstOrDefault(cus => cus.Name == organizationToRegister.Name);

            if (organization == null)
            {
                organization = Mapper.Map<Organization>(organizationToRegister);
                _dbContext.Add(organization);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return Conflict("Organization with this Name already exists.");
            }             
        }
    }
}