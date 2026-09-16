// app.js

let searchTimeout = null;
let currentSaleTotal = 0.0;
let currentSaleRest = 0.0;
let clientSearchTimeout = null;
let currentClientId = 0;
let clienteSaldoAtual = 0;

// Called on keyup in the search input
function handleSearchInput(event) {
    const input = document.getElementById('searchInput');
    const query = input.value;
    
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({
                action: 'SEARCH_PRODUCTS',
                query: query
            });
        }
    }, 300); // 300ms debounce
}

// Called when user clicks a search result
function addSelectedItem(productId) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({
            action: 'ADD_SELECTED_ITEM',
            id: productId
        });
    }
    
    // Clear search box
    const input = document.getElementById('searchInput');
    input.value = '';
    handleSearchInput(); // trigger search with empty query to reset list
    input.focus();
}

// C# will call this function to update the list
function updateItemsList(itemsJson) {
    const items = JSON.parse(itemsJson);
    const tbody = document.getElementById('itemsList');
    tbody.innerHTML = ''; // Clear current

    items.forEach((item, index) => {
        appendItemToList(item, index);
    });
    
    updateButtonsState();
}

// Helper for consistent string hashing to generate colors
function getColorForCategory(str) {
    if (!str) return 'transparent';
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    const hue = Math.abs(hash % 360);
    return `hsl(${hue}, 65%, 25%)`; // Darker backgrounds for neon theme
}

// C# will call this to populate the center search results table
function renderSearchResults(jsonString) {
    const products = JSON.parse(jsonString);
    const tbody = document.getElementById('searchResultsList');
    tbody.innerHTML = '';

    if (products.length === 0) {
        tbody.innerHTML = '<tr><td colspan="3" style="text-align: center; color: var(--text-secondary);">Nenhum produto encontrado.</td></tr>';
        return;
    }

    products.forEach(p => {
        const tr = document.createElement('tr');
        tr.onclick = () => addSelectedItem(p.Id);
        
        const catBadge = p.CategoriaNome ? `<span style="background-color: ${getColorForCategory(p.CategoriaNome)}; color: #fff; margin-left: 8px; font-size: 0.65em; padding: 2px 6px; border-radius: 4px; vertical-align: middle;">${p.CategoriaNome}</span>` : '';
        
        let estoqueColor = '';
        if (p.Estoque <= 0) {
            estoqueColor = 'color: #ff4444; font-weight: bold;'; // vermelho
        } else if (p.Estoque >= 1 && p.Estoque <= 5) {
            estoqueColor = 'color: #ff8800; font-weight: bold;'; // laranja
        } else {
            estoqueColor = 'color: #33b5e5; font-weight: bold;'; // azul claro
        }
        
        tr.innerHTML = `
            <td>
                <span class="item-name">${p.Nome}${catBadge}</span><br>
                ${p.Marca ? `<span class="item-sub">${p.Marca}</span>` : ''}
            </td>
            <td>R$ ${p.PrecoVenda.toFixed(2).replace('.', ',')}</td>
            <td style="${estoqueColor}">${p.Estoque}</td>
        `;
        tbody.appendChild(tr);
    });
}

// C# will call this function to update the totals
function updateTotals(totalsJson) {
    const totals = JSON.parse(totalsJson);
    
    currentSaleTotal = totals.total;
    currentSaleRest = totals.rest;
    
    document.getElementById('totalAValue').innerText = `R$ ${totals.total.toFixed(2).replace('.', ',')}`;
    
    const restEl = document.querySelector('.value.warning');
    const changeEl = document.querySelector('.value.success');
    
    restEl.innerText = `R$ ${totals.rest.toFixed(2).replace('.', ',')}`;
    changeEl.innerText = `R$ ${totals.change.toFixed(2).replace('.', ',')}`;
}

