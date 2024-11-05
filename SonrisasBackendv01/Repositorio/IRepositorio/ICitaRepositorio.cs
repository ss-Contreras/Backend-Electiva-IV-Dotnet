using System.Collections.Generic;
using System.Threading.Tasks;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public interface ICitaRepositorio
	{
		Task<IEnumerable<Cita>> ObtenerTodosAsync(string search);
		Task<Cita> ObtenerPorIdAsync(int id);
		Task<Cita> CrearAsync(Cita cita);
		Task<bool> ActualizarAsync(Cita cita);
		Task<bool> EliminarAsync(int id);
	}
}