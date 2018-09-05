using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iSeal.Dal
{
    public class iSealDbInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly iSealDbContext _dbContext;

        public iSealDbInitializer(
            UserManager<User> userManager,
            iSealDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        //Consider moving Seed values to config file
        public async Task Seed()
        {
            _dbContext.Database.Migrate();

            if (!(await _dbContext.Users.AnyAsync()))
            {
                var user = new User
                {
                    UserName = "isealAdmin",
                    Email = "admin@iseal.com"
                };
                await _userManager.CreateAsync(user, "!se4lP4ss");
                var adminUser = await _userManager.FindByEmailAsync("admin@iseal.com");
                await _userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "admin"));
            }
        }
    }
}
