using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public class RadiografiaRepositorio : IRadiografiaRepositorio
	{
		private readonly ApplicationDbContext _context;

		public RadiografiaRepositorio(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Radiografia> CrearRadiografiaAsync(Radiografia radiografia)
		{
			if (radiografia == null)
				throw new ArgumentNullException(nameof(radiografia));

			await _context.Radiografias.AddAsync(radiografia);
			await _context.SaveChangesAsync();
			return radiografia;
		}

		public async Task<bool> ActualizarRadiografiaAsync(Radiografia radiografia)
		{
			if (radiografia == null)
				throw new ArgumentNullException(nameof(radiografia));

			_context.Radiografias.Update(radiografia);
			try
			{
				return await _context.SaveChangesAsync() > 0;
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!RadiografiaExiste(radiografia.Id))
					throw new KeyNotFoundException("Radiografía no encontrada.");
				else
					throw;
			}
		}

		public async Task<bool> EliminarRadiografiaAsync(int id)
		{
			var radiografia = await _context.Radiografias.FindAsync(id);
			if (radiografia == null)
				throw new KeyNotFoundException("Radiografía no encontrada.");

			_context.Radiografias.Remove(radiografia);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Radiografia> ObtenerRadiografiaPorIdAsync(int id)
		{
			return await _context.Radiografias
				.Include(r => r.Paciente)
				.FirstOrDefaultAsync(r => r.Id == id);
		}

		public async Task<IEnumerable<Radiografia>> ObtenerRadiografiasRecientesAsync(int cantidad = 10)
		{
			return await _context.Radiografias
				.Include(r => r.Paciente)
				.OrderByDescending(r => r.Fecha)
				.Take(cantidad)
				.ToListAsync();
		}

		public async Task<IEnumerable<Radiografia>> BuscarRadiografiasAsync(string terminoBusqueda)
		{
			if (string.IsNullOrWhiteSpace(terminoBusqueda))
				return await _context.Radiografias.Include(r => r.Paciente).ToListAsync();

			terminoBusqueda = terminoBusqueda.ToLower();

			return await _context.Radiografias
				.Include(r => r.Paciente)
				.Where(r =>
					r.Descripcion.ToLower().Contains(terminoBusqueda) ||
					r.Paciente.Nombre.ToLower().Contains(terminoBusqueda) ||
					r.Paciente.Cedula.ToLower().Contains(terminoBusqueda) ||
					r.Fecha.ToString("yyyy-MM-dd").Contains(terminoBusqueda))
				.ToListAsync();
		}

		private bool RadiografiaExiste(int id)
		{
			return _context.Radiografias.Any(r => r.Id == id);
		}
	}
}
