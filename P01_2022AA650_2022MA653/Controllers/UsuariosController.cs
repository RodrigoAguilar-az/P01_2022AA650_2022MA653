using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022AA650_2022MA653.Models;

namespace P01_2022AA650_2022MA653.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ClaseContext _claseContext;

        public UsuariosController(ClaseContext claseContext)
        {

            _claseContext = claseContext;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            var listadoUsuarios = (from e in _claseContext.Usuarios
                                   select new
                                   {
                                       e.Id,
                                       e.Nombre,
                                       e.Correo,
                                       e.Telefono,
                                       e.Rol
                                   }).ToList();

            if (listadoUsuarios.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoUsuarios);
        }

        [HttpGet]
        [Route("GetById{id}")]
        public IActionResult Get(int id)
        {
            Usuarios? usuario = (from e in _claseContext.Usuarios
                                 where e.Id == id
                                 select e).FirstOrDefault();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);

        }

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarUsario([FromBody] Usuarios usuarios)
        {
            try
            {
                _claseContext.Usuarios.Add(usuarios);
                _claseContext.SaveChanges();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion([FromBody] Login login)
        {
            try
            {
                var usuario = _claseContext.Usuarios
                    .Where(u => u.Correo == login.Correo && u.Contraseña == login.Contraseña)
                    .FirstOrDefault();

                if (usuario == null)
                {
                    return BadRequest("Credenciales inválidas");
                }

                var usuarioInfo = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol
                };

                return Ok(new { mensaje = "Exito, Inicio de sesión", usuario = usuarioInfo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarUsuario(int id, [FromBody] Usuarios usarioUpdate)
        {
            Usuarios? UserActual = (from e in _claseContext.Usuarios
                                    where e.Id == id
                                    select e).FirstOrDefault();

            if (UserActual == null)
            {
                return NotFound();
            }

            UserActual.Nombre = usarioUpdate.Nombre;
            UserActual.Correo = usarioUpdate.Correo;
            UserActual.Telefono = usarioUpdate.Telefono;
            UserActual.Contraseña = usarioUpdate.Contraseña;
            UserActual.Rol = usarioUpdate.Rol;

            _claseContext.Entry(UserActual).State = EntityState.Modified;
            _claseContext.SaveChanges();


            return Ok(usarioUpdate);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarUser(int id)
        {
            Usuarios? usuarios = (from e in _claseContext.Usuarios
                                  where e.Id == id
                                  select e).FirstOrDefault();

            if (usuarios == null)
            {
                return NotFound();
            }

            _claseContext.Usuarios.Attach(usuarios);
            _claseContext.Usuarios.Remove(usuarios);
            _claseContext.SaveChanges();

            return Ok(usuarios);
        }

    }
}
