// SonrisasBackendv01.Repositorios.CitaRepositorio.cs 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
	public class CitaRepositorio : ICitaRepositorio
	{
		private readonly ApplicationDbContext _context;

		public CitaRepositorio(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Cita>> ObtenerTodosAsync(string search)
		{
			var query = _context.Citas
				.Include(c => c.Paciente)
				.Include(c => c.Odontologo)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(c =>
					c.Paciente.Nombre.Contains(search) ||
					c.Id.ToString().Contains(search) ||
					c.Estado.ToString().Contains(search)); // Convertir Estado a string
			}

			var citas = await query.ToListAsync();

			if (citas == null || !citas.Any())
			{
				throw new Exception("No se encontraron citas en la base de datos.");
			}

			return citas;
		}

		public async Task<Cita> ObtenerPorIdAsync(int id)
		{
			return await _context.Citas
								 .Include(c => c.Paciente)
								 .Include(c => c.Odontologo)
								 .AsNoTracking()
								 .FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Cita> CrearAsync(Cita cita)
		{
			// Validaciones...

			// Verificar si el Paciente existe
			var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == cita.PacienteId);
			if (!pacienteExiste)
			{
				throw new ArgumentException($"No existe un paciente con el ID {cita.PacienteId}.");
			}

			// Verificar si el Odontólogo existe
			var odontologoExiste = await _context.Odontologos.AnyAsync(o => o.Id == cita.OdontologoId);
			if (!odontologoExiste)
			{
				throw new ArgumentException($"No existe un odontólogo con el ID {cita.OdontologoId}.");
			}

			// Verificar que la Fecha es futura
			if (cita.Fecha <= DateTime.Now)
			{
				throw new ArgumentException("La fecha de la cita debe ser una fecha futura.");
			}

			// Verificar que el Odontólogo no tenga otra cita en el mismo horario
			var conflicto = await _context.Citas
										  .AnyAsync(c => c.OdontologoId == cita.OdontologoId
													  && c.Fecha.Date == cita.Fecha.Date
													  && c.Fecha.TimeOfDay == cita.Fecha.TimeOfDay);
			if (conflicto)
			{
				throw new InvalidOperationException("El odontólogo ya tiene una cita programada en esta fecha y hora.");
			}

			// Agregar la Cita
			await _context.Citas.AddAsync(cita);
			await _context.SaveChangesAsync();

			// Cargar las entidades relacionadas
			return await _context.Citas
								 .Include(c => c.Paciente)
								 .Include(c => c.Odontologo)
								 .FirstOrDefaultAsync(c => c.Id == cita.Id);
		}

		public async Task<bool> ActualizarAsync(Cita cita)
		{
			// Validaciones...

			// Verificar si la Cita existe
			var citaExistente = await _context.Citas.FindAsync(cita.Id);
			if (citaExistente == null)
			{
				return false;
			}

			// Verificar si el Paciente existe
			var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == cita.PacienteId);
			if (!pacienteExiste)
			{
				throw new ArgumentException($"No existe un paciente con el ID {cita.PacienteId}.");
			}

			// Verificar si el Odontólogo existe
			var odontologoExiste = await _context.Odontologos.AnyAsync(o => o.Id == cita.OdontologoId);
			if (!odontologoExiste)
			{
				throw new ArgumentException($"No existe un odontólogo con el ID {cita.OdontologoId}.");
			}

			// Verificar que la Fecha es futura
			if (cita.Fecha <= DateTime.Now)
			{
				throw new ArgumentException("La fecha de la cita debe ser una fecha futura.");
			}

			// Verificar que el Odontólogo no tenga otra cita en el mismo horario
			var conflicto = await _context.Citas
										  .AnyAsync(c => c.OdontologoId == cita.OdontologoId
													  && c.Id != cita.Id
													  && c.Fecha.Date == cita.Fecha.Date
													  && c.Fecha.TimeOfDay == cita.Fecha.TimeOfDay);
			if (conflicto)
			{
				throw new InvalidOperationException("El odontólogo ya tiene una cita programada en esta fecha y hora.");
			}

			// Actualizar la Cita
			citaExistente.Fecha = cita.Fecha;
			citaExistente.Estado = cita.Estado;
			citaExistente.Motivo = cita.Motivo; // Asegurarse de que 'Motivo' ahora existe
			citaExistente.PacienteId = cita.PacienteId;
			citaExistente.OdontologoId = cita.OdontologoId;

			_context.Citas.Update(citaExistente);
			var resultado = await _context.SaveChangesAsync();

			return resultado > 0;
		}

		public async Task<bool> EliminarAsync(int id)
		{
			var cita = await _context.Citas.FindAsync(id);
			if (cita == null)
			{
				return false;
			}

			_context.Citas.Remove(cita);
			var resultado = await _context.SaveChangesAsync();

			return resultado > 0;
		}
	}
}
