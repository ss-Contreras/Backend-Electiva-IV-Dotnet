using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models.Dtos
{
	public class CrearHistorialDto
	{
		[Required(ErrorMessage = "La fecha de creación del historial es obligatoria.")]
		public DateTime Fecha { get; set; }

		[Required(ErrorMessage = "La descripción del historial es obligatoria.")]
		[StringLength(1000, ErrorMessage = "La descripción no puede exceder de 1000 caracteres.")]
		public string Descripcion { get; set; }

		[Required(ErrorMessage = "El ID del paciente es obligatorio.")]
		public int PacienteId { get; set; }

		[Required(ErrorMessage = "El ID del odontólogo es obligatorio.")]
		public int OdontologoId { get; set; }
	}
}
