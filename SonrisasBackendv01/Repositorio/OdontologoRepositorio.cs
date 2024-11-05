using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Repositorios
{
    public class OdontologoRepositorio : IOdontologoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public OdontologoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los odontólogos
        public async Task<IEnumerable<Odontologo>> ObtenerTodosAsync()
        {
            var odontologos = await _context.Odontologos.ToListAsync();
            if (odontologos == null || !odontologos.Any())
            {
                throw new KeyNotFoundException("No se encontraron odontólogos en la base de datos.");
            }
            return odontologos;
        }

        // Obtener un odontólogo por su ID
        public async Task<Odontologo> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID proporcionado no es válido.");
            }

            var odontologo = await _context.Odontologos.FindAsync(id);
            if (odontologo == null)
            {
                throw new KeyNotFoundException($"No se encontró un odontólogo con el ID {id}.");
            }

            return odontologo;
        }

        // Verificar si existe un odontólogo por su número de licencia
        public async Task<bool> ExisteOdontologoPorLicencia(string numeroLicencia)
        {
            if (string.IsNullOrWhiteSpace(numeroLicencia))
            {
                throw new ArgumentException("El número de licencia proporcionado no es válido.");
            }

            return await _context.Odontologos.AnyAsync(o => o.NumeroLicencia == numeroLicencia);
        }

        // Verificar si existe un odontólogo por su ID
        public async Task<bool> ExisteOdontologoPorId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID proporcionado no es válido.");
            }

            return await _context.Odontologos.AnyAsync(o => o.Id == id);
        }

        // Crear un nuevo odontólogo
        public async Task<bool> CrearAsync(Odontologo odontologo)
        {
            if (odontologo == null)
            {
                throw new ArgumentNullException(nameof(odontologo), "El objeto odontólogo no puede ser nulo.");
            }

            if (await ExisteOdontologoPorLicencia(odontologo.NumeroLicencia))
            {
                throw new InvalidOperationException("Ya existe un odontólogo con el mismo número de licencia.");
            }

            await _context.Odontologos.AddAsync(odontologo);
            return await _context.SaveChangesAsync() > 0;
        }

        // Actualizar un odontólogo existente
        public async Task<bool> ActualizarAsync(Odontologo odontologo)
        {
            if (odontologo == null)
            {
                throw new ArgumentNullException(nameof(odontologo), "El objeto odontólogo no puede ser nulo.");
            }

            var odontologoExistente = await _context.Odontologos.FindAsync(odontologo.Id);
            if (odontologoExistente == null)
            {
                throw new KeyNotFoundException($"No se encontró un odontólogo con el ID {odontologo.Id} para actualizar.");
            }

            // Verificar si el número de licencia ya está en uso por otro odontólogo
            if (await _context.Odontologos.AnyAsync(o => o.NumeroLicencia == odontologo.NumeroLicencia && o.Id != odontologo.Id))
            {
                throw new InvalidOperationException("Otro odontólogo ya tiene el mismo número de licencia.");
            }

            // Actualizar los valores del odontólogo existente
            _context.Entry(odontologoExistente).CurrentValues.SetValues(odontologo);
            return await _context.SaveChangesAsync() > 0;
        }

        // Eliminar un odontólogo por su ID
        public async Task<bool> EliminarAsync(int id)
        {
            var odontologo = await ObtenerPorIdAsync(id);
            if (odontologo == null)
            {
                throw new KeyNotFoundException($"No se encontró un odontólogo con el ID {id} para eliminar.");
            }

            _context.Odontologos.Remove(odontologo);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
