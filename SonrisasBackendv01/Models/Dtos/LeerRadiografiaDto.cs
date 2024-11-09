namespace SonrisasBackendv01.Dtos
{
	public class LeerRadiografiaDto
	{
		public int Id { get; set; }
		public string ImageUrl { get; set; }
		public string Descripcion { get; set; }
		public DateTime Fecha { get; set; }
		public int PacienteId { get; set; }
		public string NombrePaciente { get; set; }
		public string CorreoElectronicoPaciente { get; set; }
	}
}
