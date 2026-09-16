let isAbertura = true;

window.loadData = function (abertura) {
    isAbertura = abertura;
    document.getElementById("pageTitle").innerText = isAbertura ? "Abertura de Caixa" : "Fechamento de Caixa";
    document.getElementById("lblDescricao").innerText = isAbertura ? "Informe o valor do fundo de troco (R$):" : "Informe o valor em gaveta para fechamento (R$):";
};

document.getElementById('valor').addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');
    value = (value / 100).toFixed(2) + '';
    value = value.replace(".", ",");
    value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    e.target.value = value;
});

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const valorStr = document.getElementById("valor").value;
    const valor = parseFloat(valorStr.replace(/\./g, '').replace(',', '.'));
    
    if (isNaN(valor)) {
        alert("Por favor, digite um valor válido.");
        return;
    }
    
    window.chrome.webview.postMessage({ action: "SAVE", valor: valor });
});

