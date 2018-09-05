using System;
using System.ComponentModel.DataAnnotations;

namespace iSeal.API.DTO
{
    public class CredentialModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
