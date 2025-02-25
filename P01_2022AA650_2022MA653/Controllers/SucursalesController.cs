using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022AA650_2022MA653.Models;

namespace P01_2022AA650_2022MA653.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : ControllerBase
    {
        private readonly ClaseContext _context;

        public SucursalesController(ClaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetSucursales()
        {
            var sucursales = _context.Sucursales
                .Select(s => new
                {
                    s.Id,
                    s.Nombre,
                    s.Direccion,
                    s.Telefono,
                    s.UsuarioId, 
                    UsuarioNombre = _context.Usuarios
                        .Where(u => u.Id == s.UsuarioId)
                        .Select(u => u.Nombre)
                        .FirstOrDefault(),  
                    s.NumeroEspaciosDisponibles
                })
                .ToList();

            return Ok(sucursales);
        }


        // Crear una nueva sucursal
        [HttpPost]
        [Route("Add")]
        public IActionResult CrearSucursal([FromBody] Sucursales sucursal)
        {
            if (sucursal == null)
            {
                return BadRequest("Los datos de la sucursal no pueden ser nulos.");
            }

            if (string.IsNullOrEmpty(sucursal.Nombre))
            {
                return BadRequest("El campo 'Nombre' es obligatorio.");
            }
            if (string.IsNullOrEmpty(sucursal.Direccion))
            {
                return BadRequest("El campo 'Direccion' es obligatorio.");
            }
            if (string.IsNullOrEmpty(sucursal.Telefono))
            {
                return BadRequest("El campo 'Telefono' es obligatorio.");
            }
            if (sucursal.UsuarioId == 0)
            {
                return BadRequest("El campo 'UsuarioId' es obligatorio.");
            }

            var usuario = _context.Usuarios.Find(sucursal.UsuarioId);
            if (usuario == null)
            {
                return NotFound("El usuario especificado no existe.");
            }

            // número de espacios disponibles a 0 siempre
            sucursal.NumeroEspaciosDisponibles = 0;

            _context.Sucursales.Add(sucursal);
            _context.SaveChanges();
            return Ok(sucursal);
        }


        // Actualizar una sucursal
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult ActualizarSucursal(int id, [FromBody] Sucursales sucursalModificada)
        {
            if (sucursalModificada == null)
            {
                return BadRequest("Los datos de la sucursal no pueden ser nulos.");
            }

            if (string.IsNullOrEmpty(sucursalModificada.Nombre))
            {
                return BadRequest("El campo 'Nombre' es obligatorio.");
            }
            if (string.IsNullOrEmpty(sucursalModificada.Direccion))
            {
                return BadRequest("El campo 'Direccion' es obligatorio.");
            }
            if (string.IsNullOrEmpty(sucursalModificada.Telefono))
            {
                return BadRequest("El campo 'Telefono' es obligatorio.");
            }
            if (sucursalModificada.UsuarioId == 0)
            {
                return BadRequest("El campo 'UsuarioId' es obligatorio.");
            }

            var sucursal = _context.Sucursales.Find(id);
            if (sucursal == null)
            {
                return NotFound("Sucursal no encontrada.");
            }

            var usuarioExiste = _context.Usuarios.Any(u => u.Id == sucursalModificada.UsuarioId);
            if (!usuarioExiste)
            {
                return NotFound("El usuario especificado no existe.");
            }

            sucursal.Nombre = sucursalModificada.Nombre;
            sucursal.Direccion = sucursalModificada.Direccion;
            sucursal.Telefono = sucursalModificada.Telefono;
            sucursal.UsuarioId = sucursalModificada.UsuarioId;

            _context.Entry(sucursal).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(sucursal);
        }



        // Eliminar una sucursal
        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult EliminarSucursal(int id)
        {
            var sucursal = _context.Sucursales.Find(id);
            if (sucursal == null)
            {
                return NotFound("Sucursal no encontrada.");
            }

            // Elimina los espacios de parqueo asociados a la sucursal
            var espacios = _context.EspaciosParqueo.Where(e => e.SucursalId == id).ToList();
            if (espacios.Any())
            {
                _context.EspaciosParqueo.RemoveRange(espacios);
            }

            _context.Sucursales.Remove(sucursal);
            _context.SaveChanges();

            return Ok(sucursal);
        }

    }
}
