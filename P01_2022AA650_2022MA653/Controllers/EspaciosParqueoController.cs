using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // Obtener todos los espacios de parqueo disponibles
        [HttpGet]
        [Route("EspaciosDisponibles")]
        public IActionResult GetEspaciosDisponibles()
        {
            var espacios = _context.EspaciosParqueo.Where(e => e.Estado == "Disponible").ToList();
            return Ok(espacios);
        }

        // Registrar un nuevo espacio de parqueo por sucursal
        [HttpPost]
        [Route("Espacios/Add")]
        public IActionResult CrearEspacioParqueo([FromBody] EspaciosParqueo espacio)
        {
            var sucursal = _context.Sucursales.Find(espacio.SucursalId);
            if (sucursal == null) return NotFound("Sucursal no encontrada");

            _context.EspaciosParqueo.Add(espacio);
            _context.SaveChanges();
            return Ok(espacio);
        }

        // Actualizar un espacio de parqueo
        [HttpPut]
        [Route("Espacios/Update/{id}")]
        public IActionResult ActualizarEspacioParqueo(int id, [FromBody] EspaciosParqueo espacioModificado)
        {
            var espacio = _context.EspaciosParqueo.Find(id);
            if (espacio == null) return NotFound();

            espacio.Numero = espacioModificado.Numero;
            espacio.Ubicacion = espacioModificado.Ubicacion;
            espacio.CostoPorHora = espacioModificado.CostoPorHora;
            espacio.Estado = espacioModificado.Estado;

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

            _context.EspaciosParqueo.Remove(espacio);
            _context.SaveChanges();

            return Ok(espacio);
        }
    }
}
