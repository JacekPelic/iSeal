using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;


namespace iSeal.Dal.Contexts
{
    public class iSealDbContext : IdentityDbContext<User>
    {
        public iSealDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Seal> Seals { get; set; }
    }
}
