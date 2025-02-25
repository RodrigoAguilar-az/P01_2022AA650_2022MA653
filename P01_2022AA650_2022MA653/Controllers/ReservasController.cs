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
            var espacio = await _claseContext.EspaciosParqueo
                .FirstOrDefaultAsync(e => e.Id == request.EspacioParqueoId && e.Estado == "Disponible");

            if (espacio == null)
                return BadRequest("El espacio no está disponible.");

            var nuevaHoraFin = request.HoraInicio.Add(TimeSpan.FromHours(request.CantidadHoras));

            var reservasExistentes = await _claseContext.Reservas
                .Where(r => r.EspacioParqueoId == request.EspacioParqueoId && r.Fecha.Date == request.Fecha.Date)
                .ToListAsync();

            bool existeConflicto = reservasExistentes.Any(r =>
            {
                var horaFinExistente = r.HoraInicio.Add(TimeSpan.FromHours(r.CantidadHoras));
                return request.HoraInicio < horaFinExistente && nuevaHoraFin > r.HoraInicio;
            });

            if (existeConflicto)
                return BadRequest("Ya existe una reserva en el rango de horas seleccionado.");

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
            var ahora = DateTime.Now;
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

        [HttpDelete("cancelar/{reservaId}")]
        public async Task<IActionResult> CancelarReserva(int reservaId)
        {
            var reserva = await _claseContext.Reservas
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            var ahora = DateTime.Now;

            if (reserva.Fecha.Date < ahora.Date ||
               (reserva.Fecha.Date == ahora.Date && reserva.HoraInicio <= ahora.TimeOfDay))
            {
                return BadRequest("No se puede cancelar una reserva que ya pasó.");
            }

            _claseContext.Reservas.Remove(reserva);
            await _claseContext.SaveChangesAsync();

            return Ok("Reserva cancelada con éxito.");
        }

        [HttpGet("espacios-reservados/{sucursalId}")]
        public async Task<IActionResult> GetEspaciosReservados(int sucursalId, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var espaciosReservados = await (from r in _claseContext.Reservas
                                            join e in _claseContext.EspaciosParqueo
                                            on r.EspacioParqueoId equals e.Id
                                            where e.SucursalId == sucursalId &&
                                                  r.Fecha.Date >= fechaInicio.Date &&
                                                  r.Fecha.Date <= fechaFin.Date
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

            if (espaciosReservados.Count == 0)
                return Ok("No hay espacios reservados en ese rango de fechas.");

            return Ok(espaciosReservados);
        }

    }
}
