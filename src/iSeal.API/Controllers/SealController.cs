using AutoMapper;
using iSeal.API.DTO;
using iSeal.Dal.Contexts;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.API.Controllers
{
    [Authorize]
    [Route("api/seal")]
    public class SealController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly iSealDbContext _dbContext;

        public SealController(UserManager<User> userManager, iSealDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterSeal([FromBody] SealRegister sealToRegister)
        {
            if (sealToRegister == null)
            {
                BadRequest();
            }

            string userName = HttpContext.User.Identity.Name;
            User user = await _userManager.FindByNameAsync(userName);

            if (user.Organization == null)
            {
                return BadRequest("User is not subscribied to any Organization");
            }

            var seal = Mapper.Map<Seal>(sealToRegister);
            seal.Organization = user.Organization;

            _dbContext.Add(seal);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Unlock")]
        public async Task<IActionResult> AccessSeal([FromBody] SealAccessRequest sealAccessModel)
        {
            if (sealAccessModel == null)
            {
                return BadRequest();
            }

            string userName = HttpContext.User.Identity.Name;
            User user = await _userManager.FindByNameAsync(userName);

            var seal = user.Organization.Seals.FirstOrDefault(se => se.Guid == sealAccessModel.Guid);

            if (seal == null)
            {
                return Unauthorized();
            }
            else
            {
                SealAccessResponse sealAccessResponse = Mapper.Map<SealAccessResponse>(seal);
                return Ok(sealAccessResponse);
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSealsForOrganization()
        {
            string userName = HttpContext.User.Identity.Name;
            User user = await _userManager.FindByNameAsync(userName);

            if (user.Organization == null)
            {
                return BadRequest("User is not subscribied to any Organization");
            }

            IEnumerable<SealViewModel> seals =  Mapper.Map<IEnumerable<SealViewModel>>(user.Organization.Seals);

            return Ok(seals);
        }
    }
}
