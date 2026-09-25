// textosAutomaticos.js
// Catalog of standard reported problems and technical diagnoses for OS
// Rule compliance: ASCII characters only in code comments

const TEXTOS_AUTOMATICOS_OS = [
    // TELA / TOUCH
    {
        id: 1,
        categoria: "TELA / TOUCH",
        problema: "Tela trincada ou quebrada.",
        laudo: "Durante a inspeção visual, foram constatadas trincas e danos físicos no conjunto frontal do aparelho. Recomenda-se a substituição do conjunto da tela para restabelecer sua integridade e funcionamento adequado."
    },
    {
        id: 2,
        categoria: "TELA / TOUCH",
        problema: "Tela com pixels danificados ou vazamento.",
        laudo: "Durante os testes, foram identificadas manchas e/ou falhas de pixels no display, comprometendo a exibição correta da imagem. Recomenda-se a substituição do conjunto da tela."
    },
    {
        id: 3,
        categoria: "TELA / TOUCH",
        problema: "Tela apagada, porém o aparelho continua ligado e emitindo sons ou vibrações.",
        laudo: "O aparelho apresenta sinais de funcionamento, como sons e/ou vibrações, porém não exibe imagem no display. Será necessária análise do conjunto da tela, conexões e circuito responsável pela imagem para identificação da origem da falha."
    },
    {
        id: 4,
        categoria: "TELA / TOUCH",
        problema: "Tela piscando ou apresentando falhas na imagem.",
        laudo: "Durante os testes, o display apresentou oscilações e falhas intermitentes na imagem. Recomenda-se análise do conjunto da tela, conexões internas e circuito de imagem."
    },
    {
        id: 5,
        categoria: "TELA / TOUCH",
        problema: "Tela com manchas pretas, coloridas ou esbranquiçadas.",
        laudo: "Foram identificadas manchas e alterações na imagem do display, indicando comprometimento do conjunto da tela. Recomenda-se a substituição da peça após confirmação técnica."
    },
    {
        id: 6,
        categoria: "TELA / TOUCH",
        problema: "Tela apresentando linhas verticais ou horizontais.",
        laudo: "Durante os testes, foram constatadas linhas permanentes/intermitentes no display, comprometendo a exibição da imagem. Recomenda-se análise e possível substituição do conjunto da tela."
    },
    {
        id: 7,
        categoria: "TELA / TOUCH",
        problema: "Tela com cores alteradas ou imagem distorcida.",
        laudo: "O display apresenta alteração na reprodução das cores e/ou distorção da imagem. Será necessária análise do conjunto da tela e do circuito responsável pela geração da imagem."
    },
    {
        id: 8,
        categoria: "TELA / TOUCH",
        problema: "Touch não funciona em algumas partes da tela.",
        laudo: "Durante os testes, foram identificadas áreas da tela sem resposta aos comandos de toque. Recomenda-se a substituição do conjunto responsável pelo touch após confirmação do diagnóstico."
    },
    {
        id: 9,
        categoria: "TELA / TOUCH",
        problema: "Touch parou de funcionar completamente.",
        laudo: "Durante os testes, não foi identificada resposta aos comandos de toque. Será necessária análise do conjunto da tela, conexões e circuito do touch."
    },
    {
        id: 10,
        categoria: "TELA / TOUCH",
        problema: "Toque fantasma na tela.",
        laudo: "Durante os testes, foram identificados comandos involuntários na tela, ocorrendo sem interação do usuário. Recomenda-se análise do conjunto do touch, conexões e possíveis interferências no funcionamento."
    },
    {
        id: 11,
        categoria: "TELA / TOUCH",
        problema: "Tela travada, sem permitir deslizar ou digitar.",
        laudo: "Foi constatada ausência ou falha na resposta aos comandos de toque, impossibilitando a utilização normal do aparelho. Recomenda-se análise do touch e do sistema operacional para determinar a origem da falha."
    },
    {
        id: 12,
        categoria: "TELA / TOUCH",
        problema: "Brilho da tela muito baixo, mesmo configurado no máximo.",
        laudo: "Durante os testes, foi constatada baixa luminosidade do display mesmo com o brilho configurado em nível elevado. Recomenda-se análise da tela, sensor de luminosidade e circuito de iluminação."
    },
    {
        id: 13,
        categoria: "TELA / TOUCH",
        problema: "Tela apaga ou acende sozinha durante o uso.",
        laudo: "Durante os testes, foram observadas oscilações no funcionamento da tela. Recomenda-se análise do sensor de proximidade, sistema, conexões e conjunto da tela."
    },
    {
        id: 14,
        categoria: "TELA / TOUCH",
        problema: "Tela acende, mas não apresenta imagem.",
        laudo: "O display emite iluminação de fundo (backlight), porém não apresenta imagem gráfica. Recomenda-se análise de conexões do cabo flexível, display e circuito de processamento gráfico."
    },

    // BATERIA / CARREGAMENTO
    {
        id: 15,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Aparelho não apresenta sinal de carregamento ou não carrega.",
        laudo: "Durante os testes, o aparelho não apresentou resposta ao ser conectado a uma fonte de alimentação compatível. Recomenda-se análise do conector de carga, bateria e circuito de alimentação."
    },
    {
        id: 16,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Bateria descarrega muito rápido / pouca duração.",
        laudo: "Durante os testes, foi identificado consumo elevado de bateria em relação ao funcionamento normal esperado. Recomenda-se avaliação da saúde da bateria e do consumo energético do aparelho."
    },
    {
        id: 17,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Celular desliga mesmo indicando carga na bateria.",
        laudo: "Foi constatado desligamento inesperado mesmo com indicação de carga disponível. Recomenda-se análise da bateria, alimentação e sistema para identificação da causa."
    },
    {
        id: 18,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Aparelho apresenta carregamento lento.",
        laudo: "Durante os testes, o aparelho apresentou velocidade de carregamento abaixo do esperado. Recomenda-se análise do carregador, cabo, conector de carga, bateria e circuito de alimentação."
    },
    {
        id: 19,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Cabo de carregamento precisa ficar em determinada posição para carregar.",
        laudo: "Foi constatada instabilidade na conexão durante o carregamento, ocorrendo funcionamento apenas em determinadas posições do cabo. Recomenda-se inspeção e possível reparo ou substituição do conector de carga."
    },
    {
        id: 20,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Conector de carga está frouxo ou com mau contato.",
        laudo: "Durante a inspeção, foi identificada folga e/ou instabilidade no conector de carga, comprometendo a conexão adequada do cabo. Recomenda-se reparo ou substituição do componente."
    },
    {
        id: 21,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Aparelho indica que está carregando, porém a porcentagem da bateria não aumenta.",
        laudo: "O aparelho reconhece a conexão do carregador, porém não apresenta evolução adequada no percentual da bateria. Recomenda-se análise da bateria e do circuito de carga."
    },
    {
        id: 22,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Porcentagem da bateria fica travada ou apresenta valores incorretos.",
        laudo: "Durante os testes, foram observadas inconsistências na indicação do nível de bateria. Recomenda-se análise da saúde da bateria, calibração e sistema de gerenciamento de energia."
    },
    {
        id: 23,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Celular esquenta muito durante o carregamento.",
        laudo: "Foi constatado aquecimento durante o processo de carga. Recomenda-se análise da bateria, conector, carregador utilizado e circuito de alimentação para verificar a origem do aquecimento."
    },
    {
        id: 24,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Bateria estufada.",
        laudo: "Durante a inspeção, foi constatado aumento físico do volume da bateria, causando pressão e/ou abertura da estrutura do aparelho. Recomenda-se interromper o uso e realizar a substituição da bateria."
    },
    {
        id: 25,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Aparelho não reconhece carregador ou cabo USB.",
        laudo: "Durante os testes, o aparelho não reconheceu corretamente a conexão USB. Recomenda-se análise do conector, cabo utilizado e circuito responsável pela comunicação e carregamento."
    },
    {
        id: 26,
        categoria: "BATERIA / CARREGAMENTO",
        problema: "Carregamento por indução (sem fio) não funciona.",
        laudo: "Durante os testes com carregador compatível, o aparelho não iniciou o carregamento por indução. Recomenda-se análise da bobina de carregamento e do circuito responsável pela função."
    },

    // REDE / CHIP / SINAL
    {
        id: 27,
        categoria: "REDE / CHIP / SINAL",
        problema: "Celular não reconhece o chip (SIM).",
        laudo: "Durante os testes, o aparelho não realizou a leitura do cartão SIM. Recomenda-se análise do chip, bandeja, leitor SIM e circuito responsável pela comunicação com a rede."
    },
    {
        id: 28,
        categoria: "REDE / CHIP / SINAL",
        problema: "Aparelho apresenta mensagem 'Sem Serviço' ou 'Sem Sinal'.",
        laudo: "Foi constatada ausência de conexão com a rede da operadora. Recomenda-se análise utilizando chip funcional, além da verificação de antenas e circuito de rede."
    },
    {
        id: 29,
        categoria: "REDE / CHIP / SINAL",
        problema: "Sinal da operadora fica caindo constantemente.",
        laudo: "Durante os testes, foi identificada instabilidade na conexão com a rede móvel. Recomenda-se análise das antenas, conexões internas, chip e circuito de radiofrequência."
    },
    {
        id: 30,
        categoria: "REDE / CHIP / SINAL",
        problema: "Celular reconhece o chip, mas não realiza ligações.",
        laudo: "O aparelho reconhece o cartão SIM, porém apresenta falha ao realizar chamadas. Recomenda-se verificar configurações, operadora e componentes relacionados à rede móvel."
    },
    {
        id: 31,
        categoria: "REDE / CHIP / SINAL",
        problema: "Não consegue receber ligações.",
        laudo: "Durante os testes, foram identificadas dificuldades no recebimento de chamadas. Recomenda-se análise das configurações de rede, chip e comunicação com a operadora."
    },
    {
        id: 32,
        categoria: "REDE / CHIP / SINAL",
        problema: "Internet móvel não funciona.",
        laudo: "O aparelho apresenta falha no acesso aos dados móveis. Recomenda-se verificar configurações de rede/APN, chip, operadora e circuito de comunicação."
    },
    {
        id: 33,
        categoria: "REDE / CHIP / SINAL",
        problema: "Segundo chip (SIM 2) não funciona.",
        laudo: "Durante os testes, o segundo slot SIM não realizou corretamente a leitura ou comunicação com a rede. Recomenda-se análise do leitor SIM e circuito correspondente."
    },
    {
        id: 34,
        categoria: "REDE / CHIP / SINAL",
        problema: "Aparelho não reconhece eSIM ou não consegue ativar o eSIM.",
        laudo: "Foi identificada falha na ativação ou utilização do eSIM. Recomenda-se verificar compatibilidade, configurações, situação junto à operadora e funcionamento do sistema."
    },
    {
        id: 35,
        categoria: "REDE / CHIP / SINAL",
        problema: "Aparelho apresenta somente chamadas de emergência.",
        laudo: "O dispositivo não consegue registrar-se na rede celular para chamadas comuns. Recomenda-se checagem de status junto a operadora, verificação de leitor SIM e integridade do modulo de RF."
    },

    // WI-FI / BLUETOOTH / GPS / NFC
    {
        id: 36,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Wi-Fi não liga.",
        laudo: "Durante os testes, não foi possível ativar a função Wi-Fi. Recomenda-se análise do sistema e do circuito responsável pela conectividade sem fio."
    },
    {
        id: 37,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Wi-Fi liga, mas não encontra nenhuma rede.",
        laudo: "A função Wi-Fi é ativada, porém não localiza redes disponíveis corretamente. Recomenda-se análise da antena, conexões e circuito Wi-Fi."
    },
    {
        id: 38,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Wi-Fi conecta e desconecta sozinho.",
        laudo: "Durante os testes, foi constatada instabilidade na conexão Wi-Fi. Recomenda-se análise das configurações, antenas e circuito de conectividade."
    },
    {
        id: 39,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Sinal do Wi-Fi está muito fraco.",
        laudo: "Foi identificado baixo nível de recepção do sinal Wi-Fi em comparação com outros dispositivos utilizados no mesmo ambiente. Recomenda-se análise da antena e conexões internas."
    },
    {
        id: 40,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Bluetooth não liga ou não encontra outros dispositivos.",
        laudo: "Durante os testes, a função Bluetooth apresentou falha de ativação e/ou comunicação com outros dispositivos. Recomenda-se análise do sistema e circuito de conectividade."
    },
    {
        id: 41,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "GPS não localiza corretamente a posição / posição incorreta.",
        laudo: "Durante os testes, foram identificadas falhas ou baixa precisão na localização do aparelho. Recomenda-se análise das configurações, antena e sistema de localização."
    },
    {
        id: 42,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "NFC ou pagamento por aproximação não funciona.",
        laudo: "Durante os testes, a função NFC não realizou comunicação corretamente com dispositivos compatíveis. Recomenda-se análise da antena NFC, configurações e circuito correspondente."
    },
    {
        id: 43,
        categoria: "WI-FI / BLUETOOTH / GPS / NFC",
        problema: "Roteador Wi-Fi / compartilhamento de internet não funciona.",
        laudo: "Durante os testes, o modo de ponto de acesso (hotspot) apresentou falha ao transmitir sinal para outros dispositivos. Recomenda-se restauracao de ajustes de rede e testes no circuito integrado de radio Wi-Fi."
    },

    // AUDIO / MICROFONE
    {
        id: 44,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Aparelho não reproduz som em vídeos, músicas ou aplicativos.",
        laudo: "Durante os testes, não foi identificada reprodução de áudio pelo alto-falante. Recomenda-se análise do componente, conexões e circuito de áudio."
    },
    {
        id: 45,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Alto-falante apresenta som baixo, chiado ou distorcido.",
        laudo: "Foi constatada alteração na qualidade e/ou intensidade do áudio reproduzido. Recomenda-se inspeção e limpeza técnica, seguida de análise do alto-falante e circuito de áudio."
    },
    {
        id: 46,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Durante ligações, não é possível ouvir a outra pessoa (auricular).",
        laudo: "Durante os testes, foi identificada falha na reprodução do áudio de chamadas pelo alto-falante auricular. Recomenda-se análise do auricular, conexões e circuito de áudio."
    },
    {
        id: 47,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Som funciona somente no viva-voz.",
        laudo: "O áudio das chamadas é reproduzido pelo viva-voz, porém apresenta falha no modo convencional. Recomenda-se análise do alto-falante auricular e circuito relacionado."
    },
    {
        id: 48,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Microfone não funciona.",
        laudo: "Durante os testes, não foi identificada captação adequada de áudio pelo microfone. Recomenda-se análise do componente, entradas de áudio e circuito correspondente."
    },
    {
        id: 49,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Microfone apresenta som muito baixo, chiados ou falhas.",
        laudo: "A captação de áudio apresentou baixo volume e/ou ruídos durante os testes. Recomenda-se limpeza técnica e análise do microfone e circuito de áudio."
    },
    {
        id: 50,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Microfone não funciona em áudios do WhatsApp.",
        laudo: "Durante os testes de gravação, foi identificada falha na captação de áudio. Recomenda-se verificar permissões do aplicativo, microfones secundários e sistema de áudio do aparelho."
    },
    {
        id: 51,
        categoria: "ÁUDIO / MICROFONE",
        problema: "Fone de ouvido não é reconhecido ou permanece indicando fone conectado.",
        laudo: "Durante os testes, o aparelho não reconheceu corretamente o dispositivo de áudio conectado ou manteve o conector em curto. Recomenda-se análise do conector jack/conector de carga e circuito de áudio."
    },

    // CAMERAS / FLASH
    {
        id: 52,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera frontal não funciona.",
        laudo: "Durante os testes, a câmera frontal não apresentou imagem e/ou funcionamento adequado. Recomenda-se análise do módulo da câmera, conexões e circuito correspondente."
    },
    {
        id: 53,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera traseira não funciona.",
        laudo: "Durante os testes, a câmera traseira não apresentou imagem e/ou funcionamento adequado. Recomenda-se análise do módulo da câmera, conexões e circuito correspondente."
    },
    {
        id: 54,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera fica escura.",
        laudo: "Ao acessar a câmera, não foi apresentada imagem corretamente. Recomenda-se análise do módulo da câmera, conexões e sistema."
    },
    {
        id: 55,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera apresenta imagem embaçada, manchas ou pontos.",
        laudo: "Durante os testes, foi constatada perda de nitidez nas imagens capturadas. Recomenda-se inspeção da lente, limpeza e análise do módulo da câmera."
    },
    {
        id: 56,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera não consegue focar.",
        laudo: "Foi identificada falha no ajuste automático de foco durante os testes. Recomenda-se análise do mecanismo de foco e módulo da câmera."
    },
    {
        id: 57,
        categoria: "CÂMERAS / FLASH",
        problema: "Câmera fica tremendo durante o uso.",
        laudo: "Durante os testes, foram observadas oscilações anormais na imagem da câmera. Recomenda-se análise do sistema de estabilização óptica (OIS) e módulo da câmera."
    },
    {
        id: 58,
        categoria: "CÂMERAS / FLASH",
        problema: "Aplicativo da câmera fecha sozinho ou apresenta mensagem de erro.",
        laudo: "Durante os testes, o aplicativo da câmera apresentou encerramentos inesperados. Recomenda-se análise do sistema, aplicativos e módulos das câmeras."
    },
    {
        id: 59,
        categoria: "CÂMERAS / FLASH",
        problema: "Flash da câmera ou lanterna não funciona.",
        laudo: "Durante os testes, o LED de flash/lanterna não apresentou funcionamento adequado. Recomenda-se análise do componente e circuito responsável."
    },

    // BOTOES / SENSORES
    {
        id: 60,
        categoria: "BOTÕES / SENSORES",
        problema: "Botão Power está afundado, com mau contato ou não funciona.",
        laudo: "Durante os testes, o botão de ligar/desligar não respondeu corretamente aos comandos. Recomenda-se análise do botão, cabo flexível e conexões."
    },
    {
        id: 61,
        categoria: "BOTÕES / SENSORES",
        problema: "Botões de volume afundados, com mau contato ou não funcionam.",
        laudo: "Foi constatada falha no acionamento dos comandos de volume. Recomenda-se análise dos botões, cabo flexível e conexões."
    },
    {
        id: 62,
        categoria: "BOTÕES / SENSORES",
        problema: "Botão Home não funciona.",
        laudo: "Durante os testes, o botão Home não respondeu corretamente aos comandos. Recomenda-se análise do componente e suas conexões."
    },
    {
        id: 63,
        categoria: "BOTÕES / SENSORES",
        problema: "Sensor de proximidade não funciona e tela permanece ligada durante as chamadas.",
        laudo: "Durante as chamadas, o sensor de proximidade não apresentou funcionamento adequado no controle da tela. Recomenda-se análise do sensor, posicionamento e conexões."
    },
    {
        id: 64,
        categoria: "BOTÕES / SENSORES",
        problema: "Rotação automática da tela não funciona.",
        laudo: "Durante os testes, a orientação da tela não foi alterada automaticamente conforme a posição do aparelho. Recomenda-se análise dos sensores de movimento (giroscópio/acelerômetro) e configurações do sistema."
    },
    {
        id: 65,
        categoria: "BOTÕES / SENSORES",
        problema: "Vibração não funciona ou aparelho fica vibrando sozinho.",
        laudo: "Durante os testes, não foi identificado funcionamento adequado do sistema de vibração. Recomenda-se análise do motor de vibração e circuito correspondente."
    },
    {
        id: 66,
        categoria: "BOTÕES / SENSORES",
        problema: "Chave/botão de silencioso não funciona.",
        laudo: "O mecanismo comutador de silencioso não altera o perfil de som. Recomenda-se inspeção física do seletor mecânico e do cabo flexível lateral."
    },

    // BIOMETRIA / RECONHECIMENTO FACIAL
    {
        id: 67,
        categoria: "BIOMETRIA / RECONHECIMENTO FACIAL",
        problema: "Leitor de impressão digital não funciona ou não reconhece digital.",
        laudo: "Durante os testes, o sensor biométrico não realizou corretamente a leitura da impressão digital. Recomenda-se análise do sensor, conexões e configurações do sistema."
    },
    {
        id: 68,
        categoria: "BIOMETRIA / RECONHECIMENTO FACIAL",
        problema: "Não consegue cadastrar nova impressão digital.",
        laudo: "O aparelho apresentou falha durante o processo de cadastramento biométrico. Recomenda-se análise do sensor e do sistema responsável pela função."
    },
    {
        id: 69,
        categoria: "BIOMETRIA / RECONHECIMENTO FACIAL",
        problema: "Face ID não funciona ou apresenta mensagem de indisponível.",
        laudo: "Durante os testes, o sistema Face ID não realizou o reconhecimento facial corretamente. Recomenda-se diagnóstico dos sensores e componentes relacionados ao sistema TrueDepth."
    },
    {
        id: 70,
        categoria: "BIOMETRIA / RECONHECIMENTO FACIAL",
        problema: "Reconhecimento facial não funciona.",
        laudo: "Foi constatada falha no reconhecimento facial durante os testes. Recomenda-se análise da câmera, sensores e configurações responsáveis pela função."
    },

    // SISTEMA / SOFTWARE
    {
        id: 71,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Celular está muito lento ou travando.",
        laudo: "Durante os testes, foi identificado baixo desempenho na execução de funções e aplicativos. Recomenda-se análise do armazenamento, aplicativos instalados, sistema operacional e condições gerais do aparelho."
    },
    {
        id: 72,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Aplicativos fecham sozinhos ou apresentam mensagens de erro constantemente.",
        laudo: "Durante os testes, foram constatados encerramentos inesperados de aplicativos. Recomenda-se análise do sistema, armazenamento, atualizações e integridade dos aplicativos."
    },
    {
        id: 73,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Aplicativos não abrem / Play Store ou App Store não baixa apps.",
        laudo: "Foi constatada falha na inicialização de aplicativos. Recomenda-se verificar armazenamento disponível, atualizações, permissões e integridade do sistema."
    },
    {
        id: 74,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Celular reinicia sozinho durante o uso.",
        laudo: "Durante os testes, o aparelho apresentou reinicializações inesperadas. A falha pode estar relacionada ao sistema, bateria, alimentação ou componentes internos, sendo necessário diagnóstico complementar."
    },
    {
        id: 75,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Aparelho fica travado na tela da marca ao ligar (bootloop).",
        laudo: "O aparelho inicia o processo de inicialização, porém permanece travado na logomarca. Recomenda-se análise do sistema operacional, armazenamento e componentes responsáveis pela inicialização."
    },
    {
        id: 76,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Aparelho fica preso em um ciclo de reinicialização contínua.",
        laudo: "Foi constatado ciclo contínuo de reinicialização, impedindo a inicialização completa do sistema. Recomenda-se análise de software, bateria, alimentação e armazenamento interno."
    },
    {
        id: 77,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Celular não liga ou não dá sinal de vida.",
        laudo: "Durante os testes iniciais, o aparelho não apresentou sinais normais de inicialização. Recomenda-se análise da bateria, circuito de alimentação, botão Power e placa principal."
    },
    {
        id: 78,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Aparelho travou ou apresentou erro após atualização do sistema.",
        laudo: "O aparelho apresenta falha de inicialização ou funcionamento após atualização do sistema. Recomenda-se análise da integridade do software e, quando aplicável, recuperação ou reinstalação do sistema."
    },
    {
        id: 79,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Celular apresenta propagandas indesejadas constantemente / possível vírus.",
        laudo: "Durante a análise, foram identificados comportamentos anormais e/ou aplicativos potencialmente indesejados responsáveis pela exibição excessiva de propagandas. Recomenda-se limpeza do sistema, remoção dos aplicativos suspeitos e análise de segurança."
    },
    {
        id: 80,
        categoria: "SISTEMA / SOFTWARE",
        problema: "Sem espaço de armazenamento disponível no celular.",
        laudo: "Foi constatado baixo espaço disponível no armazenamento interno, podendo causar lentidão e falhas em aplicativos. Recomenda-se limpeza de arquivos desnecessários e gerenciamento do armazenamento."
    },

    // BLOQUEIOS / CONTAS
    {
        id: 81,
        categoria: "BLOQUEIOS / CONTAS",
        problema: "Cliente esqueceu a senha, PIN ou padrão de desbloqueio.",
        laudo: "O aparelho encontra-se protegido por bloqueio de tela e o cliente informa não possuir a credencial de acesso. Qualquer procedimento deverá respeitar os mecanismos de segurança do fabricante e poderá exigir comprovação de propriedade."
    },
    {
        id: 82,
        categoria: "BLOQUEIOS / CONTAS",
        problema: "Aparelho solicita a conta Google após formatação (FRP).",
        laudo: "O aparelho apresenta proteção de redefinição de fábrica (FRP), solicitando autenticação da conta Google anteriormente vinculada. Recomenda-se recuperação da conta pelos meios oficiais do Google/fabricante mediante comprovação de propriedade."
    },
    {
        id: 83,
        categoria: "BLOQUEIOS / CONTAS",
        problema: "iPhone apresenta Bloqueio de Ativação do iCloud / esqueceu ID Apple.",
        laudo: "O aparelho apresenta Bloqueio de Ativação vinculado a um ID Apple. A liberação deverá ser realizada pelo proprietário utilizando as credenciais da conta ou pelos procedimentos oficiais disponibilizados pela Apple mediante comprovação de propriedade."
    },
    {
        id: 84,
        categoria: "BLOQUEIOS / CONTAS",
        problema: "iPhone apresenta mensagem 'iPhone Indisponível'.",
        laudo: "O aparelho encontra-se na condição 'iPhone Indisponível' devido às tentativas incorretas de código de acesso. A recuperação deverá seguir os procedimentos oficiais da Apple e poderá exigir restauração do dispositivo e autenticação do ID Apple vinculado."
    },
    {
        id: 85,
        categoria: "BLOQUEIOS / CONTAS",
        problema: "Aparelho bloqueado por aplicativo financeiro / PayJoy / financiamento.",
        laudo: "Foi identificado bloqueio vinculado a serviço/aplicativo financeiro instalado ou associado ao aparelho. A regularização deverá ser realizada diretamente com a empresa responsável pelo bloqueio ou instituição financeira correspondente."
    },

    // DANOS FISICOS / LIQUIDO
    {
        id: 86,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Celular caiu e não liga mais.",
        laudo: "O aparelho apresenta histórico de impacto e não inicializa normalmente. Recomenda-se inspeção interna para verificar possíveis danos em tela, bateria, conexões, placa principal e demais componentes."
    },
    {
        id: 87,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Celular caiu e a tela ficou sem imagem.",
        laudo: "Após o impacto relatado, o aparelho apresenta ausência de imagem. Recomenda-se análise do conjunto da tela, conectores e placa principal para identificação dos danos."
    },
    {
        id: 88,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Aparelho entrou em contato com água ou outro líquido.",
        laudo: "O aparelho possui relato de contato com líquido. Recomenda-se abertura e inspeção interna imediata para identificação de umidade, oxidação e possíveis danos aos componentes eletrônicos."
    },
    {
        id: 89,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Carcaça está quebrada, trincada ou empenada.",
        laudo: "Durante a inspeção visual, foram identificados danos estruturais na carcaça do aparelho. Recomenda-se substituição/reparo das peças afetadas e avaliação de possíveis danos internos decorrentes do impacto."
    },
    {
        id: 90,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Tampa traseira está quebrada ou trincada.",
        laudo: "Foi constatado dano físico na tampa traseira, comprometendo a integridade externa do aparelho. Recomenda-se substituição da peça."
    },
    {
        id: 91,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Tampa traseira está soltando / descolando.",
        laudo: "Foi constatado descolamento da tampa traseira. Recomenda-se inspeção interna antes da recolagem, principalmente para verificar possível expansão da bateria ou deformação estrutural."
    },
    {
        id: 92,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Lente da câmera está quebrada ou trincada.",
        laudo: "Durante a inspeção visual, foi constatado dano físico na lente/proteção externa da câmera. Recomenda-se substituição da peça e avaliação do módulo da câmera."
    },
    {
        id: 93,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Bandeja do chip está quebrada ou presa.",
        laudo: "Foi identificado dano e/ou travamento da bandeja do cartão SIM. Recomenda-se remoção técnica e inspeção do leitor para evitar danos adicionais."
    },
    {
        id: 94,
        categoria: "DANOS FÍSICOS / LÍQUIDO",
        problema: "Aparelho apresenta sinais de oxidação.",
        laudo: "Durante a inspeção interna, foram identificados sinais de oxidação em componentes do aparelho. Recomenda-se limpeza técnica e avaliação individual dos circuitos afetados, não sendo possível garantir previamente a recuperação completa."
    },

    // DESEMPENHO / OUTROS
    {
        id: 95,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Celular esquenta excessivamente durante o uso ou carregamento.",
        laudo: "Durante os testes, foi identificado aquecimento acima do comportamento esperado. Recomenda-se análise da bateria, consumo do sistema e componentes da placa para identificação da origem."
    },
    {
        id: 96,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Computador não reconhece o celular pelo cabo USB ou não transfere arquivos.",
        laudo: "Durante os testes de conexão USB, o aparelho não foi reconhecido corretamente pelo computador. Recomenda-se análise do cabo, conector USB, configurações e circuito de comunicação."
    },
    {
        id: 97,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Cartão de memória não é reconhecido pelo aparelho.",
        laudo: "Durante os testes, o aparelho não realizou corretamente a leitura do cartão de memória. Recomenda-se testar outro cartão compatível e analisar o leitor e suas conexões."
    },
    {
        id: 98,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Notificações não aparecem ou não emitem som.",
        laudo: "Durante os testes, foram identificadas alterações no funcionamento das notificações. Recomenda-se análise das configurações de som, permissões dos aplicativos e sistema operacional."
    },
    {
        id: 99,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Despertador não toca.",
        laudo: "Durante os testes, foi identificada falha relacionada à reprodução ou configuração dos alarmes. Recomenda-se verificar volume, permissões, configurações e funcionamento do sistema."
    },
    {
        id: 100,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Aparelho apresenta data ou horário incorretos.",
        laudo: "Foi constatada inconsistência na data e/ou horário do aparelho. Recomenda-se verificar configurações automáticas de rede, fuso horário e funcionamento do sistema."
    },
    {
        id: 101,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Cliente deseja realizar backup dos dados.",
        laudo: "Solicitado procedimento de backup dos dados disponíveis no aparelho. A execução dependerá das condições de funcionamento do dispositivo, disponibilidade dos dados e acesso autorizado às contas do cliente."
    },
    {
        id: 102,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Cliente deseja transferir os dados para outro aparelho.",
        laudo: "Solicitada transferência dos dados disponíveis para outro dispositivo. O procedimento dependerá da compatibilidade entre os aparelhos, funcionamento do dispositivo de origem e credenciais necessárias."
    },
    {
        id: 103,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Cliente deseja restaurar ou formatar o aparelho para os padrões de fábrica.",
        laudo: "Solicitada restauração do aparelho para as configurações de fábrica. O procedimento poderá apagar permanentemente os dados armazenados e poderá exigir posteriormente as credenciais das contas anteriormente vinculadas."
    },
    {
        id: 104,
        categoria: "DESEMPENHO / OUTROS",
        problema: "Cliente deseja atualizar o sistema operacional.",
        laudo: "Solicitada atualização do sistema operacional. Será verificada a disponibilidade de versão compatível e as condições necessárias para realização do procedimento."
    }
];
