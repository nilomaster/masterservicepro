// app.js

let searchTimeout = null;
let currentFilter = 'Todas';
let isKanbanView = false;
let selectedOSId = 0;

// Init
document.addEventListener('DOMContentLoaded', () => {
    // Setup filter buttons
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            e.target.classList.add('active');
            currentFilter = e.target.getAttribute('data-filter');
            requestData();
        });
    });

    // Close context menu on outside click
    document.addEventListener('click', (e) => {
        if (!e.target.closest('.context-menu')) {
            hideContextMenu();
        }
    });

    // Request initial data
    setTimeout(requestData, 100);
});

function toggleView() {
    isKanbanView = !isKanbanView;
    const btn = document.getElementById('btnToggleView');
    const tableV = document.getElementById('tableView');
    const kanbanV = document.getElementById('kanbanView');

    if (isKanbanView) {
        btn.innerText = "📋 Modo Tabela";
        tableV.classList.remove('active');
        kanbanV.classList.add('active');
    } else {
        btn.innerText = "📋 Modo Kanban";
        kanbanV.classList.remove('active');
        tableV.classList.add('active');
    }
}

function handleSearchInput(event) {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        requestData();
    }, 300);
}

function requestData() {
    const query = document.getElementById('searchInput').value;
    const dtInicio = document.getElementById('dtInicio').value;
    const dtFim = document.getElementById('dtFim').value;
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({
            action: 'LOAD_DATA',
            filter: currentFilter,
            query: query,
            dtInicio: dtInicio,
            dtFim: dtFim
        });
    } else {
        console.log("Mock LOAD_DATA", currentFilter, query, dtInicio, dtFim);
    }
}

function initDates() {
    if (window.isAdmin === false) {
        const d = new Date();
        const formatYMD = (dateObj) => {
            const offset = dateObj.getTimezoneOffset() * 60000;
            return new Date(dateObj.getTime() - offset).toISOString().split('T')[0];
        };
        document.getElementById('dtInicio').value = formatYMD(d);
        document.getElementById('dtFim').value = formatYMD(d);
        requestData();
    }
}

function sendAction(actionName, id) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({
            action: actionName,
            id: id
        });
    }
}

// C# will call this to populate data
function renderData(jsonString) {
    const data = JSON.parse(jsonString);
    renderTable(data);
    renderKanban(data);
}

function getStatusClass(status) {
    if (status === 'Pendente' || status === 'Aberto') return 'status-aberto';
    if (status === 'Em Andamento') return 'status-andamento';
    if (status === 'Finalizado') return 'status-finalizado';
    if (status === 'Cancelado' || status === 'Cancelada') return 'status-cancelado';
    return 'status-aberto';
}

function formatDateTime(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return '-';
    return date.toLocaleDateString('pt-BR') + ' ' + date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
}

