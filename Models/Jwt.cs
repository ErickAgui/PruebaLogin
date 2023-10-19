using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using PruebaLogin.Data;

namespace PruebaLogin.Models
{
    public class Jwt
    {
        public string Key {get; set;}
        public string Issuer {get; set;}
        public string Audience {get; set;}
        public string Subject {get; set;}

        public static dynamic validartoken(ClaimsIdentity identity)
        {
            
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new 
                    {
                        success = false,
                        message = "Verifica si estas enviando un token valido",
                        result = ""
                    };
                }
                var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value;

                return new 
                {
                    success = true,
                    message = "exito",
                    result = id
                };
            }
            catch(Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Catch: " + ex.Message,
                    result = ""
                };
            }
        }
    }
}