using iSeal.Dal.Contexts;
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
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.Migrate();

            if (!(await _dbContext.Users.AnyAsync()))
            {
                var organization = new Organization
                {
                    Name = "iSeal"
                };

                _dbContext.Organizations.Add(organization);

                var seal = new Seal
                {
                    SyncKey = "asfbkqbriq2uh39fh103u^%#!@sfd",
                    Organization = organization
                };
                _dbContext.Seals.Add(seal);
                _dbContext.SaveChanges();

                var user = new User
                {
                    UserName = "admin@iseal.com",
                    Email = "admin@iseal.com",
                    Organization = organization
                };

                await _userManager.CreateAsync(user, "!se4lP4ss");
                var adminUser = await _userManager.FindByEmailAsync("admin@iseal.com");
                await _userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "admin"));
            }
        }
    }
}
