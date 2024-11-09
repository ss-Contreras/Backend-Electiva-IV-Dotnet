using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public interface IRadiografiaRepositorio
	{
		Task<Radiografia> CrearRadiografiaAsync(Radiografia radiografia);
		Task<IEnumerable<Radiografia>> ObtenerRadiografiasRecientesAsync(int cantidad = 10);
		Task<Radiografia> ObtenerRadiografiaPorIdAsync(int id);
		Task<IEnumerable<Radiografia>> BuscarRadiografiasAsync(string terminoBusqueda);
		Task<bool> ActualizarRadiografiaAsync(Radiografia radiografia);
		Task<bool> EliminarRadiografiaAsync(int id);
	}
}
