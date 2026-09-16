let partsItems = [];

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('txtValorPecas').addEventListener('input', calculateTotal);
    document.getElementById('txtValorServico').addEventListener('input', calculateTotal);
});

function sendAction(actionName, data = null) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: actionName, data: data });
    }
}

function parseMoney(val) {
    if (!val) return 0;
    if (typeof val === 'number') return val;
    return parseFloat(val.toString().replace('R$', '').replace('.', '').replace(',', '.').trim()) || 0;
}

function formatMoney(val) {
    return val.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function calculateTotal() {
    let partsVal = parseMoney(document.getElementById('txtValorPecas').value);
    let serviceVal = parseMoney(document.getElementById('txtValorServico').value);
    
    let partsSum = partsItems.reduce((acc, item) => acc + (item.SubTotal || 0), 0);
    
    // Auto sync parts cost if grid has items
    if (partsItems.length > 0) {
        partsVal = partsSum;
        document.getElementById('txtValorPecas').value = formatMoney(partsVal);
    }
    
    // Calculate total additional costs
    let totalCustosAdicionais = partsItems.reduce((acc, item) => acc + (item.CustoAdicional || 0), 0);
    
    let lucro = serviceVal - partsVal - totalCustosAdicionais;
    
    document.getElementById('lblLucro').innerText = formatMoney(lucro);
}

// Called from C#
function loadFormData(dataJson) {
    const data = JSON.parse(dataJson);
    
    // Combos
    clientesGlobal = data.Clientes || [];
    const txtCliente = document.getElementById('cboCliente');
    const dlCliente = document.getElementById('clientesList');
    dlCliente.innerHTML = '';
    clientesGlobal.forEach(c => {
        let opt = document.createElement('option');
        opt.value = c.Nome;
        dlCliente.appendChild(opt);
    });

    const cboTecnico = document.getElementById('cboTecnico');
    cboTecnico.innerHTML = '<option value="0">-- Selecione um Técnico --</option>';
    data.Tecnicos.forEach(t => {
        let opt = document.createElement('option');
        opt.value = t.Id;
        opt.text = t.Nome;
        cboTecnico.add(opt);
    });

    // Populate OS
    if (data.OS) {
        document.getElementById('lblOsId').innerText = data.OS.Id;
        document.getElementById('btnPrint').style.display = 'block'; // Show print
        
        if (data.OS.IdCliente) {
            let cl = clientesGlobal.find(c => c.Id == data.OS.IdCliente);
            txtCliente.value = cl ? cl.Nome : '';
        } else if (data.OS.ClienteFinal) {
            txtCliente.value = data.OS.ClienteFinal;
        } else {
            txtCliente.value = '';
        }

        cboTecnico.value = data.OS.IdTecnico || 0;
        
        document.getElementById('txtMarca').value = data.OS.Marca || '';
        document.getElementById('txtModelo').value = data.OS.Modelo || '';
        document.getElementById('txtImei').value = data.OS.IMEI || '';
        document.getElementById('txtCor').value = data.OS.Cor || '';
        document.getElementById('txtDefeito').value = data.OS.Defeito || '';
        document.getElementById('txtLaudo').value = data.OS.LaudoTecnico || '';
        document.getElementById('cboStatus').value = data.OS.Status || 'Aberto';
        
        document.getElementById('txtValorPecas').value = formatMoney(data.OS.ValorPecas);
        document.getElementById('txtValorServico').value = formatMoney(data.OS.ValorTotal);
        
        partsItems = data.OS.Itens || [];
    } else {
        document.getElementById('lblOsId').innerText = "Nova";
        partsItems = [];
    }
    
    renderParts();
    calculateTotal();
}

function renderParts() {
    const tbody = document.getElementById('partsBody');
    tbody.innerHTML = '';
    
    partsItems.forEach((item, index) => {
        let custoText = item.CustoAdicional > 0 ? `<br><small style="color:var(--danger)">Custo: R$ ${formatMoney(item.CustoAdicional)}</small>` : '';
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${item.NomeProduto}</td>
            <td>${item.Quantidade}</td>
            <td>R$ ${formatMoney(item.SubTotal)}${custoText}</td>
            <td>
                <div style="display:flex; gap:5px; justify-content:center;">
                    <button class="btn-icon" style="width:30px;height:30px;font-size:14px;background:var(--warning, #f59e0b)" onclick="applyDiscount(${index})" title="Aplicar Desconto">🏷️</button>
                    <button class="btn-icon" style="width:30px;height:30px;font-size:14px;background:var(--danger)" onclick="removePart(${index})" title="Remover">🗑️</button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
    calculateTotal();
}

// C# calls this when a product is selected in BuscaProduto
function addPart(partJson) {
    const part = JSON.parse(partJson);
    partsItems.push(part);
    renderParts();
}

let currentDiscountIndex = -1;

function applyDiscount(index) {
    let item = partsItems[index];
    currentDiscountIndex = index;
    
    document.getElementById('modalItemName').innerText = item.NomeProduto;
    document.getElementById('modalCurrentValue').value = formatMoney(item.SubTotal);
    document.getElementById('modalDiscountValue').value = '0,00';
    
    document.getElementById('discountModal').classList.add('show');
}

function closeDiscountModal() {
    document.getElementById('discountModal').classList.remove('show');
    currentDiscountIndex = -1;
}

function confirmDiscount() {
    if (currentDiscountIndex === -1) return;
    
    let item = partsItems[currentDiscountIndex];
    let discountStr = document.getElementById('modalDiscountValue').value;
    let discount = parseMoney(discountStr);
    
    if (!isNaN(discount) && discount >= 0 && discount <= item.SubTotal) {
        item.SubTotal -= discount;
        item.Desconto = (item.Desconto || 0) + discount;
        renderParts();
        closeDiscountModal();
    } else if (discount > item.SubTotal) {
        alert("O valor do desconto não pode ser maior que o subtotal do item!");
    } else {
        alert("Valor de desconto inválido!");
    }
}

function openCustoModal() {
    document.getElementById('modalCustoValue').value = '0,00';
    document.getElementById('custoModal').classList.add('show');
}

function closeCustoModal() {
    document.getElementById('custoModal').classList.remove('show');
}

function confirmCusto() {
    let category = document.getElementById('cboCustoCategoria').value;
    let custoStr = document.getElementById('modalCustoValue').value;
    let custo = parseMoney(custoStr);
    
    if (!isNaN(custo) && custo > 0) {
        let newItem = {
            ProdutoId: 0,
            NomeProduto: "Terceirizado: " + category,
            Quantidade: 1,
            ValorUnitario: 0,
            SubTotal: 0,
            CustoAdicional: custo
        };
        partsItems.push(newItem);
        renderParts();
        closeCustoModal();
    } else {
        alert("Informe um valor de custo válido e maior que zero!");
    }
}

function removePart(index) {
    partsItems.splice(index, 1);
    renderParts();
}

function saveOS() {
    let typedClient = document.getElementById('cboCliente').value.trim();
    let foundClient = clientesGlobal.find(c => c.Nome.toLowerCase() === typedClient.toLowerCase());

    const osData = {
        IdCliente: foundClient ? foundClient.Id : 0,
        ClienteFinal: !foundClient ? typedClient : null,
        IdTecnico: parseInt(document.getElementById('cboTecnico').value) || null,
        Marca: document.getElementById('txtMarca').value,
        Modelo: document.getElementById('txtModelo').value,
        IMEI: document.getElementById('txtImei').value,
        Cor: document.getElementById('txtCor').value,
        Defeito: document.getElementById('txtDefeito').value,
        LaudoTecnico: document.getElementById('txtLaudo').value,
        Status: document.getElementById('cboStatus').value,
        ValorPecas: parseMoney(document.getElementById('txtValorPecas').value),
        ValorServico: parseMoney(document.getElementById('txtValorServico').value),
        Itens: partsItems
    };
    
    sendAction('SAVE_OS', osData);
}
