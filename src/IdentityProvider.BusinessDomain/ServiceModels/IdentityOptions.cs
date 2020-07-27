using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.BusinessDomain.ServiceModels
{
    public class IdentityProviderOptions
    {
        public IdentityPasswordOptions IdentityPasswordOptions { get; set; }
        public IdentityLockoutOptions IdentityLockoutOptions { get; set; }
    }

    public class IdentityLockoutOptions : LockoutOptions
    {
        public int DefaultLockoutTime { get; set; }
    }

    public class IdentityPasswordOptions : PasswordOptions
    {
        
    }
}
