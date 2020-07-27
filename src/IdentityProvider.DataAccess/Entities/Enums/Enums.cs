using System.ComponentModel;

namespace IdentityProvider.DataAccess.Entities.Enums
{
    public class RolesConstants
    {
        public enum Enum
        {
            [Description(nameof(SuperAdministrator))] 
            SuperAdministrator = 1,
            
            [Description(nameof(Administrator))] 
            Administrator,
            
            [Description(nameof(User))] 
            User
        }
    }
}