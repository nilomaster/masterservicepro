// Handle data loaded from C#
function loadData(data) {
    if (!data || !data.cliente) return;
    
    // Set Header
    document.getElementById('clientName').innerText = `HISTÓRICO: ${data.cliente.Nome.toUpperCase()}`;
    
    let doc = data.cliente.CpfCnpj ? ` | ${data.cliente.CpfCnpj}` : '';
    let tel = data.cliente.WhatsApp || data.cliente.Telefone || '';
    tel = tel ? ` | ${tel}` : '';
    document.getElementById('clientSubtitle').innerText = `Resumo de atendimentos e compras${doc}${tel}`;

    // Populate OS
    populateOS(data.historicoOS);
    
    // Populate Vendas
    populateVendas(data.historicoVendas);

    // Update Totals
    document.getElementById('totalOsCount').innerText = data.historicoOS ? data.historicoOS.length : 0;
    document.getElementById('totalVendasCount').innerText = data.historicoVendas ? data.historicoVendas.length : 0;
    
    let totalGasto = 0;
    if (data.historicoOS) {
        data.historicoOS.forEach(os => totalGasto += os.ValorTotal || 0);
    }
    if (data.historicoVendas) {
        data.historicoVendas.forEach(v => totalGasto += v.TotalFinal || 0);
    }
    
    document.getElementById('totalGasto').innerText = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalGasto);
}

function formatDate(dateString) {
    if (!dateString) return '-';
    try {
        const d = new Date(dateString);
        return d.toLocaleDateString('pt-BR');
    } catch {
        return dateString;
    }
}

function formatCurrency(value) {
    if (value === null || value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

function getStatusBadge(status) {
    if (!status) return `<span class="badge">-</span>`;
    let className = '';
    let st = status.toLowerCase();
    
    if (st.includes('abert') || st.includes('andamento') || st.includes('orçamento')) {
        className = 'status-aberta';
    } else if (st.includes('concluí') || st.includes('pront') || st.includes('entregue') || st.includes('aprovado')) {
        className = 'status-concluida';
    } else if (st.includes('cancel') || st.includes('reprovad')) {
        className = 'status-cancelada';
    }
    
    return `<span class="badge ${className}">${status}</span>`;
}

function populateOS(osList) {
    const tbody = document.querySelector('#tableOS tbody');
    const emptyState = document.getElementById('emptyOS');
    const table = document.getElementById('tableOS');
    
    tbody.innerHTML = '';
    
    if (!osList || osList.length === 0) {
        table.style.display = 'none';
        emptyState.style.display = 'flex';
        return;
    }
    
    table.style.display = 'table';
    emptyState.style.display = 'none';
    
    osList.forEach(os => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><strong>#${os.Id}</strong></td>
            <td>${os.Marca || '-'} ${os.Modelo || ''}</td>
            <td>${os.Defeito || '-'}</td>
            <td>${getStatusBadge(os.Status)}</td>
            <td style="font-weight: 500;">${formatCurrency(os.ValorTotal)}</td>
            <td>${formatDate(os.DataAbertura)}</td>
        `;
        tbody.appendChild(tr);
    });
}

function populateVendas(vendasList) {
    const tbody = document.querySelector('#tableVendas tbody');
    const emptyState = document.getElementById('emptyVendas');
    const table = document.getElementById('tableVendas');
    
    tbody.innerHTML = '';
    
    if (!vendasList || vendasList.length === 0) {
        table.style.display = 'none';
        emptyState.style.display = 'flex';
        return;
    }
    
    table.style.display = 'table';
    emptyState.style.display = 'none';
    
    vendasList.forEach(v => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><strong>#${v.Id}</strong></td>
            <td>${formatDate(v.DataVenda)}</td>
            <td>${v.FormaPagamento || '-'}</td>
            <td style="font-weight: 500;" class="success-text">${formatCurrency(v.TotalFinal)}</td>
        `;
        tbody.appendChild(tr);
    });
}

// Tab Navigation
document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        // Remove active class from all buttons and contents
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
        
        // Add active class to clicked button and target content
        btn.classList.add('active');
        const targetId = btn.getAttribute('data-tab');
        document.getElementById(targetId).classList.add('active');
    });
});

// Close actions
function closeWindow() {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'CLOSE' });
    }
}

document.getElementById('btnClose').addEventListener('click', closeWindow);
document.getElementById('btnFooterClose').addEventListener('click', closeWindow);

// Listen for direct JS calls from C# if necessary
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        // Handle potential messages from host if not using executeScriptAsync
    });
}
