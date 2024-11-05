using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Dtos;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Models.Dtos;
using SonrisasBackendv01.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Controllers
{
    [Route("api/paciente")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
		private readonly IPacienteRepositorio _pacientesRepo;
		private readonly IOdontologoRepositorio _odontologoRepo;
		private readonly IMapper _mapper;

		public PacientesController(IPacienteRepositorio pacientesRepo, IOdontologoRepositorio odontologoRepo, IMapper mapper)
        {
			_pacientesRepo = pacientesRepo;
			_odontologoRepo = odontologoRepo;
			_mapper = mapper;
		}

        // GET: api/Pacientes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaciente()
        {
            try
            {
                var listaPacientes = await _pacientesRepo.ObtenerTodosAsync();
                var listaPacientesDto = _mapper.Map<List<LeerPacienteDto>>(listaPacientes);
                return Ok(listaPacientesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar los pacientes: {ex.Message}");
            }
        }

        // GET: api/Pacientes/{id}
        [HttpGet("{id:int}", Name = "GetPaciente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaciente(int id)
        {
            try
            {
                var paciente = await _pacientesRepo.ObtenerPorIdAsync(id);
                if (paciente == null)
                {
                    return NotFound($"No se encontró un paciente con el ID {id}.");
                }
                var pacienteDto = _mapper.Map<LeerPacienteDto>(paciente);
                return Ok(pacienteDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar el paciente: {ex.Message}");
            }
        }

		// POST: api/paciente
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LeerPacienteDto))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CrearPaciente([FromForm] CrearPacienteDto crearPacienteDto)
		{
			if (crearPacienteDto == null)
			{
				return BadRequest("Los datos del paciente son necesarios.");
			}

			// Verificar si ya existe un paciente con la misma cédula
			if (await _pacientesRepo.ExistePacientePorCedula(crearPacienteDto.Cedula))
			{
				ModelState.AddModelError("Cedula", "Ya existe un paciente con la misma cédula.");
				return Conflict(ModelState);
			}

			// Verificar si el odontólogo existe
			var odontologoExiste = await _odontologoRepo.ObtenerPorIdAsync(crearPacienteDto.OdontologoId);
			if (odontologoExiste == null)
			{
				ModelState.AddModelError("OdontologoId", $"No existe un odontólogo con el ID {crearPacienteDto.OdontologoId}.");
				return BadRequest(ModelState);
			}

			var paciente = _mapper.Map<Paciente>(crearPacienteDto);

			// Manejar la imagen si se ha enviado una
			if (crearPacienteDto.Imagen != null)
			{
				string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(crearPacienteDto.Imagen.FileName);
				string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagenesPacientes", nombreArchivo);

				// Crear el directorio si no existe
				var directorio = Path.GetDirectoryName(rutaArchivo);
				if (!Directory.Exists(directorio))
				{
					Directory.CreateDirectory(directorio);
				}

				// Guardar la imagen en el sistema de archivos
				using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
				{
					await crearPacienteDto.Imagen.CopyToAsync(fileStream);
				}

				// Asignar las rutas de la imagen al modelo del paciente
				paciente.RutaLocalImagen = rutaArchivo;
				paciente.RutaImagen = $"/ImagenesPacientes/{nombreArchivo}";
			}

			// Crear el paciente
			if (!await _pacientesRepo.CrearAsync(paciente))
			{
				ModelState.AddModelError("", $"Algo salió mal al guardar el paciente {paciente.Nombre}");
				return StatusCode(500, ModelState);
			}

			// Obtener el paciente con el Odontologo incluido
			var pacienteConOdontologo = await _pacientesRepo.ObtenerPorIdAsync(paciente.Id);

			var pacienteDto = _mapper.Map<LeerPacienteDto>(pacienteConOdontologo);

			return CreatedAtRoute("GetPaciente", new { id = paciente.Id }, pacienteDto);
		}

		// PUT: api/Pacientes/{id}
		[HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarPaciente(int id, [FromBody] ActualizarPacienteDto actualizarPacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existePaciente = await _pacientesRepo.ObtenerPorIdAsync(id);
                if (existePaciente == null)
                {
                    return NotFound($"No se encontró un paciente con el ID {id}.");
                }

                var paciente = _mapper.Map<Paciente>(actualizarPacienteDto);
                paciente.Id = id; // Asegurarse de que el Id del paciente sea el correcto

                await _pacientesRepo.ActualizarAsync(paciente);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el paciente: {ex.Message}");
            }
        }

        // DELETE: api/Pacientes/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarPaciente(int id)
        {
            try
            {
                var paciente = await _pacientesRepo.ObtenerPorIdAsync(id);
                if (paciente == null)
                {
                    return NotFound($"No se encontró un paciente con el ID {id}.");
                }

                await _pacientesRepo.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el paciente: {ex.Message}");
            }
        }
    }
}
