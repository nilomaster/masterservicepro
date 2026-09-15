# MasterServicePro - Atualizações

## Correção: Lançamento de Troco no Caixa de OS

- **Data**: 15/09/2026
- **Arquivo Modificado**: MasterServicePro/Forms/FrmOSWeb.cs
- **Motivo**: 
  Foi detectado um problema onde o sistema estava subtraindo incorretamente os valores de "Troco" no momento de fechar a OS (Ordem de Serviço). Anteriormente, se a OS fosse R$ 40 e o cliente pagasse com R$ 50 (troco de R$ 10), o sistema lançava a entrada de R$ 40 mas lançava uma saída errônea de R$ 10 (Troco), gerando um caixa líquido registrado incorretamente (R$ 30, em vez de R$ 40).
  O mesmo erro ocorria no pagamento Misto.
- **Resolução**: 
  - A lógica de dupla dedução foi corrigida.
  - Pagamentos na opção *Dinheiro*: Registram exatamente o os.ValorTotal recebido, sem lançar uma Saída para o troco, garantindo que apenas a entrada líquida exata da OS conte na gaveta.
  - Pagamentos na opção *Misto*: O valor rm.Troco é abatido diretamente de rm.MistoDinheiro no momento do lançamento, garantindo que não aconteça o registro da saída indevida. O saldo registrará corretamente a entrada do dinheiro líquido recebido para cobrir o serviço, mantendo a contabilidade limpa de "Retiradas".
