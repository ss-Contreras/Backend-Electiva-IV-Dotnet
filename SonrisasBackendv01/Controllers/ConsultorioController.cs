using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Models.Dtos;
using SonrisasBackendv01.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Controllers
{
    [ApiController]
    [Route("api/consultorio")]
    public class ConsultorioController : ControllerBase
    {
        private readonly IConsultorioRepositorio _consultorioRepositorio;

        public ConsultorioController(IConsultorioRepositorio consultorioRepositorio)
        {
            _consultorioRepositorio = consultorioRepositorio;
        }

        // GET: api/consultorio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultorioDto>>> ObtenerTodos()
        {
            try
            {
                var consultorios = await _consultorioRepositorio.ObtenerTodosAsync();

                var consultorioDtos = consultorios.Select(c => new ConsultorioDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Direccion = c.Direccion,
                    Telefono = c.Telefono,
                    Odontologos = c.Odontologos?.Select(o => new OdontologoDto
                    {
                        Id = o.Id,
                        Nombre = o.Nombre,
                        Apellido = o.Apellido,
                        NumeroLicencia = o.NumeroLicencia,
                        Telefono = o.Telefono,
                        Email = o.Email,
                        ConsultorioId = o.ConsultorioId
                    }).ToList()
                }).ToList();

                return Ok(consultorioDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los consultorios: {ex.Message}");
            }
        }


        // GET: api/consultorio/{id}
        [HttpGet("{id:int}", Name = "ObtenerConsultorio")]
        public async Task<ActionResult<ConsultorioDto>> ObtenerPorId(int id)
        {
            try
            {
                var consultorio = await _consultorioRepositorio.ObtenerPorIdAsync(id);

                var consultorioDto = new ConsultorioDto
                {
                    Id = consultorio.Id,
                    Nombre = consultorio.Nombre,
                    Direccion = consultorio.Direccion,
                    Telefono = consultorio.Telefono,
                    Odontologos = consultorio.Odontologos?.Select(o => new OdontologoDto
                    {
                        Id = o.Id,
                        Nombre = o.Nombre,
                        Apellido = o.Apellido,
                        NumeroLicencia = o.NumeroLicencia,
                        Telefono = o.Telefono,
                        Email = o.Email,
                        ConsultorioId = o.ConsultorioId
                    }).ToList()
                };

                return Ok(consultorioDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el consultorio: {ex.Message}");
            }
        }


        // POST: api/consultorio
        [HttpPost]
        public async Task<ActionResult<ConsultorioDto>> Crear([FromBody] CrearConsultorioDto crearConsultorioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _consultorioRepositorio.ExisteConsultorioPorNombre(crearConsultorioDto.Nombre))
                {
                    return Conflict("Ya existe un consultorio con el mismo nombre.");
                }

                var consultorio = new Consultorio
                {
                    Nombre = crearConsultorioDto.Nombre,
                    Direccion = crearConsultorioDto.Direccion,
                    Telefono = crearConsultorioDto.Telefono
                };

                var creado = await _consultorioRepositorio.CrearAsync(consultorio);

                if (creado)
                {
                    var consultorioDto = new ConsultorioDto
                    {
                        Id = consultorio.Id,
                        Nombre = consultorio.Nombre,
                        Direccion = consultorio.Direccion,
                        Telefono = consultorio.Telefono,
                        Odontologos = new List<OdontologoDto>()
                    };

                    return CreatedAtRoute("ObtenerConsultorio", new { id = consultorio.Id }, consultorioDto);
                }
                else
                {
                    return StatusCode(500, "Error al crear el consultorio.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el consultorio: {ex.Message}");
            }
        }

        // PUT: api/consultorio/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] ConsultorioDto consultorioDto)
        {
            if (id != consultorioDto.Id)
            {
                return BadRequest("El ID del consultorio no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!await _consultorioRepositorio.ExisteConsultorioPorId(id))
                {
                    return NotFound($"No se encontró un consultorio con el ID {id}.");
                }

                // Verificar si otro consultorio ya tiene el mismo nombre
                var existeOtroNombre = await _consultorioRepositorio.ExisteConsultorioPorNombre(consultorioDto.Nombre);
                var consultorioActual = await _consultorioRepositorio.ObtenerPorIdAsync(id);

                if (existeOtroNombre && consultorioActual.Nombre != consultorioDto.Nombre)
                {
                    return Conflict("Otro consultorio ya tiene el mismo nombre.");
                }

                var consultorio = new Consultorio
                {
                    Id = consultorioDto.Id,
                    Nombre = consultorioDto.Nombre,
                    Direccion = consultorioDto.Direccion,
                    Telefono = consultorioDto.Telefono,
                    Odontologos = consultorioDto.Odontologos?.Select(o => new Odontologo
                    {
                        Id = o.Id,
                        Nombre = o.Nombre,
                        Apellido = o.Apellido,
                        // Mapear otras propiedades necesarias
                    }).ToList()
                };

                var actualizado = await _consultorioRepositorio.ActualizarAsync(consultorio);

                if (actualizado)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, "Error al actualizar el consultorio.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el consultorio: {ex.Message}");
            }
        }

        // DELETE: api/consultorio/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                if (!await _consultorioRepositorio.ExisteConsultorioPorId(id))
                {
                    return NotFound($"No se encontró un consultorio con el ID {id}.");
                }

                var eliminado = await _consultorioRepositorio.EliminarAsync(id);

                if (eliminado)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, "Error al eliminar el consultorio.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el consultorio: {ex.Message}");
            }
        }
    }
}
