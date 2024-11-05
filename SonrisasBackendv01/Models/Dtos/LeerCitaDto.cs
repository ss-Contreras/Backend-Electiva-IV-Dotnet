// Dtos/LeerCitaDto.cs
using System;

namespace SonrisasBackendv01.Dtos
{
	public class LeerCitaDto
	{
		/// <summary>
		/// Identificador único de la cita.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Fecha y hora de la cita en formato ISO 8601.
		/// </summary>
		public DateTime Fecha { get; set; }

		/// <summary>
		/// Estado actual de la cita (e.g., "Pendiente", "Completada", "Cancelada").
		/// </summary>
		public string Estado { get; set; }

		/// <summary>
		/// Motivo de la cita.
		/// </summary>
		public string Motivo { get; set; }

		/// <summary>
		/// Identificador del paciente asociado a la cita.
		/// </summary>
		public int PacienteId { get; set; }

		/// <summary>
		/// Nombre completo del paciente.
		/// </summary>
		public string NombrePaciente { get; set; }

		/// <summary>
		/// Correo electrónico del paciente.
		/// </summary>
		public string CorreoElectronicoPaciente { get; set; }

		/// <summary>
		/// Identificador del odontólogo asociado a la cita.
		/// </summary>
		public int OdontologoId { get; set; }

		/// <summary>
		/// Nombre completo del odontólogo.
		/// </summary>
		public string NombreOdontologo { get; set; }
	}
}
