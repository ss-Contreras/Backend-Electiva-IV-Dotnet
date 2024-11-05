namespace SonrisasBackendv01.Models.Dtos
{
	public class HistorialDto
	{
		public int Id { get; set; }
		public DateTime Fecha { get; set; }
		public string Descripcion { get; set; }
		public int PacienteId { get; set; }
		public string NombrePaciente { get; set; }
		public int OdontologoId { get; set; }
		public string NombreOdontologo { get; set; }
	}
}
