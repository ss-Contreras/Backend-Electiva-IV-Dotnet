using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonrisasBackendv01.Models
{
    public class Historial
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de creación del historial es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La descripción del historial es obligatoria")]
        public string Descripcion { get; set; }

		// Relación con Paciente
		[ForeignKey("Paciente")]
		public int PacienteId { get; set; }
		public Paciente Paciente { get; set; }

		// Relación con Odontologo
		[ForeignKey("Odontologo")]
		public int OdontologoId { get; set; }
		public Odontologo Odontologo { get; set; }
	}
}
