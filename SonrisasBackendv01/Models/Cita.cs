using System.ComponentModel.DataAnnotations;

namespace SonrisasBackendv01.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El estado de la cita es obligatorio")]
        public EstadoCita Estado { get; set; }

		[Required]
		[StringLength(500)]
		public string Motivo { get; set; }

		// Relación con Paciente
		public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        // Relación con Odontólogo
        public int OdontologoId { get; set; }
        public Odontologo Odontologo { get; set; }
    }

    public enum EstadoCita
    {
        Pendiente,
        Completada,
        Cancelada
    }
}
