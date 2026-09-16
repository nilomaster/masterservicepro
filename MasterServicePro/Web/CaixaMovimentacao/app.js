let tipoAtual = "Saída"; // ou "Entrada"

// Inicialização: chamado pelo C# quando carregar
window.loadData = function (tipo) {
    tipoAtual = tipo;
    
    document.getElementById("pageTitle").innerText = tipo === "Suprimento" ? "Registrar Adicionar Dinheiro" : "Registrar Retirar Dinheiro";
    
    const subcategoria = document.getElementById("subcategoria");
    subcategoria.innerHTML = "";
    
    const btnConfirm = document.getElementById("btnConfirm");
    
    if (tipo === "Suprimento") {
        const opcoes = ["Aporte de Caixa", "Troco Inicial", "Venda Direta", "Outros"];
        opcoes.forEach(op => {
            const el = document.createElement("option");
            el.value = op;
            el.innerText = op;
            subcategoria.appendChild(el);
        });
        btnConfirm.className = "btn btn-primary success";
    } else {
        const opcoes = [
            "Despesa com Peças", 
            "Aluguel / Condomínio", 
            "Pró-labore / Salários", 
            "Água / Luz / Internet", 
            "Material de Escritório", 
            "Ferramentas / Equipamentos", 
            "Estorno/Devolução", 
            "Outros"
        ];
        opcoes.forEach(op => {
            const el = document.createElement("option");
            el.value = op;
            el.innerText = op;
            subcategoria.appendChild(el);
        });
        btnConfirm.className = "btn btn-primary danger";
    }
};

// Mask para dinheiro
document.getElementById('valor').addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');
    value = (value / 100).toFixed(2) + '';
    value = value.replace(".", ",");
    value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    e.target.value = value;
});

// Ações
document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const valorStr = document.getElementById("valor").value;
    const valor = parseFloat(valorStr.replace(/\./g, '').replace(',', '.'));
    
    if (isNaN(valor) || valor <= 0) {
        alert("Por favor, digite um valor maior que zero.");
        return;
    }
    
    const descricao = document.getElementById("descricao").value.trim();
    if (descricao === "") {
        alert("Informe o motivo ou observação.");
        return;
    }
    
    const data = {
        FormaPagamento: document.getElementById("formaPagamento").value,
        Subcategoria: document.getElementById("subcategoria").value,
        Valor: valor,
        Descricao: descricao
    };
    
    window.chrome.webview.postMessage({ action: "SAVE", data: data });
});