function renderTable(data) {
    const tbody = document.getElementById('osTableBody');
    tbody.innerHTML = '';

    if (data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" style="text-align: center; color: var(--text-secondary); padding: 30px;">Nenhuma O.S. encontrada.</td></tr>';
        return;
    }

    data.forEach(os => {
        const tr = document.createElement('tr');
        tr.oncontextmenu = (e) => showContextMenu(e, os.Id);
        tr.ondblclick = () => sendAction('EDITAR_OS', os.Id);

        tr.innerHTML = `
            <td>${os.Id}</td>
            <td><strong>${os.Cliente || 'Sem Cliente'}</strong></td>
            <td>${os.Marca || ''} ${os.Modelo || ''}</td>
            <td style="color: var(--danger-red); font-size: 12px;">${os.Defeito || ''}</td>
            <td>R$ ${os.ValorTotal ? os.ValorTotal.toFixed(2).replace('.', ',') : '0,00'}</td>
            <td>R$ ${os.Lucro != null ? os.Lucro.toFixed(2).replace('.', ',') : (os.ValorTotal ? os.ValorTotal.toFixed(2).replace('.', ',') : '0,00')}</td>
            <td style="font-size: 12px; color: var(--text-secondary);">${formatDateTime(os.DataAbertura)}</td>
            <td style="font-size: 12px; color: var(--text-secondary);">${(os.Status === 'Finalizado' || os.Status === 'Entregue') ? formatDateTime(os.DataAtualizacao) : '-'}</td>
            <td><span class="status-badge ${getStatusClass(os.Status)}">${os.Status}</span></td>
            <td>
                <div class="table-actions">
                    <button class="btn-icon" onclick="sendAction('EDITAR_OS', ${os.Id})" title="Editar">✏️</button>
                    <button class="btn-icon" onclick="sendAction('IMPRIMIR_OS', ${os.Id})" title="Imprimir">🖨️</button>
                    <button class="btn-icon" onclick="showContextMenu(event, ${os.Id})" title="Mais Ações">⋮</button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function renderKanban(data) {
    const cols = {
        'Aberto': document.querySelector('#col-Aberto .kanban-cards'),
        'Em Andamento': document.querySelector('#col-Em\\ Andamento .kanban-cards'),
        'Finalizado': document.querySelector('#col-Finalizado .kanban-cards'),
        'Cancelado': document.querySelector('#col-Cancelado .kanban-cards')
    };

    // Clear all
    Object.values(cols).forEach(col => { if(col) col.innerHTML = ''; });

    data.forEach(os => {
        let statusKey = 'Aberto';
        if (os.Status === 'Em Andamento') statusKey = 'Em Andamento';
        else if (os.Status === 'Finalizado') statusKey = 'Finalizado';
        else if (os.Status === 'Cancelado' || os.Status === 'Cancelada') statusKey = 'Cancelado';

        const card = document.createElement('div');
        card.className = `kanban-card ${getStatusClass(os.Status)}`;
        card.oncontextmenu = (e) => showContextMenu(e, os.Id);
        card.ondblclick = () => sendAction('EDITAR_OS', os.Id);

        card.innerHTML = `
            <div class="kanban-card-title">O.S. #${os.Id} - Lucro: R$ ${os.Lucro != null ? os.Lucro.toFixed(2).replace('.', ',') : (os.ValorTotal ? os.ValorTotal.toFixed(2).replace('.', ',') : '0,00')}</div>
            <div class="kanban-card-subtitle">${os.Cliente || 'Sem Cliente'}<br>${os.Marca || ''} ${os.Modelo || ''}</div>
            <div class="kanban-card-defeito">${os.Defeito || ''}</div>
        `;

        if (cols[statusKey]) {
            cols[statusKey].appendChild(card);
        }
    });
}

// Context Menu
function showContextMenu(e, id) {
    e.preventDefault();
    selectedOSId = id;
    
    // Highlight table row if in table
    document.querySelectorAll('.data-table tbody tr').forEach(tr => tr.classList.remove('selected'));
    const tr = e.target.closest('tr');
    if (tr) tr.classList.add('selected');

    const menu = document.getElementById('contextMenu');
    
    // Ensure menu doesn't go offscreen
    let x = e.clientX;
    let y = e.clientY;
    
    menu.style.display = 'block'; // Display to calculate dimensions
    
    if (x + menu.offsetWidth > window.innerWidth) {
        x = window.innerWidth - menu.offsetWidth - 10;
    }
    if (y + menu.offsetHeight > window.innerHeight) {
        y = window.innerHeight - menu.offsetHeight - 10;
    }

    menu.style.left = `${x}px`;
    menu.style.top = `${y}px`;
    menu.classList.add('active');
}

function hideContextMenu() {
    const menu = document.getElementById('contextMenu');
    menu.classList.remove('active');
    setTimeout(() => { menu.style.display = 'none'; }, 100);
    document.querySelectorAll('.data-table tbody tr').forEach(tr => tr.classList.remove('selected'));
}

function handleContextAction(action) {
    hideContextMenu();
    if (selectedOSId > 0) {
        sendAction(action, selectedOSId);
    }
}
