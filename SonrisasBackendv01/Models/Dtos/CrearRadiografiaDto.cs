using Microsoft.AspNetCore.Http;
using System;

namespace SonrisasBackendv01.Dtos
{
	public class CrearRadiografiaDto
	{
		public IFormFile Imagen { get; set; }
		public string Descripcion { get; set; }
		public DateTime Fecha { get; set; }
		public int PacienteId { get; set; }
	}
}