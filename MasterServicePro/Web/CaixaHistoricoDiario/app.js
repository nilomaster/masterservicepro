// Formata moeda
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

// Formata hora (espera string ISO ou Date)
function formatTime(dateString) {
    const d = new Date(dateString);
    return d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute:'2-digit' });
}

window.loadData = function(movimentacoes) {
    const tbody = document.querySelector("#tabelaHistorico tbody");
    tbody.innerHTML = "";

    if (!movimentacoes || movimentacoes.length === 0) {
        tbody.innerHTML = "<tr><td colspan='7' style='text-align:center;'>Nenhuma movimentação registrada hoje.</td></tr>";
        return;
    }

    movimentacoes.forEach(mov => {
        const tr = document.createElement("tr");
        
        const isEntrada = mov.Tipo === "Entrada";
        const valorClass = isEntrada ? "text-success" : "text-danger";
        
        tr.innerHTML = `
            <td>${mov.Tipo || ''}</td>
            <td>${mov.Categoria || ''}</td>
            <td>${mov.Subcategoria || ''}</td>
            <td>${mov.FormaPagamento || ''}</td>
            <td>${mov.Descricao || ''}</td>
            <td class="${valorClass}">${formatCurrency(mov.Valor)}</td>
            <td>${formatTime(mov.Data)}</td>
        `;
        
        tbody.appendChild(tr);
    });
};
