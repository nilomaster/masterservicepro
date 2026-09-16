using System;

namespace MasterServicePro.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        public Categoria()
        {
            Ativo = true;
        }
    }
}
