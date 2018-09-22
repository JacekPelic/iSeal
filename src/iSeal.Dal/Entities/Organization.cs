using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iSeal.Dal.Entities
{
    public class Organization : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Seal> Seals { get; set; }
    }
}
