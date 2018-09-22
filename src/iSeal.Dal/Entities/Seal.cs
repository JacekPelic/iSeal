using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace iSeal.Dal.Entities
{
    public class Seal : BaseEntity
    {
        public virtual Organization Organization { get; set; }
        public int? OrganizationId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Guid { get; set; }

        public string SyncKey { get; set; }
    }
}
