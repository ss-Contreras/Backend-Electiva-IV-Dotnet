// Dtos/ActualizarCitaDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Dtos
{
	public class ActualizarCitaDto
	{
		[Required(ErrorMessage = "El ID de la cita es obligatorio.")]
		public int Id { get; set; }

		[Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
		public DateTime Fecha { get; set; }

		[Required(ErrorMessage = "El estado de la cita es obligatorio.")]
		[StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
		public string Estado { get; set; }

		[Required(ErrorMessage = "El motivo de la cita es obligatorio.")]
		[StringLength(500, ErrorMessage = "El motivo de la cita no puede exceder los 500 caracteres.")]
		public string Motivo { get; set; }

		[Required(ErrorMessage = "El ID del paciente es obligatorio.")]
		public int PacienteId { get; set; }

		[Required(ErrorMessage = "El ID del odontólogo es obligatorio.")]
		public int OdontologoId { get; set; }
	}
}
