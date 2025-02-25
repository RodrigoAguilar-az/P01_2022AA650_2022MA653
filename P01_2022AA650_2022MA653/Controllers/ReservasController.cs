using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022AA650_2022MA653.Models;

namespace P01_2022AA650_2022MA653.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ClaseContext _claseContext;
        public ReservasController(ClaseContext claseContext)
        {

            _claseContext = claseContext;
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] Reservas request)
        {
            // Verificar que el espacio esté disponible
            var espacio = await _claseContext.EspaciosParqueo
                .FirstOrDefaultAsync(e => e.Id == request.EspacioParqueoId && e.Estado == "Disponible");

            if (espacio == null)
                return BadRequest("El espacio no está disponible.");

            var nuevaHoraFin = request.HoraInicio.Add(TimeSpan.FromHours(request.CantidadHoras));

            // Obtener reservas existentes del mismo espacio y fecha
            var reservasExistentes = await _claseContext.Reservas
                .Where(r => r.EspacioParqueoId == request.EspacioParqueoId && r.Fecha.Date == request.Fecha.Date)
                .ToListAsync();

            // Verificar conflictos de horario
            bool existeConflicto = reservasExistentes.Any(r =>
            {
                var horaFinExistente = r.HoraInicio.Add(TimeSpan.FromHours(r.CantidadHoras));
                return request.HoraInicio < horaFinExistente && nuevaHoraFin > r.HoraInicio;
            });

            if (existeConflicto)
                return BadRequest("Ya existe una reserva en el rango de horas seleccionado.");

            // Crear la reserva
            var reserva = new Reservas
            {
                UsuarioId = request.UsuarioId,
                EspacioParqueoId = request.EspacioParqueoId,
                Fecha = request.Fecha,
                HoraInicio = request.HoraInicio,
                CantidadHoras = request.CantidadHoras
            };

            _claseContext.Reservas.Add(reserva);
            await _claseContext.SaveChangesAsync();

            return Ok("Reserva creada con éxito.");
        }


        [HttpGet("reservas-activas/{usuarioId}")]
        public async Task<IActionResult> GetReservasActivas(int usuarioId)
        {
            // Obtener la fecha y hora actual
            var ahora = DateTime.Now;

            // Realizar el Join entre Reservas y EspaciosParqueo
            var reservasActivas = await (from r in _claseContext.Reservas
                                         join e in _claseContext.EspaciosParqueo
                                         on r.EspacioParqueoId equals e.Id
                                         where r.UsuarioId == usuarioId &&
                                               r.Fecha.Date >= ahora.Date &&
                                               (r.Fecha.Date > ahora.Date || r.HoraInicio >= ahora.TimeOfDay)
                                         select new
                                         {
                                             r.Id,
                                             r.UsuarioId,
                                             r.EspacioParqueoId,
                                             r.Fecha,
                                             r.HoraInicio,
                                             r.CantidadHoras,
                                             EspacioParqueo = new
                                             {
                                                 e.Numero,
                                                 e.Ubicacion,
                                                 e.CostoPorHora
                                             }
                                         })
                                          .ToListAsync();

            if (reservasActivas.Count == 0)
                return Ok("No tienes reservas activas.");

            return Ok(reservasActivas);
        }





    }
}
