// Integração com WebView2
function sendToHost(message) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage(message);
    } else {
        console.log('DEV MSG:', message);
    }
}

// Receber mensagens do C#
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        try {
            const data = JSON.parse(event.data);
            if (data.action === "load_data") {
                renderTable(data.entradas);
            }
        } catch (e) {
            console.error("Erro ao fazer parse da mensagem:", e);
        }
    });
}

function formatDate(dateString) {
    if (!dateString) return "-";
    const d = new Date(dateString);
    return d.toLocaleDateString('pt-BR') + ' ' + d.toLocaleTimeString('pt-BR', {hour: '2-digit', minute:'2-digit'});
}

function formatMoney(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value || 0);
}

function renderTable(entradas) {
    const tbody = document.getElementById('tbody-entradas');
    const loading = document.getElementById('loading');
    const emptyState = document.getElementById('empty-state');
    
    loading.classList.add('hidden');
    
    if (!entradas || entradas.length === 0) {
        tbody.innerHTML = '';
        emptyState.classList.remove('hidden');
        return;
    }
    
    emptyState.classList.add('hidden');
    
    let html = '';
    entradas.forEach(item => {
        html += `
            <tr class="border-b border-slate-700/50 hover:bg-slate-700/30 transition-colors">
                <td class="py-3 px-4 text-slate-300">#${item.Id}</td>
                <td class="py-3 px-4 text-slate-200">${formatDate(item.DataEntrada)}</td>
                <td class="py-3 px-4 text-slate-200 font-medium">${item.FornecedorNome || '-'}</td>
                <td class="py-3 px-4 text-slate-400">${item.NumeroNota || '-'}</td>
                <td class="py-3 px-4 text-emerald-400 font-semibold text-right">${formatMoney(item.ValorTotal)}</td>
                <td class="py-3 px-4 text-slate-400 text-xs truncate max-w-[200px]" title="${item.Observacao || ''}">${item.Observacao || '-'}</td>
            </tr>
        `;
    });
    
    tbody.innerHTML = html;
}

function registrarNovaEntrada() {
    sendToHost({ action: 'nova_entrada' });
}

function fechar() {
    sendToHost({ action: 'fechar' });
}

// Request inicial de dados
document.addEventListener('DOMContentLoaded', () => {
    sendToHost({ action: 'request_data' });
});
