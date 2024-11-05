using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models.Dtos
{
    public class CrearPacienteDto
    {
        [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

		[Required(ErrorMessage = "El ID del odontólogo es obligatorio.")]
		public int OdontologoId { get; set; }

		[Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(15, ErrorMessage = "La cédula no puede exceder los 15 caracteres.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        public int Edad { get; set; }

		[Required(ErrorMessage = "La fecha de cita es obligatoria.")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "El campo nuevo paciente es obligatorio.")]
        public OpcionBinaria NuevoPaciente { get; set; }

        [Required(ErrorMessage = "El campo paciente recomendado es obligatorio.")]
        public OpcionBinaria PacienteRecomendado { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio.")]
        public string MotivoConsulta { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres.")]
        public string Direccion { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Imagen { get; set; } // Archivo de imagen
    }
}
