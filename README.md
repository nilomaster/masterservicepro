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
  - Pagamentos na opção *Misto*: O valor `frm.Troco` é abatido diretamente de `frm.MistoDinheiro` no momento do lançamento, garantindo que não aconteça o registro da saída indevida. O saldo registrará corretamente a entrada do dinheiro líquido recebido para cobrir o serviço, mantendo a contabilidade limpa de "Retiradas".

## Nova Feature: Escolha da Forma de Pagamento no Estorno

- **Data**: 15/09/2026
- **Arquivos Modificados**: 
  - MasterServicePro/Web/PromptEstorno/index.html
  - MasterServicePro/Forms/FrmPromptMotivoEstorno.cs
  - MasterServicePro/Forms/FrmRelatorios.cs
  - MasterServicePro/Forms/FrmCorrecoesAdminWeb.cs
  - MasterServicePro/DAL/VendaRepository.cs
- **Motivo**: 
  Quando o cliente realizava o estorno de uma venda/OS paga via Pix ou Cartão, o sistema deduzia o estorno diretamente como "Dinheiro" do caixa físico, não permitindo registrar a devolução na conta de onde ele de fato foi retirado (Ex: estorno do Pix da empresa).
- **Resolução**: 
  - Foi adicionado um dropdown Forma de Pagamento (Devolução) na tela de motivo de estorno, que aparece quando a opção selecionada é "Devolver Dinheiro (Saída do Caixa)".
  - Ao confirmar, o front-end envia a escolha do pagamento ao C#.
  - O VendaRepository foi atualizado para registrar a movimentação de Saída com a exata FormaPagamento escolhida pelo usuário.
