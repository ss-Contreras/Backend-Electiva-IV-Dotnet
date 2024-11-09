namespace SonrisasBackendv01.Models
{
	public class Radiografia
	{
		public int Id { get; set; }
		public string ImageUrl { get; set; }
		public string Descripcion { get; set; }
		public DateTime Fecha { get; set; }
		public int PacienteId { get; set; }
		public Paciente Paciente { get; set; }
	}
}