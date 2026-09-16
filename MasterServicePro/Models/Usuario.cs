using System;

namespace MasterServicePro.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; } // Hashed password
        public string Nivel { get; set; } // "Admin" or "Funcionário"
        public bool Ativo { get; set; }

        public Usuario()
        {
            Ativo = true;
        }
    }
}
