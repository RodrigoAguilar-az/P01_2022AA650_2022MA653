using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Crear una nueva reserva
        [HttpPost]
        [Route("Add")]
        public IActionResult CrearReserva([FromBody] Reservas reserva)
        {
            try
            {
                var espacio = _claseContext.EspaciosParqueos
                    .FirstOrDefault(e => e.Id == reserva.EspacioParqueoId);

                if (espacio == null)
                {
                    return BadRequest("Espacio de parqueo no existe");
                }

                if (espacio.Estado != "disponible")
                {
                    return BadRequest("El espacio de parqueo no está disponible");
                }
        
                var horaInicio = TimeSpan.Parse(reserva.HoraInicio.ToString());
                var horaFin = horaInicio.Add(TimeSpan.FromHours(reserva.CantidadHoras));

                var reservasExistentes = _claseContext.Reservas
                    .Where(r => r.EspacioParqueoId == reserva.EspacioParqueoId &&
                                r.Fecha == reserva.Fecha)
                    .ToList();

                foreach (var res in reservasExistentes)
                {
                    var resHoraInicio = TimeSpan.Parse(res.HoraInicio.ToString());
                    var resHoraFin = resHoraInicio.Add(TimeSpan.FromHours(res.CantidadHoras));

                    if ((horaInicio >= resHoraInicio && horaInicio < resHoraFin) ||
                        (horaFin > resHoraInicio && horaFin <= resHoraFin) ||
                        (horaInicio <= resHoraInicio && horaFin >= resHoraFin))
                    {
                        return BadRequest("Ya existe una reserva para este espacio en el horario");
                    }
                }

                _claseContext.Reservas.Add(reserva);
                _claseContext.SaveChanges();

                return Ok(new { mensaje = "Reserva creada", reserva });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
