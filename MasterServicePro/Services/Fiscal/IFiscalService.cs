using System;
using System.Threading.Tasks;
using MasterServicePro.Models.Fiscal;

namespace MasterServicePro.Services.Fiscal
{
    public interface IFiscalService
    {
        Task<FiscalResponse> EmitirNFCeAsync(VendaFiscalDTO venda);
        Task<FiscalResponse> EmitirNFeAsync(VendaFiscalDTO venda);
        Task<FiscalResponse> CancelarNotaAsync(string chaveAcesso, string justificativa);
    }

    public class FiscalResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string ChaveAcesso { get; set; }
        public string NumeroRecibo { get; set; }
        public string CaminhoXml { get; set; }
        public string CaminhoPdf { get; set; }
        public string UrlDanfe { get; set; }
    }
}
