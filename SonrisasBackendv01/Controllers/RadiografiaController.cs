using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonrisasBackendv01.Dtos;
using SonrisasBackendv01.Repositorios;
using SonrisasBackendv01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SonrisasBackendv01.Controllers
{
	[Route("api/radiografias")]
	[ApiController]
	public class RadiografiasController : ControllerBase
	{
		private readonly IRadiografiaRepositorio _radiografiaRepo;
		private readonly IPacienteRepositorio _pacienteRepo;
		private readonly IMapper _mapper;

		public RadiografiasController(IRadiografiaRepositorio radiografiaRepo, IPacienteRepositorio pacienteRepo, IMapper mapper)
		{
			_radiografiaRepo = radiografiaRepo;
			_pacienteRepo = pacienteRepo;
			_mapper = mapper;
		}

		// GET: api/radiografias
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRadiografias()
		{
			try
			{
				// Puedes decidir si este endpoint devuelve todas las radiografías o las recientes por defecto
				var listaRadiografias = await _radiografiaRepo.ObtenerRadiografiasRecientesAsync();
				var listaRadiografiasDto = _mapper.Map<IEnumerable<LeerRadiografiaDto>>(listaRadiografias);
				return Ok(listaRadiografiasDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar las radiografías: {ex.Message}");
			}
		}

		// GET: api/radiografias/{id}
		[HttpGet("{id:int}", Name = "GetRadiografia")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRadiografia(int id)
		{
			try
			{
				var radiografia = await _radiografiaRepo.ObtenerRadiografiaPorIdAsync(id);
				if (radiografia == null)
				{
					return NotFound($"No se encontró una radiografía con el ID {id}.");
				}
				var radiografiaDto = _mapper.Map<LeerRadiografiaDto>(radiografia);
				return Ok(radiografiaDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar la radiografía: {ex.Message}");
			}
		}

		// GET: api/radiografias/recientes?cantidad=10
		[HttpGet("recientes")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRadiografiasRecientes([FromQuery] int cantidad = 10)
		{
			try
			{
				var radiografiasRecientes = await _radiografiaRepo.ObtenerRadiografiasRecientesAsync(cantidad);
				var radiografiasDto = _mapper.Map<IEnumerable<LeerRadiografiaDto>>(radiografiasRecientes);
				return Ok(radiografiasDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al recuperar las radiografías recientes: {ex.Message}");
			}
		}

		// GET: api/radiografias/buscar?termino=...
		[HttpGet("buscar")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> BuscarRadiografias([FromQuery] string termino)
		{
			try
			{
				var radiografias = await _radiografiaRepo.BuscarRadiografiasAsync(termino);
				var radiografiasDto = _mapper.Map<IEnumerable<LeerRadiografiaDto>>(radiografias);
				return Ok(radiografiasDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al buscar radiografías: {ex.Message}");
			}
		}

		// POST: api/radiografias
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LeerRadiografiaDto))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CrearRadiografia([FromForm] CrearRadiografiaDto crearRadiografiaDto)
		{
			if (crearRadiografiaDto == null)
			{
				return BadRequest("Los datos de la radiografía son necesarios.");
			}

			// Verificar si el paciente existe
			var pacienteExiste = await _pacienteRepo.ObtenerPorIdAsync(crearRadiografiaDto.PacienteId);
			if (pacienteExiste == null)
			{
				ModelState.AddModelError("PacienteId", $"No existe un paciente con el ID {crearRadiografiaDto.PacienteId}.");
				return BadRequest(ModelState);
			}

			// Manejar la imagen si se ha enviado una
			string imageUrl = null;
			if (crearRadiografiaDto.Imagen != null)
			{
				// Validar el tipo y tamaño de la imagen
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var extension = Path.GetExtension(crearRadiografiaDto.Imagen.FileName).ToLower();

				if (!Array.Exists(allowedExtensions, e => e == extension))
				{
					ModelState.AddModelError("Imagen", "Solo se permiten imágenes en formato .jpg, .jpeg o .png.");
					return BadRequest(ModelState);
				}

				if (crearRadiografiaDto.Imagen.Length > 5 * 1024 * 1024)
				{
					ModelState.AddModelError("Imagen", "La imagen no puede exceder los 5MB.");
					return BadRequest(ModelState);
				}

				string uniqueFileName = Guid.NewGuid().ToString() + extension;
				string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "radiografias");

				// Crear el directorio si no existe
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await crearRadiografiaDto.Imagen.CopyToAsync(fileStream);
				}

				imageUrl = $"{Request.Scheme}://{Request.Host}/images/radiografias/{uniqueFileName}";
			}

			var radiografia = _mapper.Map<Radiografia>(crearRadiografiaDto);
			radiografia.ImageUrl = imageUrl;

			try
			{
				var radiografiaCreada = await _radiografiaRepo.CrearRadiografiaAsync(radiografia);

				// Obtener la radiografia con el Paciente incluido
				var radiografiaConPaciente = await _radiografiaRepo.ObtenerRadiografiaPorIdAsync(radiografiaCreada.Id);

				var radiografiaDto = _mapper.Map<LeerRadiografiaDto>(radiografiaConPaciente);

				return CreatedAtRoute("GetRadiografia", new { id = radiografiaDto.Id }, radiografiaDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al crear la radiografía: {ex.Message}");
			}
		}

		// PUT: api/radiografias/{id}
		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ActualizarRadiografia(int id, [FromForm] ActualizarRadiografiaDto actualizarRadiografiaDto)
		{
			if (actualizarRadiografiaDto == null)
			{
				return BadRequest("Los datos para actualizar la radiografía son necesarios.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var radiografiaExiste = await _radiografiaRepo.ObtenerRadiografiaPorIdAsync(id);
			if (radiografiaExiste == null)
			{
				return NotFound($"No se encontró una radiografía con el ID {id}.");
			}

			// Verificar si el paciente existe si se está actualizando
			if (actualizarRadiografiaDto.PacienteId.HasValue)
			{
				var pacienteExiste = await _pacienteRepo.ObtenerPorIdAsync(actualizarRadiografiaDto.PacienteId.Value);
				if (pacienteExiste == null)
				{
					ModelState.AddModelError("PacienteId", $"No existe un paciente con el ID {actualizarRadiografiaDto.PacienteId}.");
					return BadRequest(ModelState);
				}
			}

			// Manejar la imagen si se ha enviado una
			if (actualizarRadiografiaDto.Imagen != null)
			{
				// Validar el tipo y tamaño de la imagen
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var extension = Path.GetExtension(actualizarRadiografiaDto.Imagen.FileName).ToLower();

				if (!Array.Exists(allowedExtensions, e => e == extension))
				{
					ModelState.AddModelError("Imagen", "Solo se permiten imágenes en formato .jpg, .jpeg o .png.");
					return BadRequest(ModelState);
				}

				if (actualizarRadiografiaDto.Imagen.Length > 5 * 1024 * 1024)
				{
					ModelState.AddModelError("Imagen", "La imagen no puede exceder los 5MB.");
					return BadRequest(ModelState);
				}

				string uniqueFileName = Guid.NewGuid().ToString() + extension;
				string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "radiografias");

				// Crear el directorio si no existe
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await actualizarRadiografiaDto.Imagen.CopyToAsync(fileStream);
				}

				radiografiaExiste.ImageUrl = $"{Request.Scheme}://{Request.Host}/images/radiografias/{uniqueFileName}";
			}

			// Mapear otros campos
			radiografiaExiste.Descripcion = actualizarRadiografiaDto.Descripcion;
			radiografiaExiste.Fecha = actualizarRadiografiaDto.Fecha;
			if (actualizarRadiografiaDto.PacienteId.HasValue)
			{
				radiografiaExiste.PacienteId = actualizarRadiografiaDto.PacienteId.Value;
			}

			try
			{
				bool resultado = await _radiografiaRepo.ActualizarRadiografiaAsync(radiografiaExiste);

				if (!resultado)
				{
					return StatusCode(500, "Error al actualizar la radiografía.");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al actualizar la radiografía: {ex.Message}");
			}
		}

		// DELETE: api/radiografias/{id}
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> EliminarRadiografia(int id)
		{
			var radiografia = await _radiografiaRepo.ObtenerRadiografiaPorIdAsync(id);
			if (radiografia == null)
			{
				return NotFound($"No se encontró una radiografía con el ID {id}.");
			}

			try
			{
				bool resultado = await _radiografiaRepo.EliminarRadiografiaAsync(id);
				if (!resultado)
				{
					return StatusCode(500, "Error al eliminar la radiografía.");
				}

				// Opcional: Eliminar la imagen del servidor si es necesario
				if (!string.IsNullOrEmpty(radiografia.ImageUrl))
				{
					string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "radiografias", Path.GetFileName(radiografia.ImageUrl));
					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al eliminar la radiografía: {ex.Message}");
			}
		}

		// POST: api/radiografias/buscar
		// Opcional: Si deseas implementar una ruta de búsqueda con método POST
		/*
        [HttpPost("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuscarRadiografias([FromBody] string termino)
        {
            try
            {
                var radiografias = await _radiografiaRepo.BuscarRadiografiasAsync(termino);
                var radiografiasDto = _mapper.Map<IEnumerable<LeerRadiografiaDto>>(radiografias);
                return Ok(radiografiasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar radiografías: {ex.Message}");
            }
        }
        */
	}
}
