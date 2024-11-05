using SonrisasBackendv01.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Repositorios
{
    public interface IOdontologoRepositorio
    {
        // Obtener todos los odontólogos
        Task<IEnumerable<Odontologo>> ObtenerTodosAsync();

        // Obtener un odontólogo por su ID
        Task<Odontologo> ObtenerPorIdAsync(int id);

        // Verificar si existe un odontólogo por su número de licencia
        Task<bool> ExisteOdontologoPorLicencia(string numeroLicencia);

        // Verificar si existe un odontólogo por su ID
        Task<bool> ExisteOdontologoPorId(int id);

        // Crear un nuevo odontólogo
        Task<bool> CrearAsync(Odontologo odontologo);

        // Actualizar un odontólogo existente
        Task<bool> ActualizarAsync(Odontologo odontologo);

        // Eliminar un odontólogo por su ID
        Task<bool> EliminarAsync(int id);
    }
}