// C# will call this to update Client Info
function updateClient(clientJson) {
    const client = JSON.parse(clientJson);
    currentClientId = client.id;
    clienteSaldoAtual = client.balance || 0;
    document.getElementById('clientName').innerText = client.name;
    const balanceBEl = document.querySelector('.balance-badge b');
    balanceBEl.innerText = `R$ ${client.balance.toFixed(2).replace('.', ',')}`;
    
    const optSaldo = document.getElementById('optSaldoCliente');
    if (optSaldo) {
        if (clienteSaldoAtual > 0) {
            optSaldo.style.display = 'block';
            optSaldo.disabled = false;
            optSaldo.innerText = `Saldo do Cliente (R$ ${clienteSaldoAtual.toFixed(2).replace('.', ',')})`;
        } else {
            optSaldo.style.display = 'none';
            optSaldo.disabled = true;
            if (document.getElementById('modalPaymentMethod').value === 'saldo_cliente') {
                document.getElementById('modalPaymentMethod').value = 'dinheiro';
            }
        }
    }
}

// Helper to append item to table
function appendItemToList(item, index) {
    const tbody = document.getElementById('itemsList');
    const tr = document.createElement('tr');
    
    tr.innerHTML = `
        <td>
            <span class="item-name">${item.name}</span><br>
            ${item.sub ? `<span class="item-sub">${item.sub}</span>` : ''}
        </td>
        <td>
            <div class="qty-control" style="cursor: text;">
                <input type="number" value="${item.qty}" min="1" style="width: 45px; background: transparent; color: white; border: none; text-align: center; font-size: 14px; outline: none;" onchange="sendUpdateQty(${index}, this.value)" onclick="this.select()">
            </div>
            <span class="qty-price">${item.price.toFixed(2).replace('.', ',')}</span>
        </td>
        <td class="item-total">R$ ${item.total.toFixed(2).replace('.', ',')}</td>
    `;
    
    tbody.appendChild(tr);
}

function sendUpdateQty(index, newQty) {
    if (newQty < 1) newQty = 1;
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'UPDATE_QTY', index: index, qty: parseInt(newQty) });
    }
}

// Listen for enter key on input (just to prevent default form behavior if any)
document.getElementById('searchInput').addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        // The search is already handled by keyup debounce
    }
});

// Generic action sender
function sendAction(actionName) {
    if (actionName === 'FINALIZAR_VENDA' || actionName === 'CANCELAR_VENDA' || actionName === 'REMOVER_ITEM') {
        const itemCount = document.getElementById('itemsList').children.length;
        if (itemCount === 0) return;
    }

    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: actionName });
    } else {
        console.log("Mock ACTION:", actionName);
    }
}

// Global Keyboard Shortcuts
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        // If any modal is active, close it
        const activeModal = document.querySelector('.modal-overlay.active');
        if (activeModal) {
            closeModal(activeModal.id);
            return;
        }
    }

    switch(e.key) {
        case 'F2':
            e.preventDefault();
            document.getElementById('searchInput').focus();
            break;
        case 'F5':
            e.preventDefault();
            openModal('modal-finalizar');
            break;
        case 'Delete':
            if (document.activeElement.tagName !== 'INPUT') {
                e.preventDefault();
                sendAction('REMOVER_ITEM');
            }
            break;
        case 'Escape':
            e.preventDefault();
            sendAction('CANCELAR_VENDA');
            break;
    }
});

// --- Modal System & Business Logic ---

function openModal(modalId) {
    if (modalId === 'modal-finalizar') {
        const itemCount = document.getElementById('itemsList').children.length;
        if (itemCount === 0) return;
    }

    document.getElementById(modalId).classList.add('active');
    
    if (modalId === 'modal-busca-cliente') {
        const input = document.getElementById('searchClientInput');
        input.value = '';
        input.focus();
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({ action: 'SEARCH_CLIENTS', query: '' });
        }
    } else if (modalId === 'modal-desconto') {
        const input = document.getElementById('descontoInput');
        input.value = '';
        input.focus();
    } else if (modalId === 'modal-finalizar') {
        document.getElementById('modalFinalizarTotal').innerText = `R$ ${currentSaleTotal.toFixed(2).replace('.', ',')}`;
        document.getElementById('valorRecebidoInput').value = '';
        document.getElementById('mistoDinheiro').value = '';
        document.getElementById('mistoPix').value = '';
        document.getElementById('mistoCartao').value = '';
        document.getElementById('mistoSaldo').value = '';
        document.getElementById('modalFinalizarTroco').innerText = `R$ 0,00`;
        
        syncPaymentMethod();
        
        setTimeout(() => document.getElementById('valorRecebidoInput').focus(), 100);
    }
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}

