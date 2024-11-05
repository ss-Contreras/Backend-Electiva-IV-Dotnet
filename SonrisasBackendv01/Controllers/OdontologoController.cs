using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Models.Dtos;
using SonrisasBackendv01.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Controllers
{
    [Route("api/odontologo")]
    [ApiController]
    public class OdontologoController : ControllerBase
    {
        private readonly IOdontologoRepositorio _odontologoRepo;
        private readonly IMapper _mapper;

        public OdontologoController(IOdontologoRepositorio odontologoRepo, IMapper mapper)
        {
            _odontologoRepo = odontologoRepo;
            _mapper = mapper;
        }

        // GET: api/Odontologo
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<OdontologoDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerOdontologos()
        {
            try
            {
                var odontologos = await _odontologoRepo.ObtenerTodosAsync();
                var odontologosDto = _mapper.Map<List<OdontologoDto>>(odontologos);

                return Ok(odontologosDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No se encontraron odontólogos en la base de datos.");
            }
        }

        // GET: api/Odontologo/{id}
        [HttpGet("{id:int}", Name = "GetOdontologo")]
        [ProducesResponseType(200, Type = typeof(OdontologoDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObtenerOdontologo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID proporcionado no es válido.");
            }

            try
            {
                var odontologo = await _odontologoRepo.ObtenerPorIdAsync(id);
                var odontologoDto = _mapper.Map<OdontologoDto>(odontologo);

                return Ok(odontologoDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No se encontró un odontólogo con el ID {id}.");
            }
        }

        // POST: api/Odontologo
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(OdontologoDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CrearOdontologo([FromBody] CrearOdontologoDto crearOdontologoDto)
        {
            if (crearOdontologoDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verificar si ya existe un odontólogo con el mismo número de licencia
            if (await _odontologoRepo.ExisteOdontologoPorLicencia(crearOdontologoDto.NumeroLicencia))
            {
                ModelState.AddModelError("", "Ya existe un odontólogo con el mismo número de licencia.");
                return StatusCode(409, ModelState);
            }

            var odontologo = _mapper.Map<Odontologo>(crearOdontologoDto);

            if (!await _odontologoRepo.CrearAsync(odontologo))
            {
                ModelState.AddModelError("", $"Algo salió mal al guardar el odontólogo {odontologo.Nombre}");
                return StatusCode(500, ModelState);
            }

            var odontologoDto = _mapper.Map<OdontologoDto>(odontologo);

            return CreatedAtRoute("GetOdontologo", new { id = odontologoDto.Id }, odontologoDto);
        }

        // PUT: api/Odontologo/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]  // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ActualizarOdontologo(int id, [FromBody] CrearOdontologoDto actualizarOdontologoDto)
        {
            if (actualizarOdontologoDto == null || id != actualizarOdontologoDto.ConsultorioId)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el odontólogo existe
            if (!await _odontologoRepo.ExisteOdontologoPorId(id))
            {
                return NotFound($"No se encontró un odontólogo con el ID {id}.");
            }

            // Verificar si otro odontólogo ya tiene el mismo número de licencia
            if (await _odontologoRepo.ExisteOdontologoPorLicencia(actualizarOdontologoDto.NumeroLicencia) && id != actualizarOdontologoDto.ConsultorioId)
            {
                ModelState.AddModelError("", "Otro odontólogo ya tiene el mismo número de licencia.");
                return StatusCode(409, ModelState);
            }

            var odontologo = _mapper.Map<Odontologo>(actualizarOdontologoDto);
            odontologo.Id = id;  // Asegurar que el ID del odontólogo sea el correcto

            if (!await _odontologoRepo.ActualizarAsync(odontologo))
            {
                ModelState.AddModelError("", $"Algo salió mal al actualizar el odontólogo {odontologo.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE: api/Odontologo/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]  // No Content
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EliminarOdontologo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID proporcionado no es válido.");
            }

            try
            {
                if (!await _odontologoRepo.ExisteOdontologoPorId(id))
                {
                    return NotFound($"No se encontró un odontólogo con el ID {id}.");
                }

                if (!await _odontologoRepo.EliminarAsync(id))
                {
                    ModelState.AddModelError("", $"Algo salió mal al intentar eliminar el odontólogo con ID {id}");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }
    }
}
