using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P01_2022AA650_2022MA653.Models;

namespace P01_2022AA650_2022MA653.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspaciosParqueoController : ControllerBase
    {
        private readonly ClaseContext _context;

        public EspaciosParqueoController(ClaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("EspaciosDisponibles")]
        public IActionResult GetEspaciosDisponibles()
        {
            var espacios = _context.EspaciosParqueo
                .Where(e => e.Estado == "Disponible")
                .Select(e => new
                {
                    e.Id,
                    e.SucursalId, 
                    SucursalNombre = _context.Sucursales
                        .Where(s => s.Id == e.SucursalId)
                        .Select(s => s.Nombre)
                        .FirstOrDefault(),
                    e.Numero,
                    e.Ubicacion,
                    e.CostoPorHora,
                    e.Estado
                })
                .ToList();

            return Ok(espacios);
        }


        // Registrar un nuevo espacio de parqueo por sucursal
        [HttpPost]
        [Route("Espacios/Add")]
        public IActionResult CrearEspacioParqueo([FromBody] EspaciosParqueo espacio)
        {
            if (espacio == null)
            {
                return BadRequest("El espacio de parqueo no puede ser nulo.");
            }

            // Validar campos requeridos
            if (espacio.SucursalId == 0)
            {
                return BadRequest("El campo 'SucursalId' es obligatorio.");
            }
            if (espacio.Numero <= 0)
            {
                return BadRequest("El campo 'Numero' es obligatorio.");
            }
            if (string.IsNullOrEmpty(espacio.Ubicacion))
            {
                return BadRequest("El campo 'Ubicacion' es obligatorio.");
            }
            if (espacio.CostoPorHora <= 0)
            {
                return BadRequest("El campo 'CostoPorHora' debe ser un valor positivo.");
            }

            var sucursal = _context.Sucursales.Find(espacio.SucursalId);
            if (sucursal == null)
            {
                return NotFound("Sucursal no encontrada.");
            }

            espacio.Estado = "Disponible"; // Estado por defecto al crear

            _context.EspaciosParqueo.Add(espacio);

            sucursal.NumeroEspaciosDisponibles += 1;

            _context.SaveChanges();
            return Ok(espacio);
        }

        // Actualizar un espacio de parqueo
        [HttpPut]
        [Route("Espacios/Update/{id}")]
        public IActionResult ActualizarEspacioParqueo(int id, [FromBody] EspaciosParqueo espacioModificado)
        {
            if (espacioModificado == null)
            {
                return BadRequest("El espacio de parqueo no puede ser nulo.");
            }

            // Validar campos requeridos
            if (espacioModificado.SucursalId == 0)
            {
                return BadRequest("El campo 'SucursalId' es obligatorio.");
            }
            if (espacioModificado.Numero <= 0)
            {
                return BadRequest("El campo 'Numero' es obligatorio.");
            }
            if (string.IsNullOrEmpty(espacioModificado.Ubicacion))
            {
                return BadRequest("El campo 'Ubicacion' es obligatorio.");
            }
            if (espacioModificado.CostoPorHora <= 0)
            {
                return BadRequest("El campo 'CostoPorHora' debe ser un valor positivo.");
            }
            if (espacioModificado.Estado != "Disponible" && espacioModificado.Estado != "Ocupado")
            {
                return BadRequest("El estado debe ser 'Disponible' o 'Ocupado'.");
            }

            var espacio = _context.EspaciosParqueo.Find(id);
            if (espacio == null)
            {
                return NotFound("Espacio de parqueo no encontrado.");
            }

            var sucursal = _context.Sucursales.Find(espacioModificado.SucursalId);
            if (sucursal == null)
            {
                return NotFound("Sucursal no encontrada.");
            }

            // Validación de cambio de sucursal
            if (espacio.SucursalId != espacioModificado.SucursalId)
            {
                return BadRequest("No se puede cambiar el SucursalId de un espacio de parqueo.");
            }

            string estadoAnterior = espacio.Estado;
            string estadoNuevo = espacioModificado.Estado;

            espacio.Numero = espacioModificado.Numero;
            espacio.Ubicacion = espacioModificado.Ubicacion;
            espacio.CostoPorHora = espacioModificado.CostoPorHora;
            espacio.Estado = espacioModificado.Estado;

            // Actualizar los espacios disponibles de la sucursal si cambia el estado
            if (estadoNuevo == "Ocupado" && estadoAnterior != "Ocupado")
            {
                sucursal.NumeroEspaciosDisponibles -= 1;
                _context.Entry(sucursal).State = EntityState.Modified;
            }
            else if (estadoNuevo == "Disponible" && estadoAnterior == "Ocupado")
            {
                sucursal.NumeroEspaciosDisponibles += 1;
                _context.Entry(sucursal).State = EntityState.Modified;
            }

            _context.Entry(espacio).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(espacio);
        }





        // Eliminar un espacio de parqueo
        [HttpDelete]
        [Route("Espacios/Delete/{id}")]
        public IActionResult EliminarEspacioParqueo(int id)
        {
            var espacio = _context.EspaciosParqueo.Find(id);
            if (espacio == null) return NotFound();

            if (espacio.Estado == "Disponible")
            {
                var sucursal = _context.Sucursales.Find(espacio.SucursalId);
                if (sucursal != null)
                {
                    sucursal.NumeroEspaciosDisponibles -= 1;
                    _context.Entry(sucursal).State = EntityState.Modified;
                }
            }

            _context.EspaciosParqueo.Remove(espacio);
            _context.SaveChanges();

            return Ok(espacio);
        }

    }
}
