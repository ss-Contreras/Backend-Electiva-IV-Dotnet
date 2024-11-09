using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models
{
    public class Paciente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La identificación del paciente es obligatoria")]
        [MaxLength(15, ErrorMessage = "La cédula no puede exceder los 15 caracteres")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "La edad del paciente es obligatoria")]
        [Range(1, 120, ErrorMessage = "La edad debe estar entre 1 y 120 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria")]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "Indique si es un nuevo paciente")]
        public OpcionBinaria NuevoPaciente { get; set; }

        [Required(ErrorMessage = "Indique si es un paciente recomendado")]
        public OpcionBinaria PacienteRecomendado { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
        public string MotivoConsulta { get; set; }

        [MaxLength(255, ErrorMessage = "La ruta de la imagen no puede exceder los 255 caracteres")]
        [RegularExpression(@"(.*\.jpg|.*\.png)$", ErrorMessage = "Solo se permiten imágenes en formato .jpg o .png")]
        public string RutaImagen { get; set; }

        [MaxLength(255, ErrorMessage = "La ruta local de la imagen no puede exceder los 255 caracteres")]
        public string RutaLocalImagen { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [MaxLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres")]
        public string Direccion { get; set; }

        // Relación con Odontólogo
        public int OdontologoId { get; set; }
        public Odontologo Odontologo { get; set; }

		public ICollection<Historial> Historiales { get; set; }

		// Relación con Citas
		public ICollection<Cita> Citas { get; set; }
        // Relacion con Radiografias
		public ICollection<Radiografia> Radiografias { get; set; }
	}

    public enum OpcionBinaria
    {
        Si,
        No
    }
}
