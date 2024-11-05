using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Repositorios
{
    public class ConsultorioRepositorio : IConsultorioRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ConsultorioRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los consultorios
        public async Task<IEnumerable<Consultorio>> ObtenerTodosAsync()
        {
            return await _context.Consultorios
                                 .Include(c => c.Odontologos)  // Cargar los odontólogos asociados
                                 .ToListAsync();
        }

        // Obtener un consultorio por su ID
        public async Task<Consultorio> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID proporcionado no es válido.");
            }

            var consultorio = await _context.Consultorios
                                            .Include(c => c.Odontologos)  // Asegúrate de cargar los odontólogos relacionados
                                            .FirstOrDefaultAsync(c => c.Id == id);

            if (consultorio == null)
            {
                throw new KeyNotFoundException($"No se encontró un consultorio con el ID {id}.");
            }

            return consultorio;
        }


        // Verificar si existe un consultorio por su nombre
        public async Task<bool> ExisteConsultorioPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre proporcionado no es válido.");
            }

            return await _context.Consultorios.AnyAsync(c => c.Nombre == nombre);
        }

        // Verificar si existe un consultorio por su ID
        public async Task<bool> ExisteConsultorioPorId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID proporcionado no es válido.");
            }

            return await _context.Consultorios.AnyAsync(c => c.Id == id);
        }

        // Crear un nuevo consultorio
        public async Task<bool> CrearAsync(Consultorio consultorio)
        {
            if (consultorio == null)
            {
                throw new ArgumentNullException(nameof(consultorio), "El objeto consultorio no puede ser nulo.");
            }

            if (await ExisteConsultorioPorNombre(consultorio.Nombre))
            {
                throw new InvalidOperationException("Ya existe un consultorio con el mismo nombre.");
            }

            await _context.Consultorios.AddAsync(consultorio);
            return await _context.SaveChangesAsync() > 0;
        }

        // Actualizar un consultorio existente
        public async Task<bool> ActualizarAsync(Consultorio consultorio)
        {
            if (consultorio == null)
            {
                throw new ArgumentNullException(nameof(consultorio), "El objeto consultorio no puede ser nulo.");
            }

            var consultorioExistente = await _context.Consultorios
                                                     .Include(c => c.Odontologos)  // Cargar los odontólogos asociados
                                                     .FirstOrDefaultAsync(c => c.Id == consultorio.Id);

            if (consultorioExistente == null)
            {
                throw new KeyNotFoundException($"No se encontró un consultorio con el ID {consultorio.Id} para actualizar.");
            }

            // Verificar si otro consultorio ya tiene el mismo nombre
            if (await _context.Consultorios.AnyAsync(c => c.Nombre == consultorio.Nombre && c.Id != consultorio.Id))
            {
                throw new InvalidOperationException("Otro consultorio ya tiene el mismo nombre.");
            }

            // Actualizar las propiedades del consultorio existente
            consultorioExistente.Nombre = consultorio.Nombre;
            consultorioExistente.Direccion = consultorio.Direccion;
            consultorioExistente.Telefono = consultorio.Telefono;

            // Si se proporcionan odontólogos, actualizar la lista
            if (consultorio.Odontologos != null)
            {
                consultorioExistente.Odontologos = consultorio.Odontologos;
            }

            _context.Consultorios.Update(consultorioExistente);
            return await _context.SaveChangesAsync() > 0;
        }

        // Eliminar un consultorio por su ID
        public async Task<bool> EliminarAsync(int id)
        {
            var consultorio = await ObtenerPorIdAsync(id);
            _context.Consultorios.Remove(consultorio);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
