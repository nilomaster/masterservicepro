// app.js

let pieChartInstance = null;
let currentData = [];

// Formatação
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value || 0);
}

function formatDate(dateString) {
    if (!dateString) return "-";
    const d = new Date(dateString);
    return d.toLocaleDateString('pt-BR');
}

Chart.defaults.color = '#94A3B8';
Chart.defaults.font.family = "'Inter', sans-serif";

function renderPieChart(valRecebido, valPendente, valAtrasado) {
    const ctx = document.getElementById('pieChart').getContext('2d');
    
    if (pieChartInstance) {
        pieChartInstance.destroy();
    }

    pieChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Recebido (Baixado)', 'A Receber (No Prazo)', 'Inadimplência (Atrasadas)'],
            datasets: [{
                data: [valRecebido, valPendente, valAtrasado],
                backgroundColor: [
                    '#22C55E', // Green
                    '#F59E0B', // Yellow
                    '#EF4444'  // Red
                ],
                borderWidth: 0,
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { padding: 20 }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + formatCurrency(context.raw);
                        }
                    }
                }
            }
        }
    });
}

function renderTable(data) {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '';

    data.forEach(item => {
        const tr = document.createElement('tr');

        let statusClass = '';
        if (item.Status === 'Pago') statusClass = 'status-pago';
        else if (item.Status === 'Atrasado') statusClass = 'status-atrasado';
        else statusClass = 'status-pendente';

        const btnHtml = item.Status !== 'Pago' 
            ? `<button class="btn-neon btn-success btn-sm" onclick="receber(${item.Id})">RECEBER</button>`
            : `<span style="color:var(--text-secondary); font-size:12px;">Concluído</span>`;

        tr.innerHTML = `
            <td><strong>${item.ClienteNome}</strong></td>
            <td>${item.Descricao}</td>
            <td>${formatCurrency(item.ValorTotal)}</td>
            <td>${formatCurrency(item.ValorPago)}</td>
            <td><strong>${formatCurrency(item.ValorRestante)}</strong></td>
            <td>${formatDate(item.DataVencimento)}</td>
            <td><span class="status-badge ${statusClass}">${item.Status}</span></td>
            <td>${btnHtml}</td>
        `;
        tbody.appendChild(tr);
    });
}

function loadClientes(clientes) {
    const cbo = document.getElementById('cboCliente');
    cbo.innerHTML = '<option value="0">-- Todos os Clientes --</option>';
    clientes.forEach(c => {
        const opt = document.createElement('option');
        opt.value = c.Id;
        opt.textContent = c.Nome;
        cbo.appendChild(opt);
    });
}

function applyFilters() {
    const clienteId = parseInt(document.getElementById('cboCliente').value);
    const status = document.getElementById('cboStatus').value;

    let filtered = currentData;

    if (clienteId > 0) {
        filtered = filtered.filter(x => x.IdCliente === clienteId);
    }
    
    if (status !== 'Todos') {
        filtered = filtered.filter(x => x.Status === status);
    }

    renderTable(filtered);
}

// Comunicação com C#
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        try {
            const msg = typeof event.data === 'string' ? JSON.parse(event.data) : event.data;
            
            if (msg.action === 'load_data') {
                currentData = msg.data;
                
                document.getElementById('valPendente').innerText = formatCurrency(msg.valPendente);
                document.getElementById('valAtrasado').innerText = formatCurrency(msg.valAtrasado);
                document.getElementById('valRecebido').innerText = formatCurrency(msg.valRecebido);

                if (msg.clientes) {
                    loadClientes(msg.clientes);
                }

                renderPieChart(msg.valRecebido, msg.valPendente, msg.valAtrasado);
                applyFilters();
            }
        } catch(e) { console.error(e); }
    });
}

function novoLancamento() {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'novo_lancamento' });
    }
}

function receber(id) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'registrar_recebimento', id: id });
    }
}

function fechar() {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'fechar' });
    }
}
