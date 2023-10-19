using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PruebaLogin.Data;
using PruebaLogin.Models;
using PruebaLogin.Tools;

namespace PruebaLogin.Controllers
{
    [Route("[controller]")]
    public class SesionController : Controller
    {
        private IConfiguration _configuration;
        private readonly ILogger<SesionController> _logger;
        private readonly DataContext _context;

        public SesionController(ILogger<SesionController> logger, DataContext context,IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public dynamic InicioSesion ([FromBody] object obj)
        {
            EncriptaMD5 encriptaMD5 = new EncriptaMD5();
            var data = JsonConvert.DeserializeObject<dynamic>(obj.ToString());
            string user = data.usuario.ToString();
            string password = data.password.ToString();
            string verificaContraseña ;
            

            Login usuario = _context.logins.Where(x => x.Usuario == user).FirstOrDefault();

            if(usuario == null)
            {
                return new{
                    success = false,
                    message = "Credenciales Incorrectas",
                    result = ""
                };
            }
            else
            {
                verificaContraseña = encriptaMD5.Decrypt(Convert.ToBase64String(usuario.Password));   
                if (verificaContraseña != password)
                {
                    return new{
                        success = false,
                        message = "Credenciales Incorrectas",
                        result = ""
                    };
                }         
            }

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id",usuario.Id.ToString()),
                new Claim("usuario",usuario.Usuario),
                new Claim("Nombre",usuario.Nombre)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: singIn
            );

            return new 
            {
                success = true,
                message = "Se logeo",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        [HttpPost]
        [Route("UpdatePassword")]
        [Authorize]
        public async Task<ActionResult> UpdatePass([FromBody]object obj)
        {
            try
            {
                EncriptaMD5 encriptaMD5 = new EncriptaMD5();
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var data = JsonConvert.DeserializeObject<dynamic>(obj.ToString());
                string password = data.password.ToString();
                var rToken = Jwt.validartoken(identity);

                if (!rToken.success) return NotFound();
                string iduser = rToken.result.ToString();
                var login = _context.logins.FirstOrDefault(x => x.Id.ToString() == iduser);

                if (login == null)
                {
                    return NotFound();
                }

                login.Password = encriptaMD5.Encrypt(password);
                _context.Entry(login).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }

        }

        [HttpGet(Name = "GetUsuarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Login>>> GetUsuarios()
        {
            return await _context.logins.ToListAsync();

            
        }

        [HttpGet("{id}",Name ="GetUsuario")]
        [Authorize]
        public async Task<ActionResult<Login>> GetUsuario(int id)
        {
            var user = await _context.logins.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<Login>> Post(Login login)
        {
            EncriptaMD5 encriptaMD5 = new EncriptaMD5();
            var verifUsuario = _context.logins.FirstOrDefault(x => x.Usuario == login.Usuario);
            if ( verifUsuario != null )
            {
                return Content("Usuario ya creado");
            }
            login.Password = encriptaMD5.Encrypt(login.auxPassword);
            _context.Add(login);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, Login login)
        {
            if (id != login.Id)
            {
                return BadRequest();
            }

            _context.Entry(login).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{id}")]
        [Authorize]
        
        public async Task<ActionResult<Login>> Delete(int id)
        {
            var login = await _context.logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            _context.logins.Remove(login);
            await _context.SaveChangesAsync();

            return login;
        }

    }
}