function handleClientSearch(event) {
    const query = event.target.value;
    clearTimeout(clientSearchTimeout);
    clientSearchTimeout = setTimeout(() => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({ action: 'SEARCH_CLIENTS', query: query });
        }
    }, 300);
}

function renderClientSearchResults(jsonString) {
    const clients = JSON.parse(jsonString);
    const tbody = document.getElementById('clientSearchResults');
    tbody.innerHTML = '';

    if (clients.length === 0) {
        tbody.innerHTML = '<tr><td colspan="3" style="text-align: center; color: var(--text-secondary);">Nenhum cliente encontrado.</td></tr>';
        return;
    }

    clients.forEach(c => {
        const tr = document.createElement('tr');
        tr.onclick = () => selectClient(c.Id);
        
        tr.innerHTML = `
            <td>${c.Nome}</td>
            <td>${c.CpfCnpj || 'N/A'}</td>
            <td>${c.Telefone || 'N/A'}</td>
        `;
        tbody.appendChild(tr);
    });
}

function selectClient(clientId) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'SELECT_CLIENT', id: clientId });
    }
    closeModal('modal-busca-cliente');
}

function confirmDesconto() {
    const valor = parseFloat(document.getElementById('descontoInput').value) || 0;
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'CONFIRMAR_DESCONTO', valor: valor });
    }
    closeModal('modal-desconto');
}

function calculateTroco() {
    let recebido = 0;
    const pm = document.getElementById('modalPaymentMethod').value;
    
    if (pm === 'misto') {
        const din = parseFloat(document.getElementById('mistoDinheiro').value) || 0;
        const pix = parseFloat(document.getElementById('mistoPix').value) || 0;
        const car = parseFloat(document.getElementById('mistoCartao').value) || 0;
        const sal = parseFloat(document.getElementById('mistoSaldo').value) || 0;
        recebido = din + pix + car + sal;
    } else if (pm === 'A Prazo (Fiado)' || pm === 'saldo_cliente') {
        recebido = currentSaleTotal;
    } else {
        recebido = parseFloat(document.getElementById('valorRecebidoInput').value) || 0;
    }
    
    const troco = recebido - currentSaleTotal;
    const trocoEl = document.getElementById('modalFinalizarTroco');
    
    if (troco > 0) {
        trocoEl.innerText = `R$ ${troco.toFixed(2).replace('.', ',')}`;
        trocoEl.style.color = 'var(--success-green)';
    } else {
        trocoEl.innerText = `R$ 0,00`;
        trocoEl.style.color = 'var(--text-secondary)';
    }
}

