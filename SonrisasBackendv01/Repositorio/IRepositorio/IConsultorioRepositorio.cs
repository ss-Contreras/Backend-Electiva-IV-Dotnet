using SonrisasBackendv01.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Repositorios
{
    public interface IConsultorioRepositorio
    {
        // Obtener todos los consultorios
        Task<IEnumerable<Consultorio>> ObtenerTodosAsync();

        // Obtener un consultorio por su ID
        Task<Consultorio> ObtenerPorIdAsync(int id);

        // Verificar si existe un consultorio por su nombre
        Task<bool> ExisteConsultorioPorNombre(string nombre);

        // Verificar si existe un consultorio por su ID
        Task<bool> ExisteConsultorioPorId(int id);

        // Crear un nuevo consultorio
        Task<bool> CrearAsync(Consultorio consultorio);

        // Actualizar un consultorio existente
        Task<bool> ActualizarAsync(Consultorio consultorio);

        // Eliminar un consultorio por su ID
        Task<bool> EliminarAsync(int id);
    }
}
