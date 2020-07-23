using System.ComponentModel;

namespace banklytask.identityprovider.Models
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