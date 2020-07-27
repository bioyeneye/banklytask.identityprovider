using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.DataAccess.Entities
{
    public class ApplicationIdentityUser : IdentityUser
    {
        public ApplicationIdentityUser()
        {
            CreatedTime = DateTime.UtcNow;
        }
        
        [Required, PersonalData]
        public string FirstName { get; set; }
        
        [Required, PersonalData]
        public string LastName { get; set; }
        
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}