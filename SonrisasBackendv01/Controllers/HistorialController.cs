using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Models.Dtos;
using SonrisasBackendv01.Repositorios;

namespace SonrisasBackendv01.Controllers
{
	[Route("api/historial")]
	[ApiController]
	public class HistorialController : ControllerBase
	{
		private readonly IHistorialRepositorio _historialRepositorio;
		private readonly IPacienteRepositorio _pacienteRepositorio;
		private readonly IOdontologoRepositorio _odontologoRepositorio;
		private readonly IMapper _mapper;

		public HistorialController(
			IHistorialRepositorio historialRepositorio,
			IPacienteRepositorio pacienteRepositorio,
			IOdontologoRepositorio odontologoRepositorio,
			IMapper mapper)
		{
			_historialRepositorio = historialRepositorio;
			_pacienteRepositorio = pacienteRepositorio;
			_odontologoRepositorio = odontologoRepositorio;
			_mapper = mapper;
		}

		/// <summary>
		/// Obtiene todos los historiales.
		/// </summary>
		/// <returns>Lista de historiales.</returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetHistoriales()
		{
			try
			{
				var historiales = await _historialRepositorio.ObtenerTodosAsync();
				var historialesDto = _mapper.Map<List<HistorialDto>>(historiales);
				return Ok(historialesDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar los historiales: {ex.Message}");
			}
		}

		/// <summary>
		/// Obtiene un historial por su ID.
		/// </summary>
		/// <param name="id">ID del historial.</param>
		/// <returns>Historial encontrado.</returns>
		[HttpGet("{id:int}", Name = "GetHistorial")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetHistorial(int id)
		{
			try
			{
				var historial = await _historialRepositorio.ObtenerPorIdAsync(id);
				if (historial == null)
				{
					return NotFound($"No se encontró un historial con el ID {id}.");
				}
				var historialDto = _mapper.Map<HistorialDto>(historial);
				return Ok(historialDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar el historial: {ex.Message}");
			}
		}

		/// <summary>
		/// Crea un nuevo historial.
		/// </summary>
		/// <param name="crearHistorialDto">Datos para crear el historial.</param>
		/// <returns>Historial creado.</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HistorialDto))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CrearHistorial([FromBody] CrearHistorialDto crearHistorialDto)
		{
			if (crearHistorialDto == null)
			{
				return BadRequest("Los datos del historial son necesarios.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Verificar si el paciente existe
			var pacienteExiste = await _pacienteRepositorio.ObtenerPorIdAsync(crearHistorialDto.PacienteId);
			if (pacienteExiste == null)
			{
				ModelState.AddModelError("PacienteId", $"No existe un paciente con el ID {crearHistorialDto.PacienteId}.");
				return BadRequest(ModelState);
			}

			// Verificar si el odontólogo existe
			var odontologoExiste = await _odontologoRepositorio.ObtenerPorIdAsync(crearHistorialDto.OdontologoId);
			if (odontologoExiste == null)
			{
				ModelState.AddModelError("OdontologoId", $"No existe un odontólogo con el ID {crearHistorialDto.OdontologoId}.");
				return BadRequest(ModelState);
			}

			var historial = _mapper.Map<Historial>(crearHistorialDto);

			try
			{
				var historialCreado = await _historialRepositorio.CrearAsync(historial);
				var historialDto = _mapper.Map<HistorialDto>(historialCreado);
				return CreatedAtRoute("GetHistorial", new { id = historialDto.Id }, historialDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al crear el historial: {ex.Message}");
			}
		}

		/// <summary>
		/// Actualiza un historial existente.
		/// </summary>
		/// <param name="id">ID del historial a actualizar.</param>
		/// <param name="actualizarHistorialDto">Datos para actualizar el historial.</param>
		/// <returns>No Content si se actualiza correctamente.</returns>
		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ActualizarHistorial(int id, [FromBody] CrearHistorialDto actualizarHistorialDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var historialExistente = await _historialRepositorio.ObtenerPorIdAsync(id);
				if (historialExistente == null)
				{
					return NotFound($"No se encontró un historial con el ID {id}.");
				}

				// Verificar si el paciente existe
				var pacienteExiste = await _pacienteRepositorio.ObtenerPorIdAsync(actualizarHistorialDto.PacienteId);
				if (pacienteExiste == null)
				{
					ModelState.AddModelError("PacienteId", $"No existe un paciente con el ID {actualizarHistorialDto.PacienteId}.");
					return BadRequest(ModelState);
				}

				// Verificar si el odontólogo existe
				var odontologoExiste = await _odontologoRepositorio.ObtenerPorIdAsync(actualizarHistorialDto.OdontologoId);
				if (odontologoExiste == null)
				{
					ModelState.AddModelError("OdontologoId", $"No existe un odontólogo con el ID {actualizarHistorialDto.OdontologoId}.");
					return BadRequest(ModelState);
				}

				// Mapear los cambios al historial existente
				_mapper.Map(actualizarHistorialDto, historialExistente);

				var resultado = await _historialRepositorio.ActualizarAsync(historialExistente);
				if (!resultado)
				{
					return StatusCode(500, $"Algo salió mal al actualizar el historial con ID {id}.");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al actualizar el historial: {ex.Message}");
			}
		}

		/// <summary>
		/// Elimina un historial por su ID.
		/// </summary>
		/// <param name="id">ID del historial a eliminar.</param>
		/// <returns>No Content si se elimina correctamente.</returns>
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> EliminarHistorial(int id)
		{
			try
			{
				var historialExistente = await _historialRepositorio.ObtenerPorIdAsync(id);
				if (historialExistente == null)
				{
					return NotFound($"No se encontró un historial con el ID {id}.");
				}

				var resultado = await _historialRepositorio.EliminarAsync(id);
				if (!resultado)
				{
					return StatusCode(500, $"Algo salió mal al eliminar el historial con ID {id}.");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al eliminar el historial: {ex.Message}");
			}
		}
	}
}
