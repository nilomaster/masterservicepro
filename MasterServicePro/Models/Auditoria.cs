using System;

namespace MasterServicePro.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Acao { get; set; }
        public string Tabela { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHora { get; set; }

        public Auditoria()
        {
            DataHora = DateTime.Now;
        }
    }
}
