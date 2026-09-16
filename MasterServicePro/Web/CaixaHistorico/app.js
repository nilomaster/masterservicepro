window.loadData = function (data) {
    const tbody = document.getElementById("tbBody");
    tbody.innerHTML = "";
    
    if (data.length === 0) {
        tbody.innerHTML = "<tr><td colspan='5' style='text-align:center;'>Nenhum histórico encontrado.</td></tr>";
        return;
    }

    data.forEach(item => {
        const tr = document.createElement("tr");
        
        const formatMoney = (val) => new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(val);
        const formatDt = (dt) => new Date(dt).toLocaleString('pt-BR');

        tr.innerHTML = `
            <td>${item.Id}</td>
            <td>${formatDt(item.DataAbertura)}</td>
            <td>${formatDt(item.DataFechamento)}</td>
            <td>${formatMoney(item.ValorAbertura)}</td>
            <td>${formatMoney(item.ValorFechamento)}</td>
        `;
        
        tr.addEventListener("click", () => {
            window.chrome.webview.postMessage({ action: "SELECT", id: item.Id });
        });
        
        tbody.appendChild(tr);
    });
};

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

