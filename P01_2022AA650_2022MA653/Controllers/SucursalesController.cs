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
            var usuario = _context.Usuarios.Find(sucursal.UsuarioId);
            if (usuario == null)
            {
                return NotFound("El usuario especificado no existe.");
            }

            // Añadir la nueva sucursal
            _context.Sucursales.Add(sucursal);
            _context.SaveChanges();
            return Ok(sucursal);
        }

        // Actualizar una sucursal
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult ActualizarSucursal(int id, [FromBody] Sucursales sucursalModificada)
        {
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
            sucursal.NumeroEspaciosDisponibles = sucursalModificada.NumeroEspaciosDisponibles;

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
            if (sucursal == null) return NotFound();

            _context.Sucursales.Remove(sucursal);
            _context.SaveChanges();

            return Ok(sucursal);
        }
    }
}
