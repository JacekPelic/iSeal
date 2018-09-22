using System;
using System.ComponentModel.DataAnnotations;

namespace iSeal.API.DTO
{
    public class UserCredential
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
