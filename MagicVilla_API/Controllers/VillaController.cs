﻿using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VillaController> _logger;
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext context)
        {
            _logger = logger;   
            _db = context; 
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            return Ok(_db.Villas.ToList());
        }



        [HttpGet("id:int", Name ="GetVilla")]

        //hay que declarar el tipo de respuesta que va a devolver por buenas practicas
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer la villa con id: " + id);
                return BadRequest("Peticion Mala");
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound("No encontrado");
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {
           if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExite", "La villa con ese Nombre ya existe");
                return BadRequest(ModelState);
            }

            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }

          if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa model = new()
            {
                Nombre= villaDto.Nombre,
                Detalle= villaDto.Detalle,
                ImagenUrl= villaDto.ImagenUrl,
                Ocupantes= villaDto.Ocupantes,
                Tarifa= villaDto.Tarifa,
                MetrosCuadrados =villaDto.MetrosCuadrados,
                Amenidad= villaDto.Amenidad,
            }; 

            _db.Villas.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new {id = villaDto.Id },villaDto);    
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id) 
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            _db.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto) 
        {
            if (villaDto == null || id!= villaDto.Id)
            {
                return BadRequest();
            }

            Villa Modelo = new()
            {
                Id= villaDto.Id,
                Nombre= villaDto.Nombre,
                Detalle= villaDto.Detalle,  
                ImagenUrl= villaDto.ImagenUrl,
                Ocupantes= villaDto.Ocupantes,
                Tarifa  = villaDto.Tarifa,
                MetrosCuadrados=villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };

            _db.Villas.Update(Modelo);
            _db.SaveChanges();
            return NoContent();
        }





        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDto>patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

           var villa = _db.Villas.AsNoTracking().FirstOrDefault(V => V.Id == id);
            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle, 
                ImagenUrl= villa.ImagenUrl,
                Ocupantes= villa.Ocupantes,
                Tarifa=villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad= villa.Amenidad,    
            };

            if (villa == null)
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Villa modelo = new() 
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };

            _db.Villas.Update(modelo);
            _db.SaveChanges();
            return NoContent();
        }



    }
}
