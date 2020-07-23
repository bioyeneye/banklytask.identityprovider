using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using banklytask.identityprovider.Extensions;
using Microsoft.AspNetCore.Identity;

namespace banklytask.identityprovider.Models
{
    public class ApplicationIdentityUser : IdentityUser, ITimestampedEntity
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