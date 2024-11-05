using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models.Dtos
{
    public class CrearConsultorioDto
    {
        [Required(ErrorMessage = "El nombre del consultorio es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [MaxLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres")]
        public string Direccion { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        [MaxLength(20, ErrorMessage = "El número de teléfono no puede tener más de 20 caracteres")]
        public string Telefono { get; set; }
    }
}
