// SonrisasBackendv01.Models.Dtos.CitasDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models.Dtos
{
	public class CitasDto
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
		public DateTime Fecha { get; set; }

		[Required(ErrorMessage = "El estado de la cita es obligatorio.")]
		public EstadoCita Estado { get; set; }

		[Required(ErrorMessage = "El ID del paciente es obligatorio.")]
		public int PacienteId { get; set; }

		public string NombrePaciente { get; set; }

		[Required(ErrorMessage = "El ID del odontólogo es obligatorio.")]
		public int OdontologoId { get; set; }

		public string NombreOdontologo { get; set; }
	}

	public enum EstadoCita
	{
		Pendiente,
		Completada,
		Cancelada
	}
}
