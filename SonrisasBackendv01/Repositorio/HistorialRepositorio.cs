using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public class HistorialRepositorio : IHistorialRepositorio
	{
		private readonly ApplicationDbContext _context;

		public HistorialRepositorio(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Historial>> ObtenerTodosAsync()
		{
			return await _context.Historiales
								 .Include(h => h.Paciente)
								 .Include(h => h.Odontologo)
								 .ToListAsync();
		}

		public async Task<Historial> ObtenerPorIdAsync(int id)
		{
			return await _context.Historiales
								 .Include(h => h.Paciente)
								 .Include(h => h.Odontologo)
								 .FirstOrDefaultAsync(h => h.Id == id);
		}

		public async Task<Historial> CrearAsync(Historial historial)
		{
			await _context.Historiales.AddAsync(historial);
			await _context.SaveChangesAsync();

			// Cargar las entidades relacionadas
			return await _context.Historiales
								 .Include(h => h.Paciente)
								 .Include(h => h.Odontologo)
								 .FirstOrDefaultAsync(h => h.Id == historial.Id);
		}

		public async Task<bool> ActualizarAsync(Historial historial)
		{
			_context.Historiales.Update(historial);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> EliminarAsync(int id)
		{
			var historial = await _context.Historiales.FindAsync(id);
			if (historial == null)
			{
				return false;
			}

			_context.Historiales.Remove(historial);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}