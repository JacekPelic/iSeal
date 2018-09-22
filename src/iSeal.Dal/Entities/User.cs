using Microsoft.AspNetCore.Identity;
using System;


namespace iSeal.Dal.Entities
{
    public class User : IdentityUser
    {
        public virtual Organization Organization { get; set; }
        public int? OrganizationId { get; set; }
    }
}