function confirmFinalizarVenda() {
    let recebido = 0;
    let saldoUsado = 0;
    const metodo = document.getElementById('modalPaymentMethod').value;
    
    let din = 0, pix = 0, car = 0;
    if (metodo === 'saldo_cliente') {
        if (currentSaleTotal > clienteSaldoAtual) {
            alert('Saldo insuficiente! O cliente possui apenas R$ ' + clienteSaldoAtual.toFixed(2).replace('.', ',') + ' de saldo. Utilize a opção "Misto" se quiser usar parte do saldo e interar o restante em outra forma.');
            return;
        }
        saldoUsado = currentSaleTotal;
        recebido = currentSaleTotal;
    } else if (metodo === 'misto') {
        din = parseFloat(document.getElementById('mistoDinheiro').value) || 0;
        pix = parseFloat(document.getElementById('mistoPix').value) || 0;
        car = parseFloat(document.getElementById('mistoCartao').value) || 0;
        const sal = parseFloat(document.getElementById('mistoSaldo').value) || 0;
        saldoUsado = sal;
        
        if (saldoUsado > clienteSaldoAtual) {
            alert('Saldo insuficiente! O cliente possui apenas R$ ' + clienteSaldoAtual.toFixed(2).replace('.', ',') + ' de saldo.');
            return;
        }
        
        recebido = din + pix + car + sal;
    } else {
        recebido = parseFloat(document.getElementById('valorRecebidoInput').value) || 0;
    }
    
    if (recebido < currentSaleTotal && (metodo === 'dinheiro' || metodo === 'misto')) {
        alert('Valor recebido é menor que o total da venda!');
        return;
    }
    
    if (metodo.toLowerCase().includes('fiado') || metodo.toLowerCase().includes('prazo')) {
        if (!currentClientId || currentClientId === 0) {
            alert('Para vendas a prazo/fiado, é obrigatório vincular um cliente!');
            return;
        }
    }
    
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ 
            action: 'CONFIRMAR_FINALIZAR', 
            recebido: recebido, 
            metodo: metodo,
            saldoUsado: saldoUsado,
            clientId: currentClientId,
            mistoDinheiro: din,
            mistoPix: pix,
            mistoCartao: car
        });
    }
    closeModal('modal-finalizar');
}

function syncPaymentMethod() {
    const pm = document.getElementById('paymentMethod').value;
    if (pm && pm !== 'Seleção do pagamento') {
        document.getElementById('modalPaymentMethod').value = pm;
        handlePaymentMethodChange();
    }
}

function handlePaymentMethodChange() {
    const pm = document.getElementById('modalPaymentMethod').value;
    const recebidoInput = document.getElementById('valorRecebidoInput');
    const mistoDiv = document.getElementById('mistoInputs');
    
    if (pm === 'misto') {
        mistoDiv.style.display = 'block';
        document.getElementById('valorRecebidoContainer').style.display = 'none';
        recebidoInput.disabled = true;
        recebidoInput.value = '';
    } else if (pm === 'A Prazo (Fiado)' || pm === 'saldo_cliente') {
        mistoDiv.style.display = 'none';
        document.getElementById('valorRecebidoContainer').style.display = 'block';
        recebidoInput.disabled = true;
        recebidoInput.value = currentSaleTotal.toFixed(2);
    } else {
        mistoDiv.style.display = 'none';
        document.getElementById('valorRecebidoContainer').style.display = 'block';
        recebidoInput.disabled = false;
    }
    calculateTroco();
}

// --- Sidebar Menu Logic ---

function toggleSidebar() {
    const sidebar = document.getElementById('mainSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    
    if (sidebar.classList.contains('active')) {
        sidebar.classList.remove('active');
        overlay.classList.remove('active');
    } else {
        sidebar.classList.add('active');
        overlay.classList.add('active');
    }
}

function sendSidebarAction(actionName) {
    toggleSidebar(); // Close sidebar
    sendAction(actionName); // Send action to C#
}

function updateButtonsState() {
    const itemCount = document.getElementById('itemsList').children.length;
    const temItens = itemCount > 0;
    
    const btnRemover = document.getElementById('btnRemoverItem');
    const btnCancelar = document.getElementById('btnCancelarVenda');
    const btnFinalizar = document.getElementById('btnFinalizarVenda');
    
    if (btnRemover) {
        btnRemover.disabled = !temItens;
        btnRemover.style.opacity = temItens ? '1' : '0.5';
        btnRemover.style.cursor = temItens ? 'pointer' : 'not-allowed';
    }
    if (btnCancelar) {
        btnCancelar.disabled = !temItens;
        btnCancelar.style.opacity = temItens ? '1' : '0.5';
        btnCancelar.style.cursor = temItens ? 'pointer' : 'not-allowed';
    }
    if (btnFinalizar) {
        btnFinalizar.disabled = !temItens;
        btnFinalizar.style.opacity = temItens ? '1' : '0.5';
        btnFinalizar.style.cursor = temItens ? 'pointer' : 'not-allowed';
    }
}

// Inicializar estado dos botões no carregamento da página
document.addEventListener('DOMContentLoaded', () => {
    updateButtonsState();
});
