using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using MasterServicePro.Models;
using MasterServicePro.DAL;

namespace MasterServicePro.Utils
{
    public class OSPrinter
    {
        private OrdemServico _os;
        private Cliente _cliente;
        private bool _isOrcamento;
        private bool _isReciboEntrega;
        private bool _isEtiqueta;

        public void Imprimir(int idOS)
        {
            _isOrcamento = false;
            _isReciboEntrega = false;
            _isEtiqueta = false;
            var repoOS = new OrdemServicoRepository();
            var repoCli = new ClienteRepository();

            _os = repoOS.BuscarPorId(idOS);
            if (_os == null) 
            {
                MessageBox.Show("Ordem de serviço não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _cliente = repoCli.BuscarPorId(_os.IdCliente);

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            pd.DocumentName = $"Comprovante_OS_{_os.NumeroOS}";

            pd.DefaultPageSettings.PaperSize = new PaperSize("Cupom80mm", 300, 800);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = pd;
            previewDialog.WindowState = FormWindowState.Normal;
            previewDialog.Width = 350;
            previewDialog.Height = 600;
            previewDialog.ShowIcon = false;
            previewDialog.Text = $"Comprovante de Ordem de Serviço - {_os.NumeroOS}";
            
            previewDialog.ShowDialog();
        }

        public void ImprimirOrcamento(int idOS)
        {
            var repoOS = new OrdemServicoRepository();
            var repoCli = new ClienteRepository();

            _os = repoOS.BuscarPorId(idOS);
            if (_os == null) 
            {
                MessageBox.Show("Ordem de serviço não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _cliente = repoCli.BuscarPorId(_os.IdCliente);
            _isOrcamento = true;
            _isReciboEntrega = false;
            _isEtiqueta = false;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            pd.DocumentName = $"Orcamento_OS_{_os.NumeroOS}";

            pd.DefaultPageSettings.PaperSize = new PaperSize("Cupom80mm", 300, 800);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = pd;
            previewDialog.WindowState = FormWindowState.Normal;
            previewDialog.Width = 350;
            previewDialog.Height = 600;
            previewDialog.ShowIcon = false;
            previewDialog.Text = $"Orçamento de Serviço - {_os.NumeroOS}";
            
            previewDialog.ShowDialog();
        }

        public void ImprimirReciboEntrega(int idOS)
        {
            var repoOS = new OrdemServicoRepository();
            var repoCli = new ClienteRepository();

            _os = repoOS.BuscarPorId(idOS);
            if (_os == null) 
            {
                MessageBox.Show("Ordem de serviço não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _cliente = repoCli.BuscarPorId(_os.IdCliente);
            _isOrcamento = false;
            _isReciboEntrega = true;
            _isEtiqueta = false;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            pd.DocumentName = $"Recibo_Entrega_OS_{_os.NumeroOS}";

            pd.DefaultPageSettings.PaperSize = new PaperSize("Cupom80mm", 300, 800);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = pd;
            previewDialog.WindowState = FormWindowState.Normal;
            previewDialog.Width = 350;
            previewDialog.Height = 600;
            previewDialog.ShowIcon = false;
            previewDialog.Text = $"Recibo de Entrega e Garantia - {_os.NumeroOS}";
            
            previewDialog.ShowDialog();
        }

        public void ImprimirEtiquetaAparelho(int idOS)
        {
            var repoOS = new OrdemServicoRepository();
            var repoCli = new ClienteRepository();

            _os = repoOS.BuscarPorId(idOS);
            if (_os == null) 
            {
                MessageBox.Show("Ordem de serviço não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _cliente = repoCli.BuscarPorId(_os.IdCliente);
            _isOrcamento = false;
            _isReciboEntrega = false;
            _isEtiqueta = true;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            pd.DocumentName = $"Etiqueta_OS_{_os.NumeroOS}";
            
            // Bobina térmica 80mm de largura (~300px), altura compacta (~200px)
            pd.DefaultPageSettings.PaperSize = new PaperSize("Etiqueta80mm", 300, 200);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = pd;
            previewDialog.WindowState = FormWindowState.Normal;
            previewDialog.Width = 350;
            previewDialog.Height = 280;
            previewDialog.ShowIcon = false;
            previewDialog.Text = $"Etiqueta O.S. {_os.NumeroOS}";
            
            previewDialog.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (_isEtiqueta)
            {
                RenderEtiquetaAparelho(g, e.PageBounds);
                return;
            }

            var conf = new ConfiguracaoTermosRepository().ObterConfiguracao();

            // Fontes Baseadas no Modelo Bobina
            Font fontHeaderBold = new Font("Arial", 12, FontStyle.Bold);
            Font fontText = new Font("Arial", 8, FontStyle.Regular);
            Font fontTextBold = new Font("Arial", 8, FontStyle.Bold);
            Font fontSmall = new Font("Arial", 7, FontStyle.Regular);
            Font fontSmallBold = new Font("Arial", 7, FontStyle.Bold);

            Brush brush = Brushes.Black;
            int startX = 5;
            int startY = 10;
            int width = e.PageBounds.Width - 10;
            int y = startY;

            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

            // 1. Cabeçalho
            string nomeLoja = string.IsNullOrWhiteSpace(conf.NomeLoja) ? "NOME DE FANTASIA" : conf.NomeLoja.ToUpper();
            SizeF sizeNome = g.MeasureString(nomeLoja, fontHeaderBold, width, centerFormat);
            g.DrawString(nomeLoja, fontHeaderBold, brush, new RectangleF(startX, y, width, sizeNome.Height), centerFormat);
            y += (int)sizeNome.Height + 5;

            SizeF sizeEnd = g.MeasureString(conf.Endereco, fontText, width, centerFormat);
            g.DrawString(conf.Endereco, fontText, brush, new RectangleF(startX, y, width, sizeEnd.Height), centerFormat);
            y += (int)sizeEnd.Height + 5;

            g.DrawString(conf.Contato, fontText, brush, new RectangleF(startX, y, width, 15), centerFormat);
            y += 20;

            string docInfo = "";
            if (!string.IsNullOrWhiteSpace(conf.Cnpj)) docInfo += $"CNPJ : {conf.Cnpj}      ";
            docInfo += "IE : ISENTO";
            g.DrawString(docInfo, fontText, brush, startX, y);
            y += 15;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 2. Cliente
            string nomeCliente = !string.IsNullOrEmpty(_os.ClienteFinal) ? _os.ClienteFinal : (_cliente?.Nome ?? "N/A");
            g.DrawString($"CLIENTE : {nomeCliente.ToUpper()}", fontText, brush, startX, y);
            y += 15;
            string foneCliente = _cliente?.Telefone ?? _cliente?.WhatsApp ?? "N/A";
            g.DrawString($"FONE : {foneCliente}", fontText, brush, startX, y);
            y += 15;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 3. Info do Comprovante
            g.DrawString(_os.DataAbertura.ToString("dd/MM/yyyy   HH:mm"), fontText, brush, startX, y + 10);
            
            string titleOS = _isReciboEntrega ? "RECIBO DE ENTREGA E GARANTIA" : (_isOrcamento ? "ORÇAMENTO DE SERVIÇO" : "COMPROVANTE DE O.S.");
            g.DrawString(titleOS, fontSmallBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            g.DrawString($"Nº {_os.NumeroOS}", fontHeaderBold, brush, new RectangleF(startX, y + 10, width, 20), rightFormat);
            y += 30;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 4. Aparelho
            g.DrawString("DADOS DO APARELHO", fontTextBold, brush, startX, y);
            y += 15;
            g.DrawString($"Marca: {_os.Marca}", fontText, brush, startX, y);
            g.DrawString($"Modelo: {_os.Modelo}", fontText, brush, startX + 130, y);
            y += 15;
            g.DrawString($"Cor: {_os.Cor}", fontText, brush, startX, y);
            g.DrawString($"IMEI/SN: {_os.IMEI}", fontText, brush, startX + 130, y);
            y += 15;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 5. Defeito e Laudo
            g.DrawString("DEFEITO RELATADO:", fontTextBold, brush, startX, y);
            y += 12;
            string def = string.IsNullOrEmpty(_os.Defeito) ? "Nenhum" : _os.Defeito;
            g.DrawString(def, fontText, brush, new RectangleF(startX, y, width, 40));
            y += 40;

            g.DrawString("LAUDO TÉCNICO / SERVIÇO:", fontTextBold, brush, startX, y);
            y += 12;
            string lau = string.IsNullOrEmpty(_os.LaudoTecnico) ? "Aguardando..." : _os.LaudoTecnico;
            g.DrawString(lau, fontText, brush, new RectangleF(startX, y, width, 40));
            y += 40;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 6. Valores
            g.DrawString("Desconto R$", fontTextBold, brush, startX, y);
            g.DrawString(_os.Desconto.ToString("N2"), fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 15;

            g.DrawString("VALOR TOTAL R$", fontHeaderBold, brush, startX, y);
            g.DrawString(_os.ValorServico.ToString("N2"), fontHeaderBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 25;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 7. Rodapé / Termos
            string footer = "";
            if (_isReciboEntrega)
            {
                footer = "DECLARAÇÃO DE ENTREGA:\n" +
                         "Declaro que recebi o aparelho em perfeito estado de funcionamento e com o serviço concluído conforme solicitado.\n\n" +
                         "TERMO DE GARANTIA:\n" +
                         "O serviço executado possui garantia legal de 90 dias a partir da data de entrega, cobrindo exclusivamente defeitos relacionados à mão de obra ou peças substituídas. A garantia não cobre danos por mau uso, quedas ou líquidos.";
            }
            else
            {
                footer = conf.TermosOS;
            }

            g.DrawString(footer, fontSmall, Brushes.Gray, new RectangleF(startX, y, width, 120), centerFormat);
            y += 125;

            DrawDottedLine(g, startX, width, y);
            y += 12;
            g.DrawString("ASSINATURA DO CLIENTE", fontText, brush, new RectangleF(startX, y, width, 15), centerFormat);
            y += 30;

            g.DrawString("* OBRIGADO PELA PREFERÊNCIA *", fontTextBold, brush, new RectangleF(startX, y, width, 15), centerFormat);
        }

        private void DrawDottedLine(Graphics g, int x, int width, int y)
        {
            using (Pen dashedPen = new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(dashedPen, x, y, x + width, y);
            }
        }
        private void RenderEtiquetaAparelho(Graphics g, Rectangle bounds)
        {
            Font fontHeader = new Font("Arial", 8, FontStyle.Bold);
            Font fontOS = new Font("Arial", 16, FontStyle.Bold);
            Font fontTextBold = new Font("Arial", 9, FontStyle.Bold);
            Font fontText = new Font("Arial", 9, FontStyle.Regular);
            Font fontSmall = new Font("Arial", 7, FontStyle.Regular);

            Brush brush = Brushes.Black;
            Pen pen = new Pen(Color.Black, 1);

            int startX = 10;
            int startY = 10;
            int width = bounds.Width - 20;

            var conf = new ConfiguracaoTermosRepository().ObterConfiguracao();

            // 1. Título do Estabelecimento
            string nomeLoja = string.IsNullOrWhiteSpace(conf.NomeLoja) ? "MASTER SERVICE PRO" : conf.NomeLoja;
            if (nomeLoja.Length > 20) nomeLoja = nomeLoja.Substring(0, 17) + "...";
            g.DrawString(nomeLoja, fontHeader, brush, startX, startY);
            
            // Data e hora de entrada à direita
            string entryDate = _os.DataAbertura.ToString("dd/MM/yy HH:mm");
            SizeF dateSize = g.MeasureString(entryDate, fontSmall);
            g.DrawString(entryDate, fontSmall, brush, startX + width - dateSize.Width, startY + 1);

            // Linha pontilhada / separadora
            startY += 15;
            using (Pen dashedPen = new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(dashedPen, startX, startY, startX + width, startY);
            }

            // 2. Número da OS (Grande e Negrito)
            startY += 5;
            string osText = $"O.S. #{_os.NumeroOS}";
            g.DrawString(osText, fontOS, brush, startX, startY);

            // Status ou outra info pequena
            string statusText = _os.Status?.ToUpper() ?? "ABERTO";
            SizeF statusSize = g.MeasureString(statusText, fontHeader);
            g.DrawString(statusText, fontHeader, brush, startX + width - statusSize.Width, startY + 6);

            // 3. Cliente
            startY += 28;
            string clientName = !string.IsNullOrEmpty(_os.ClienteFinal) ? _os.ClienteFinal : (_cliente?.Nome ?? "N/A");
            if (clientName.Length > 28) clientName = clientName.Substring(0, 25) + "...";
            g.DrawString($"Cliente: {clientName}", fontTextBold, brush, startX, startY);

            // 4. Aparelho (Marca / Modelo / Cor)
            startY += 17;
            string deviceText = $"{_os.Marca} {_os.Modelo} ({_os.Cor})".Trim();
            if (deviceText.Length > 32) deviceText = deviceText.Substring(0, 29) + "...";
            g.DrawString($"Aparelho: {deviceText}", fontText, brush, startX, startY);

            // 5. Defeito
            startY += 17;
            string defectText = string.IsNullOrEmpty(_os.Defeito) ? "Nenhum relatado" : _os.Defeito;
            if (defectText.Length > 32) defectText = defectText.Substring(0, 29) + "...";
            g.DrawString($"Defeito: {defectText}", fontText, brush, startX, startY);

            // Linha pontilhada no final
            startY += 18;
            using (Pen dashedPen = new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(dashedPen, startX, startY, startX + width, startY);
            }

            // Rodapé da etiqueta
            startY += 4;
            g.DrawString("Evite perda: Identifique o aparelho na bancada.", fontSmall, Brushes.DimGray, startX, startY);
        }
    }
}
