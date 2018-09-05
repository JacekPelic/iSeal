using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;


namespace iSeal.Dal
{
    public class iSealDbContext : IdentityDbContext<User>
    {
        public iSealDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
