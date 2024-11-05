using Microsoft.AspNetCore.Identity;

namespace SonrisasBackendv01.Models
{
    public class AppUsuario : IdentityUser
    {
        public string Nombre { get; set; }
    }

}
