using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models
{
    public class Odontologo
    {
        [Key]
        public int Id { get; set; }

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

        // Relación con Consultorio
        public int ConsultorioId { get; set; }
        public Consultorio Consultorio { get; set; }

		// Relación con Pacientes y Citas
		public ICollection<Historial> Historiales { get; set; }
		public ICollection<Paciente> Pacientes { get; set; }
        public ICollection<Cita> Citas { get; set; }
    }
}
