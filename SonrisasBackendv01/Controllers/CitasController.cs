using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Dtos;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Controllers
{
	[Route("api/citas")]
	[ApiController]
	public class CitasController : ControllerBase
	{
		private readonly ICitaRepositorio _citasRepo;
		private readonly IMapper _mapper;

		public CitasController(ICitaRepositorio citasRepo, IMapper mapper)
		{
			_citasRepo = citasRepo;
			_mapper = mapper;
		}

		// GET: api/citas
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetCitas([FromQuery] string search = "")
		{
			try
			{
				var citas = await _citasRepo.ObtenerTodosAsync(search);
				var citasDto = _mapper.Map<IEnumerable<LeerCitaDto>>(citas);
				return Ok(citasDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar las citas: {ex.Message}");
			}
		}

		// GET: api/citas/{id}
		[HttpGet("{id:int}", Name = "GetCita")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetCita(int id)
		{
			try
			{
				var cita = await _citasRepo.ObtenerPorIdAsync(id);
				if (cita == null)
				{
					return NotFound($"No se encontró una cita con el ID {id}.");
				}
				var citaDto = _mapper.Map<LeerCitaDto>(cita);
				return Ok(citaDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar la cita: {ex.Message}");
			}
		}

		// POST: api/citas
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LeerCitaDto))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CrearCita([FromBody] CrearCitaDto crearCitaDto)
		{
			if (crearCitaDto == null)
			{
				return BadRequest("Los datos de la cita son necesarios.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var cita = _mapper.Map<Cita>(crearCitaDto);

				// Crear la cita
				var citaCreada = await _citasRepo.CrearAsync(cita);

				// Mapear a DTO
				var citaDto = _mapper.Map<LeerCitaDto>(citaCreada);

				return CreatedAtRoute("GetCita", new { id = citaDto.Id }, citaDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al crear la cita: {ex.Message}");
			}
		}

		// PUT: api/citas/{id}
		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ActualizarCita(int id, [FromBody] ActualizarCitaDto actualizarCitaDto)
		{
			if (actualizarCitaDto == null || id != actualizarCitaDto.Id)
			{
				return BadRequest("Datos de la cita inválidos.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var cita = _mapper.Map<Cita>(actualizarCitaDto);
				var resultado = await _citasRepo.ActualizarAsync(cita);

				if (!resultado)
				{
					return NotFound($"No se encontró una cita con el ID {id}.");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al actualizar la cita: {ex.Message}");
			}
		}

		// DELETE: api/citas/{id}
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> EliminarCita(int id)
		{
			try
			{
				var resultado = await _citasRepo.EliminarAsync(id);
				if (!resultado)
				{
					return NotFound($"No se encontró una cita con el ID {id}.");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al eliminar la cita: {ex.Message}");
			}
		}
	}
}
