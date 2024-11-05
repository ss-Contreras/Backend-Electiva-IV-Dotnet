using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
    public class PacienteRepositorio : IPacienteRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PacienteRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

		public async Task<IEnumerable<Paciente>> ObtenerTodosAsync()
		{
			var pacientes = await _context.Pacientes
				.Include(p => p.Odontologo)
				.ToListAsync();

			if (pacientes == null || !pacientes.Any())
			{
				throw new Exception("No se encontraron pacientes en la base de datos.");
			}
			return pacientes;
		}


		public async Task<Paciente> ObtenerPorIdAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentException("El ID proporcionado no es válido.");
			}

			var paciente = await _context.Pacientes
				.Include(p => p.Odontologo)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (paciente == null)
			{
				throw new Exception($"No se encontró un paciente con el ID {id}.");
			}

			return paciente;
		}


		// Crear un nuevo paciente
		public async Task<bool> CrearAsync(Paciente paciente)
        {
            await _context.Pacientes.AddAsync(paciente);
            return await _context.SaveChangesAsync() > 0;
        }

        // Actualizar un paciente
        public async Task ActualizarAsync(Paciente paciente)
        {
            if (paciente == null)
            {
                throw new ArgumentNullException(nameof(paciente), "El objeto paciente no puede ser nulo.");
            }

            var pacienteExistente = await _context.Pacientes.FindAsync(paciente.Id);
            if (pacienteExistente == null)
            {
                throw new Exception($"No se puede actualizar. No se encontró un paciente con el ID {paciente.Id}.");
            }

            if (await _context.Pacientes.AnyAsync(p => p.Cedula == paciente.Cedula && p.Id != paciente.Id))
            {
                throw new Exception("Otro paciente con la misma cédula ya existe.");
            }

            _context.Entry(pacienteExistente).CurrentValues.SetValues(paciente);
            await _context.SaveChangesAsync();
        }

        // Eliminar un paciente por ID
        public async Task EliminarAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID proporcionado no es válido.");
            }

            var paciente = await ObtenerPorIdAsync(id);
            if (paciente == null)
            {
                throw new Exception($"No se puede eliminar. No se encontró un paciente con el ID {id}.");
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
        }

        // Métricas y consultas adicionales

        // Contar el número total de pacientes
        public async Task<int> ContarPacientesAsync()
        {
            var totalPacientes = await _context.Pacientes.CountAsync();
            return totalPacientes;
        }

        // Contar cuántos pacientes son nuevos
        public async Task<int> ContarPacientesNuevosAsync()
        {
            var totalPacientesNuevos = await _context.Pacientes
                .CountAsync(p => p.NuevoPaciente == OpcionBinaria.Si);
            return totalPacientesNuevos;
        }

        // Contar cuántos pacientes fueron recomendados
        public async Task<int> ContarPacientesRecomendadosAsync()
        {
            var totalPacientesRecomendados = await _context.Pacientes
                .CountAsync(p => p.PacienteRecomendado == OpcionBinaria.Si);
            return totalPacientesRecomendados;
        }

        // Obtener pacientes por edad
        public async Task<IEnumerable<Paciente>> ObtenerPacientesPorEdadAsync(int edad)
        {
            if (edad <= 0)
            {
                throw new ArgumentException("La edad proporcionada no es válida.");
            }

            var pacientesPorEdad = await _context.Pacientes
                .Where(p => p.Edad == edad)
                .ToListAsync();

            if (pacientesPorEdad == null || !pacientesPorEdad.Any())
            {
                throw new Exception($"No se encontraron pacientes con la edad de {edad} años.");
            }

            return pacientesPorEdad;
        }

        // Buscar pacientes por nombre
        public async Task<IEnumerable<Paciente>> BuscarPacientesPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre proporcionado no puede ser nulo o vacío.");
            }

            var pacientesPorNombre = await _context.Pacientes
                .Where(p => p.Nombre.Contains(nombre))
                .ToListAsync();

            if (pacientesPorNombre == null || !pacientesPorNombre.Any())
            {
                throw new Exception($"No se encontraron pacientes con el nombre o parte del nombre '{nombre}'.");
            }

            return pacientesPorNombre;
        }

        public async Task<bool> ExistePacientePorCedula(string cedula)
        {
            return await _context.Pacientes.AnyAsync(p => p.Cedula == cedula);
        }
    }
}
