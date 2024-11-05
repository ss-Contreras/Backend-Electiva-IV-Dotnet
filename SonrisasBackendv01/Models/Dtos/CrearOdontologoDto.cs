using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models.Dtos
{
    public class CrearOdontologoDto
    {
        [Required(ErrorMessage = "El nombre del odontólogo es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre del odontólogo es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        [MaxLength(20, ErrorMessage = "El número de licencia no puede exceder los 20 caracteres")]
        public string NumeroLicencia { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El identificador del consultorio es obligatorio")]
        public int ConsultorioId { get; set; }
    }
}
