using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Repositorios
{
    public interface IPacienteRepositorio
    {
        // Operaciones CRUD básicas
        Task<IEnumerable<Paciente>> ObtenerTodosAsync();  // Obtener todos los pacientes
        Task<Paciente> ObtenerPorIdAsync(int id);         // Obtener paciente por ID
        Task<bool> CrearAsync(Paciente paciente);               // Crear un nuevo paciente
        Task ActualizarAsync(Paciente paciente);          // Actualizar un paciente
        Task EliminarAsync(int id);                       // Eliminar un paciente por ID

		// Métricas o parámetros específicos
		Task<bool> ExistePacientePorCedula(string cedula);
        Task<int> ContarPacientesAsync();                 // Contar el número total de pacientes
        Task<int> ContarPacientesNuevosAsync();           // Contar cuántos pacientes son nuevos (NuevoPaciente = Si)
        Task<int> ContarPacientesRecomendadosAsync();     // Contar cuántos pacientes fueron recomendados (PacienteRecomendado = Si)
        Task<IEnumerable<Paciente>> ObtenerPacientesPorEdadAsync(int edad); // Obtener pacientes filtrados por edad
        Task<IEnumerable<Paciente>> BuscarPacientesPorNombreAsync(string nombre); // Búsqueda de pacientes por nombre
    }
}
