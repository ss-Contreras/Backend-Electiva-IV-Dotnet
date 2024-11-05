using System.Collections.Generic;
using System.Threading.Tasks;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public interface IHistorialRepositorio
	{
		Task<IEnumerable<Historial>> ObtenerTodosAsync();
		Task<Historial> ObtenerPorIdAsync(int id);
		Task<Historial> CrearAsync(Historial historial);
		Task<bool> ActualizarAsync(Historial historial);
		Task<bool> EliminarAsync(int id);
	}
}
