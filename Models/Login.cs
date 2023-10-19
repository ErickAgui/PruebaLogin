using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaLogin.Models
{
    public class Login
    {
        public int Id {get; set; }
        public string Nombre {get; set;}= string.Empty;
        public string Usuario {get; set;}
        public string Correo {get; set;}
        public byte[] Password {get; set;}
        public string? auxPassword {get; set;}
    }
}