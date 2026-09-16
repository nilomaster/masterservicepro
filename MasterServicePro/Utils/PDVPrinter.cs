using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using MasterServicePro.DAL;

namespace MasterServicePro.Utils
{
    public class PDVPrinter
    {
        private int _vendaId;
        private decimal _valorRecebido;
        private decimal _troco;
        private string _formaPagamento;

        public void ImprimirVenda(int vendaId, decimal valorRecebido, decimal troco, string formaPagamento)
        {
            _vendaId = vendaId;
            _valorRecebido = valorRecebido;
            _troco = troco;
            _formaPagamento = formaPagamento;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            pd.DocumentName = $"Comprovante_Venda_{_vendaId}";
            
            // Bobina térmica 80mm de largura (~300px), altura dinâmica, definindo 300x800 como base
            pd.DefaultPageSettings.PaperSize = new PaperSize("Cupom80mm", 300, 800);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = pd;
            previewDialog.WindowState = FormWindowState.Normal;
            previewDialog.Width = 350;
            previewDialog.Height = 600;
            previewDialog.ShowIcon = false;
            previewDialog.Text = $"Comprovante de Venda - {_vendaId}";
            
            previewDialog.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var vendaRepo = new VendaRepository();
            var dtVenda = vendaRepo.BuscarPorId(_vendaId);
            if (dtVenda == null || dtVenda.Rows.Count == 0) return;

            DataRow venda = dtVenda.Rows[0];
            DateTime dataVenda = Convert.ToDateTime(venda["DataVenda"]);
            decimal totalFinal = Convert.ToDecimal(venda["TotalFinal"]);
            int clienteId = venda["ClienteId"] != DBNull.Value ? Convert.ToInt32(venda["ClienteId"]) : 0;
            
            string nomeCliente = "CONSUMIDOR FINAL";
            if (clienteId > 0)
            {
                var cliRepo = new ClienteRepository();
                var c = cliRepo.BuscarPorId(clienteId);
                if (c != null) nomeCliente = c.Nome;
            }

            var conf = new ConfiguracaoTermosRepository().ObterConfiguracao();

            // Fontes Baseadas no Modelo
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
            g.DrawString($"CLIENTE : {nomeCliente.ToUpper()}", fontText, brush, startX, y);
            y += 15;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 3. Info do Comprovante
            g.DrawString(dataVenda.ToString("dd/MM/yyyy   HH:mm"), fontText, brush, startX, y + 10);
            g.DrawString("COMPROVANTE DE VENDA", fontSmallBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            g.DrawString($"Nº {_vendaId.ToString("D6")}", fontHeaderBold, brush, new RectangleF(startX, y + 10, width, 20), rightFormat);
            y += 30;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 4. Cabeçalho da Tabela
            g.DrawString("CODIGO", fontText, brush, startX, y);
            g.DrawString("DESCRIÇÃO", fontText, brush, startX + 110, y);
            g.DrawString("GARANTIA", fontText, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 12;
            g.DrawString("QTD x UNIT", fontText, brush, startX + 110, y);
            g.DrawString("R$ VALOR", fontText, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 15;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 5. Itens da Tabela
            var dtItens = vendaRepo.BuscarItensVenda(_vendaId);
            foreach (DataRow row in dtItens.Rows)
            {
                string cod = row["ProdutoId"].ToString().PadLeft(5, '0');
                string prodName = row["Produto"].ToString().ToUpper();
                if (prodName.Length > 25) prodName = prodName.Substring(0, 22) + "...";
                decimal qtd = Convert.ToDecimal(row["Quantidade"]);
                decimal unit = Convert.ToDecimal(row["PrecoUnitario"]);
                decimal sub = Convert.ToDecimal(row["Subtotal"]);

                g.DrawString(cod, fontTextBold, brush, startX, y);
                g.DrawString(prodName, fontTextBold, brush, startX + 45, y);
                g.DrawString("90D", fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
                y += 12;
                g.DrawString($"{qtd:G} x {unit:N2}", fontTextBold, brush, startX + 110, y);
                g.DrawString(sub.ToString("N2"), fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
                y += 15;
            }

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 6. Totais
            g.DrawString("Total da Nota R$", fontTextBold, brush, startX, y);
            g.DrawString(totalFinal.ToString("N2"), fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 15;

            g.DrawString("Valor Recebido R$", fontTextBold, brush, startX, y);
            g.DrawString(_valorRecebido.ToString("N2"), fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 15;

            g.DrawString("Troco R$", fontTextBold, brush, startX, y);
            g.DrawString(_troco.ToString("N2"), fontTextBold, brush, new RectangleF(startX, y, width, 15), rightFormat);
            y += 20;

            DrawDottedLine(g, startX, width, y);
            y += 5;

            // 7. Pagamento
            g.DrawString($"FORMA DE PGTO. : {_formaPagamento.ToUpper()}", fontTextBold, brush, startX, y);
            y += 15;
            
            g.DrawString("DATA PGTO", fontTextBold, brush, startX, y);
            g.DrawString("R$ VALOR", fontTextBold, brush, startX + 110, y);
            g.DrawString("TIPO PGTO", fontTextBold, brush, startX + 200, y);
            y += 12;

            g.DrawString(dataVenda.ToString("dd/MM/yyyy"), fontText, brush, startX, y);
            g.DrawString(_valorRecebido.ToString("N2"), fontText, brush, startX + 110, y);
            g.DrawString(_formaPagamento.ToUpper(), fontText, brush, startX + 200, y);
            y += 15;

            DrawSolidLine(g, startX, width, y);
            y += 5;

            // 8. Vendedor
            string vendedor = AuthSession.Usuario ?? "CAIXA 1";
            g.DrawString($"VENDEDOR(A) : {vendedor.ToUpper()}", fontText, brush, startX, y);
            y += 15;

            DrawSolidLine(g, startX, width, y);
            y += 15;

            // 9. Garantia
            g.DrawString("Números de Série :", fontText, brush, startX, y);
            y += 15;
            g.DrawString("__________________________________________", fontText, brush, startX, y);
            y += 30;

            string termosText = "Recebi a(s) mercadoria(s) acima descrita(s), concordando plenamente com os prazos e condições de garantia.";
            g.DrawString(termosText, fontText, brush, new RectangleF(startX, y, width, 40), centerFormat);
            y += 50;

            DrawDottedLine(g, startX, width, y);
            y += 12;
            g.DrawString("ASSINATURA DO CLIENTE", fontText, brush, new RectangleF(startX, y, width, 15), centerFormat);
            y += 30;

            g.DrawString("* OBRIGADO E VOLTE SEMPRE *", fontTextBold, brush, new RectangleF(startX, y, width, 15), centerFormat);
        }

        private void DrawDottedLine(Graphics g, int x, int width, int y)
        {
            using (Pen dashedPen = new Pen(Color.Black, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(dashedPen, x, y, x + width, y);
            }
        }

        private void DrawSolidLine(Graphics g, int x, int width, int y)
        {
            g.DrawLine(Pens.Black, x, y, x + width, y);
        }
    }
}
