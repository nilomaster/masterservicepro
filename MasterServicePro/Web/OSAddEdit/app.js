let partsItems = [];
let marcasDbGlobal = [];
let modelosDbGlobal = [];

// Catalog of standard cellphone brands and their popular models (ASCII comments only)
const CATALOGO_MARCAS_MODELOS = {
    "Apple": [
        "iPhone 16 Pro Max", "iPhone 16 Pro", "iPhone 16 Plus", "iPhone 16",
        "iPhone 15 Pro Max", "iPhone 15 Pro", "iPhone 15 Plus", "iPhone 15",
        "iPhone 14 Pro Max", "iPhone 14 Pro", "iPhone 14 Plus", "iPhone 14",
        "iPhone 13 Pro Max", "iPhone 13 Pro", "iPhone 13", "iPhone 13 mini",
        "iPhone 12 Pro Max", "iPhone 12 Pro", "iPhone 12", "iPhone 12 mini",
        "iPhone 11 Pro Max", "iPhone 11 Pro", "iPhone 11",
        "iPhone XR", "iPhone XS Max", "iPhone XS", "iPhone X",
        "iPhone SE (2022)", "iPhone SE (2020)",
        "iPhone 8 Plus", "iPhone 8", "iPhone 7 Plus", "iPhone 7",
        "iPad 10", "iPad 9", "iPad Air", "iPad Pro", "Apple Watch"
    ],
    "Samsung": [
        "Galaxy S24 Ultra", "Galaxy S24+", "Galaxy S24",
        "Galaxy S23 Ultra", "Galaxy S23+", "Galaxy S23", "Galaxy S23 FE",
        "Galaxy S22 Ultra", "Galaxy S22+", "Galaxy S22",
        "Galaxy S21 Ultra", "Galaxy S21+", "Galaxy S21", "Galaxy S21 FE",
        "Galaxy S20 FE", "Galaxy S20+", "Galaxy S20",
        "Galaxy S10+", "Galaxy S10", "Galaxy S10e",
        "Galaxy Note 20 Ultra", "Galaxy Note 20", "Galaxy Note 10+",
        "Galaxy A55", "Galaxy A54 5G", "Galaxy A53 5G", "Galaxy A52s", "Galaxy A52", "Galaxy A51", "Galaxy A50",
        "Galaxy A35", "Galaxy A34 5G", "Galaxy A33 5G", "Galaxy A32", "Galaxy A31", "Galaxy A30s", "Galaxy A30",
        "Galaxy A25 5G", "Galaxy A24", "Galaxy A23", "Galaxy A22", "Galaxy A21s", "Galaxy A20s", "Galaxy A20",
        "Galaxy A15", "Galaxy A14", "Galaxy A13", "Galaxy A12", "Galaxy A11", "Galaxy A10s", "Galaxy A10",
        "Galaxy A05s", "Galaxy A05", "Galaxy A04s", "Galaxy A03s", "Galaxy A03 Core", "Galaxy A02s", "Galaxy A01",
        "Galaxy M54 5G", "Galaxy M53 5G", "Galaxy M34 5G", "Galaxy M14 5G", "Galaxy M12",
        "Galaxy Z Fold 6", "Galaxy Z Fold 5", "Galaxy Z Fold 4",
        "Galaxy Z Flip 6", "Galaxy Z Flip 5", "Galaxy Z Flip 4"
    ],
    "Motorola": [
        "Moto G84 5G", "Moto G73 5G", "Moto G54 5G", "Moto G53 5G", "Moto G34 5G", "Moto G24", "Moto G14",
        "Moto G82 5G", "Moto G62 5G", "Moto G52", "Moto G42", "Moto G32", "Moto G22",
        "Moto G60", "Moto G60s", "Moto G30", "Moto G20", "Moto G10",
        "Moto G9 Plus", "Moto G9 Play", "Moto G9 Power",
        "Moto G8 Plus", "Moto G8 Play", "Moto G8 Power", "Moto G8",
        "Moto G7 Plus", "Moto G7 Play", "Moto G7 Power", "Moto G7",
        "Edge 50 Ultra", "Edge 50 Pro", "Edge 50 Fusion",
        "Edge 40 Neo", "Edge 40 Pro", "Edge 40",
        "Edge 30 Ultra", "Edge 30 Fusion", "Edge 30 Pro", "Edge 30 Neo", "Edge 30",
        "Edge 20 Pro", "Edge 20", "Edge 20 Lite",
        "Moto E22", "Moto E13", "Moto E32", "Moto E20", "Moto E7 Plus", "Moto E7", "Moto E6 Plus"
    ],
    "Xiaomi": [
        "Redmi Note 13 Pro+ 5G", "Redmi Note 13 Pro 5G", "Redmi Note 13 5G", "Redmi Note 13 4G",
        "Redmi Note 12 Pro+ 5G", "Redmi Note 12 Pro 5G", "Redmi Note 12S", "Redmi Note 12 4G",
        "Redmi Note 11 Pro+ 5G", "Redmi Note 11 Pro", "Redmi Note 11S", "Redmi Note 11",
        "Redmi Note 10 Pro", "Redmi Note 10S", "Redmi Note 10",
        "Redmi Note 9 Pro", "Redmi Note 9S", "Redmi Note 9", "Redmi Note 8 Pro", "Redmi Note 8",
        "Redmi 13C", "Redmi 12 4G", "Redmi 12C", "Redmi 10C", "Redmi 10", "Redmi 9A", "Redmi 9C", "Redmi 9",
        "POCO X6 Pro 5G", "POCO X6 5G", "POCO X5 Pro 5G", "POCO X5 5G", "POCO X4 Pro 5G", "POCO X3 Pro", "POCO X3 NFC",
        "POCO F6 Pro", "POCO F6", "POCO F5 Pro", "POCO F5", "POCO F4 GT", "POCO F3",
        "POCO M6 Pro", "POCO M5s", "POCO M5", "POCO M4 Pro", "POCO C65",
        "Xiaomi 14 Ultra", "Xiaomi 14", "Xiaomi 13T Pro", "Xiaomi 13 Pro", "Xiaomi 13", "Xiaomi 12T Pro", "Xiaomi 12", "Xiaomi 11T Pro"
    ],
    "Realme": [
        "Realme 12 Pro+ 5G", "Realme 12 Pro 5G", "Realme 12 5G",
        "Realme 11 Pro+ 5G", "Realme 11 5G",
        "Realme 10 Pro+ 5G", "Realme 10",
        "Realme C67", "Realme C55", "Realme C53", "Realme C35", "Realme C33", "Realme C30s", "Realme C21Y", "Realme C11",
        "Realme GT 2 Pro", "Realme GT Master Edition", "Realme 9 Pro+", "Realme 8 Pro"
    ],
    "LG": [
        "K62", "K52", "K42", "K22+", "K22",
        "K61", "K51S", "K41S",
        "K12 Prime", "K12+", "K11+", "K10 Pro", "K10 Power", "K10",
        "Velvet", "G8X ThinQ", "G7 ThinQ", "Q60", "Q6+"
    ],
    "Asus": [
        "Zenfone 10", "Zenfone 9", "Zenfone 8", "Zenfone 7", "Zenfone 6", "Zenfone Max Pro M2",
        "ROG Phone 8 Pro", "ROG Phone 7", "ROG Phone 6"
    ],
    "Infinix": [
        "Smart 8", "Smart 7",
        "Hot 40 Pro", "Hot 40i", "Hot 30", "Hot 30i", "Hot 20i", "Hot 11",
        "Note 40 Pro", "Note 30 5G", "Note 30 Pro", "Note 12",
        "Zero 30 5G", "Zero Ultra"
    ],
    "Huawei": [
        "P60 Pro", "P50 Pro", "P40 Pro", "P30 Pro", "P30 Lite", "Nova 9", "Nova Y70"
    ],
    "Positivo": [
        "Twist 5 Pro", "Twist 4 Pro", "Twist 4 Mini", "Twist 3", "Q20"
    ],
    "Multilaser": [
        "G Max 2", "G Pro 2", "G 2", "F 2", "E Lite"
    ],
    "Philco": [
        "Hit P13", "Hit P12", "Hit P10", "Hit Max"
    ],
    "TCL": [
        "TCL 40 SE", "TCL 40 NxtPaper", "TCL 30 SE", "TCL 20 SE", "TCL 10 SE"
    ],
    "Nokia": [
        "G21", "G11 Plus", "C30", "C21 Plus", "C20", "C01 Plus", "5.4", "2.4"
    ],
    "Sony": [
        "Xperia 1 VI", "Xperia 1 V", "Xperia 5 V", "Xperia 10 V", "Xperia XZ"
    ],
    "Google": [
        "Pixel 9 Pro XL", "Pixel 9 Pro", "Pixel 9", "Pixel 8 Pro", "Pixel 8", "Pixel 7 Pro", "Pixel 7", "Pixel 6 Pro", "Pixel 6"
    ]
};

// Populates marcas datalist combining catalog and DB records
function populateMarcasDatalist(marcasDb = []) {
    marcasDbGlobal = marcasDb || [];
    const dl = document.getElementById('marcasList');
    if (!dl) return;
    dl.innerHTML = '';

    const marcasSet = new Set(Object.keys(CATALOGO_MARCAS_MODELOS));
    marcasDbGlobal.forEach(m => {
        if (m && m.trim()) marcasSet.add(m.trim());
    });

    const sortedMarcas = Array.from(marcasSet).sort((a, b) => a.localeCompare(b));
    sortedMarcas.forEach(marca => {
        const opt = document.createElement('option');
        opt.value = marca;
        dl.appendChild(opt);
    });
}

// Populates or filters modelos datalist based on the selected brand
function updateModelosDatalist(selectedMarca = '') {
    const dl = document.getElementById('modelosList');
    if (!dl) return;
    dl.innerHTML = '';

    const modelosSet = new Set();
    const cleanMarca = (selectedMarca || '').trim().toLowerCase();

    // Check if selected brand matches catalog
    let matchedBrandKey = Object.keys(CATALOGO_MARCAS_MODELOS).find(k => k.toLowerCase() === cleanMarca);

    if (matchedBrandKey) {
        // Add models for specific brand
        CATALOGO_MARCAS_MODELOS[matchedBrandKey].forEach(mod => modelosSet.add(mod));
    } else {
        // If no brand or brand not in catalog, include all popular models
        Object.values(CATALOGO_MARCAS_MODELOS).forEach(arr => {
            arr.forEach(mod => modelosSet.add(mod));
        });
    }

    // Add models from database
    modelosDbGlobal.forEach(mod => {
        if (mod && mod.trim()) modelosSet.add(mod.trim());
    });

    const sortedModelos = Array.from(modelosSet).sort((a, b) => a.localeCompare(b));
    sortedModelos.forEach(modelo => {
        const opt = document.createElement('option');
        opt.value = modelo;
        dl.appendChild(opt);
    });
}

// Direct money mask in Reais with auto dots for thousands and comma for cents
function attachDirectMoneyMask(input, onChangeCallback) {
    if (!input) return;

    function formatIntegerPart(str) {
        let clean = (str || "").replace(/\D/g, "");
        if (!clean) return "0";
        clean = clean.replace(/^0+/, "") || "0";
        return clean.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }

    function getParts(val) {
        val = (val || "0,00").toString().trim();
        let commaIdx = val.indexOf(",");
        if (commaIdx === -1) {
            let dotIdx = val.lastIndexOf(".");
            if (dotIdx !== -1 && val.length - dotIdx - 1 <= 2) {
                let intPart = val.slice(0, dotIdx).replace(/\D/g, "") || "0";
                let decPart = (val.slice(dotIdx + 1).replace(/\D/g, "") + "00").slice(0, 2);
                return { intPart, decPart };
            }
            return { intPart: val.replace(/\D/g, "") || "0", decPart: "00" };
        }
        let intPart = val.slice(0, commaIdx).replace(/\D/g, "") || "0";
        let decPart = (val.slice(commaIdx + 1).replace(/\D/g, "") + "00").slice(0, 2);
        return { intPart, decPart };
    }

    function setValWithCursor(rawInt, dec, targetCursor) {
        let formattedInt = formatIntegerPart(rawInt);
        let newVal = formattedInt + "," + dec;
        input.value = newVal;
        if (typeof targetCursor === "number") {
            let safe = Math.max(0, Math.min(newVal.length, targetCursor));
            if (input.setSelectionRange) input.setSelectionRange(safe, safe);
        }
        if (onChangeCallback) onChangeCallback();
    }

    let isFirstClick = false;
    input.addEventListener("focus", function () {
        isFirstClick = true;
        setTimeout(() => {
            if (this.select) this.select();
        }, 10);
    });

    input.addEventListener("mouseup", function (e) {
        if (isFirstClick) {
            e.preventDefault();
            isFirstClick = false;
        }
    });

    input.addEventListener("keydown", function (e) {
        let key = e.key;

        // Allow navigation and system shortcuts
        if (
            e.ctrlKey || e.metaKey || e.altKey ||
            key === "Tab" || key === "Enter" || key === "Escape" ||
            key === "ArrowLeft" || key === "ArrowRight" || key === "ArrowUp" || key === "ArrowDown" ||
            key === "Home" || key === "End"
        ) {
            return;
        }

        let isDigit = /^[0-9]$/.test(key);
        let isCommaOrDot = key === "," || key === ".";
        let isBackspace = key === "Backspace";
        let isDelete = key === "Delete";

        if (!isDigit && !isCommaOrDot && !isBackspace && !isDelete) {
            e.preventDefault();
            return;
        }

        let start = this.selectionStart || 0;
        let end = this.selectionEnd || 0;
        let isAllSelected = (start === 0 && end === this.value.length && this.value.length > 0);

        if (isDigit) {
            e.preventDefault();
            let commaIdx = this.value.indexOf(",");
            if (commaIdx === -1) commaIdx = this.value.length;

            if (isAllSelected) {
                setValWithCursor(key, "00", 1);
                return;
            }

            let parts = getParts(this.value);

            if (start > commaIdx) {
                // Typing into decimals
                let decPos = start - commaIdx;
                let d1 = parts.decPart[0] || "0";
                let d2 = parts.decPart[1] || "0";
                if (decPos === 1) {
                    parts.decPart = key + d2;
                    setValWithCursor(parts.intPart, parts.decPart, commaIdx + 2);
                } else {
                    parts.decPart = d1 + key;
                    setValWithCursor(parts.intPart, parts.decPart, commaIdx + 3);
                }
            } else {
                // Typing into integer part
                let oldVal = this.value;
                let textBefore = oldVal.slice(0, start).replace(/\D/g, "");
                let textAfter = oldVal.slice(start, commaIdx).replace(/\D/g, "");
                let newRawInt = textBefore + key + textAfter;

                let digitsBeforeNewCursor = textBefore.length + 1;
                let formattedNewInt = formatIntegerPart(newRawInt);

                let count = 0;
                let newCursorPos = formattedNewInt.length;
                for (let i = 0; i < formattedNewInt.length; i++) {
                    if (/\d/.test(formattedNewInt[i])) count++;
                    if (count === digitsBeforeNewCursor) {
                        newCursorPos = i + 1;
                        break;
                    }
                }

                setValWithCursor(newRawInt, parts.decPart, newCursorPos);
            }
        } else if (isCommaOrDot) {
            e.preventDefault();
            let commaIdx = this.value.indexOf(",");
            if (commaIdx !== -1 && this.setSelectionRange) {
                this.setSelectionRange(commaIdx + 1, commaIdx + 1);
            }
        } else if (isBackspace) {
            if (isAllSelected) {
                e.preventDefault();
                setValWithCursor("0", "00", 1);
                if (this.select) this.select();
                return;
            }

            let commaIdx = this.value.indexOf(",");
            let parts = getParts(this.value);

            if (start > commaIdx + 1) {
                // Inside decimals
                e.preventDefault();
                let decPos = start - commaIdx;
                let d1 = parts.decPart[0] || "0";
                if (decPos === 3) {
                    parts.decPart = d1 + "0";
                    setValWithCursor(parts.intPart, parts.decPart, commaIdx + 2);
                } else if (decPos === 2) {
                    parts.decPart = "00";
                    setValWithCursor(parts.intPart, parts.decPart, commaIdx + 1);
                }
            } else if (start === commaIdx + 1) {
                // Right after comma, jump cursor before comma
                e.preventDefault();
                if (this.setSelectionRange) this.setSelectionRange(commaIdx, commaIdx);
            } else {
                // In integer part
                e.preventDefault();
                let oldVal = this.value;
                let textBefore = oldVal.slice(0, start).replace(/\D/g, "");
                let textAfter = oldVal.slice(start, commaIdx).replace(/\D/g, "");
                if (textBefore.length > 0) {
                    let newRawInt = textBefore.slice(0, -1) + textAfter;
                    if (!newRawInt) newRawInt = "0";

                    let digitsBeforeNewCursor = textBefore.length - 1;
                    let formattedNewInt = formatIntegerPart(newRawInt);

                    let count = 0;
                    let newCursorPos = 0;
                    for (let i = 0; i < formattedNewInt.length; i++) {
                        if (/\d/.test(formattedNewInt[i])) count++;
                        if (count === digitsBeforeNewCursor) {
                            newCursorPos = i + 1;
                            break;
                        }
                    }
                    if (digitsBeforeNewCursor === 0) newCursorPos = Math.min(1, formattedNewInt.length);

                    setValWithCursor(newRawInt, parts.decPart, newCursorPos);
                }
            }
        } else if (isDelete) {
            if (isAllSelected) {
                e.preventDefault();
                setValWithCursor("0", "00", 1);
                if (this.select) this.select();
            }
        }
    });

    input.addEventListener("paste", function (e) {
        e.preventDefault();
        let pasteText = (e.clipboardData || window.clipboardData).getData("text");
        let parts = getParts(pasteText);
        setValWithCursor(parts.intPart, parts.decPart, formatIntegerPart(parts.intPart).length);
    });

    input.addEventListener("blur", function () {
        let parts = getParts(this.value);
        let formatted = formatIntegerPart(parts.intPart) + "," + parts.decPart;
        this.value = formatted;
        if (onChangeCallback) onChangeCallback();
    });
}

document.addEventListener('DOMContentLoaded', () => {
    // Attach direct money masks
    attachDirectMoneyMask(document.getElementById('txtValorPecas'), calculateTotal);
    attachDirectMoneyMask(document.getElementById('txtValorServico'), calculateTotal);
    attachDirectMoneyMask(document.getElementById('modalDiscountValue'), null);
    attachDirectMoneyMask(document.getElementById('modalCustoValue'), null);

    // IMEI constraint: numeric only and max 17 digits
    const txtImei = document.getElementById('txtImei');
    if (txtImei) {
        txtImei.addEventListener('input', function () {
            this.value = this.value.replace(/\D/g, '').slice(0, 17);
        });
    }

    // Dynamic brand selection listener
    const txtMarca = document.getElementById('txtMarca');
    if (txtMarca) {
        txtMarca.addEventListener('input', () => {
            updateModelosDatalist(txtMarca.value);
        });
        txtMarca.addEventListener('change', () => {
            updateModelosDatalist(txtMarca.value);
        });
        txtMarca.addEventListener('focus', function () {
            if (this.showPicker) { try { this.showPicker(); } catch (e) { } }
        });
    }

    const txtModelo = document.getElementById('txtModelo');
    if (txtModelo) {
        txtModelo.addEventListener('focus', function () {
            if (this.showPicker) { try { this.showPicker(); } catch (e) { } }
        });
    }

    // Initialize automatic suggestion dropdowns
    initTextosAutomaticos();
});

function sendAction(actionName, data = null) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: actionName, data: data });
    }
}

function parseMoney(val) {
    if (!val) return 0;
    if (typeof val === 'number') return val;
    return parseFloat(val.toString().replace('R$', '').replace(/\./g, '').replace(',', '.').trim()) || 0;
}

function formatMoney(val) {
    if (isNaN(val) || val === null || val === undefined) val = 0;
    return val.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function calculateTotal() {
    let partsVal = parseMoney(document.getElementById('txtValorPecas').value);
    let serviceVal = parseMoney(document.getElementById('txtValorServico').value);
    
    // Calculate total additional outsourced costs
    let totalCustosAdicionais = partsItems.reduce((acc, item) => acc + (item.CustoAdicional || 0), 0);
    
    let lucro = serviceVal - partsVal - totalCustosAdicionais;
    
    document.getElementById('lblLucro').innerText = formatMoney(lucro);
}

// Called from C#
function loadFormData(dataJson) {
    const data = JSON.parse(dataJson);
    
    // Combos
    clientesGlobal = data.Clientes || [];
    marcasDbGlobal = data.MarcasDb || [];
    modelosDbGlobal = data.ModelosDb || [];

    populateMarcasDatalist(marcasDbGlobal);
    updateModelosDatalist(data.OS ? (data.OS.Marca || '') : '');
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
        updateModelosDatalist(data.OS.Marca || '');
        document.getElementById('txtModelo').value = data.OS.Modelo || '';
        document.getElementById('txtImei').value = (data.OS.IMEI || '').replace(/\D/g, '').slice(0, 17);
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
        document.getElementById('txtValorPecas').value = '0,00';
        document.getElementById('txtValorServico').value = '0,00';
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
    if (part.SubTotal && part.SubTotal > 0) {
        let currentPecas = parseMoney(document.getElementById('txtValorPecas').value);
        document.getElementById('txtValorPecas').value = formatMoney(currentPecas + part.SubTotal);
    }
    renderParts();
}

let currentDiscountIndex = -1;

function applyDiscount(index) {
    let item = partsItems[index];
    currentDiscountIndex = index;
    
    document.getElementById('modalItemName').innerText = item.NomeProduto;
    document.getElementById('modalCurrentValue').value = formatMoney(item.SubTotal);
    const inputDesc = document.getElementById('modalDiscountValue');
    inputDesc.value = '0,00';
    
    document.getElementById('discountModal').classList.add('show');
    setTimeout(() => {
        inputDesc.focus();
        if (inputDesc.select) inputDesc.select();
    }, 50);
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
        let currentPecas = parseMoney(document.getElementById('txtValorPecas').value);
        let newPecas = Math.max(0, currentPecas - discount);
        document.getElementById('txtValorPecas').value = formatMoney(newPecas);
        renderParts();
        closeDiscountModal();
    } else if (discount > item.SubTotal) {
        alert("O valor do desconto não pode ser maior que o subtotal do item!");
    } else {
        alert("Valor de desconto inválido!");
    }
}

function openCustoModal() {
    const inputCusto = document.getElementById('modalCustoValue');
    inputCusto.value = '0,00';
    document.getElementById('custoModal').classList.add('show');
    setTimeout(() => {
        inputCusto.focus();
        if (inputCusto.select) inputCusto.select();
    }, 50);
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
    let item = partsItems[index];
    if (item && item.SubTotal > 0) {
        let currentPecas = parseMoney(document.getElementById('txtValorPecas').value);
        let newPecas = Math.max(0, currentPecas - item.SubTotal);
        document.getElementById('txtValorPecas').value = formatMoney(newPecas);
    }
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
        Marca: document.getElementById('txtMarca').value.trim(),
        Modelo: document.getElementById('txtModelo').value.trim(),
        IMEI: document.getElementById('txtImei').value.replace(/\D/g, '').slice(0, 17),
        Cor: document.getElementById('txtCor').value.trim(),
        Defeito: document.getElementById('txtDefeito').value,
        LaudoTecnico: document.getElementById('txtLaudo').value,
        Status: document.getElementById('cboStatus').value,
        ValorPecas: parseMoney(document.getElementById('txtValorPecas').value),
        ValorServico: parseMoney(document.getElementById('txtValorServico').value),
        Itens: partsItems
    };
    
    sendAction('SAVE_OS', osData);
}

// Logic for automatic text suggestions for reported problems and technical diagnosis
let lastAutoSuggestedLaudo = "";

function normalizeTextSearch(text) {
    if (!text) return "";
    return text.toString().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
}

function escapeHtml(text) {
    if (!text) return "";
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
}

function highlightMatch(fullText, query) {
    if (!query) return escapeHtml(fullText);
    const normFull = normalizeTextSearch(fullText);
    const normQuery = normalizeTextSearch(query);
    const idx = normFull.indexOf(normQuery);
    if (idx === -1) return escapeHtml(fullText);

    const before = fullText.slice(0, idx);
    const match = fullText.slice(idx, idx + query.length);
    const after = fullText.slice(idx + query.length);
    return `${escapeHtml(before)}<mark>${escapeHtml(match)}</mark>${escapeHtml(after)}`;
}

function initTextosAutomaticos() {
    const btnToggleDefeito = document.getElementById("btnToggleDefeito");
    const dropdownDefeito = document.getElementById("dropdownDefeito");
    const txtDefeito = document.getElementById("txtDefeito");

    const btnToggleLaudo = document.getElementById("btnToggleLaudo");
    const dropdownLaudo = document.getElementById("dropdownLaudo");
    const txtLaudo = document.getElementById("txtLaudo");

    if (!txtDefeito || !txtLaudo || !dropdownDefeito || !dropdownLaudo) return;

    // Helper to get items grouped by category
    function getCategorizedItems(filterText = "", searchField = "problema") {
        const query = normalizeTextSearch(filterText).trim();
        const groups = {};

        if (typeof TEXTOS_AUTOMATICOS_OS === "undefined" || !Array.isArray(TEXTOS_AUTOMATICOS_OS)) {
            return groups;
        }

        TEXTOS_AUTOMATICOS_OS.forEach(item => {
            const cat = item.categoria || "OUTROS";
            const matchProb = normalizeTextSearch(item.problema).includes(query);
            const matchLaudo = normalizeTextSearch(item.laudo).includes(query);
            const matchCat = normalizeTextSearch(cat).includes(query);

            let isMatch = false;
            if (!query) {
                isMatch = true;
            } else if (searchField === "problema") {
                isMatch = matchProb || matchCat;
            } else {
                isMatch = matchLaudo || matchProb || matchCat;
            }

            if (isMatch) {
                if (!groups[cat]) groups[cat] = [];
                groups[cat].push(item);
            }
        });

        return groups;
    }

    // Render dropdown for Defeito
    function renderDropdownDefeito(filterText = "", autoFocusSearch = false) {
        dropdownDefeito.innerHTML = "";

        // Search bar at the top of dropdown
        const searchBar = document.createElement("div");
        searchBar.className = "dropdown-search-bar";
        searchBar.innerHTML = `<input type="text" class="dropdown-filter-input" placeholder="Pesquisar problema... (ou role a lista abaixo)" value="${escapeHtml(filterText)}" autocomplete="off">`;
        dropdownDefeito.appendChild(searchBar);

        const filterInput = searchBar.querySelector(".dropdown-filter-input");

        // Scroll container for items
        const scrollContainer = document.createElement("div");
        scrollContainer.className = "dropdown-items-scroll";
        dropdownDefeito.appendChild(scrollContainer);

        function fillItems(currentQuery) {
            scrollContainer.innerHTML = "";
            const groups = getCategorizedItems(currentQuery, "problema");
            const categories = Object.keys(groups);

            if (categories.length === 0) {
                scrollContainer.innerHTML = `<div class="suggest-empty">Nenhum problema encontrado para "${escapeHtml(currentQuery)}"</div>`;
                return;
            }

            categories.forEach(cat => {
                const header = document.createElement("div");
                header.className = "suggest-category-header";
                header.textContent = cat;
                scrollContainer.appendChild(header);

                groups[cat].forEach(item => {
                    const el = document.createElement("div");
                    el.className = "suggest-item";
                    el.innerHTML = highlightMatch(item.problema, currentQuery);
                    el.addEventListener("mousedown", (e) => {
                        e.preventDefault();
                        selectDefeitoItem(item);
                    });
                    scrollContainer.appendChild(el);
                });
            });
        }

        fillItems(filterText);

        filterInput.addEventListener("input", (e) => {
            fillItems(e.target.value);
        });

        filterInput.addEventListener("keydown", (e) => {
            if (e.key === "Escape") {
                dropdownDefeito.style.display = "none";
            }
        });

        dropdownDefeito.style.display = "flex";

        if (autoFocusSearch) {
            setTimeout(() => {
                filterInput.focus();
                if (filterInput.value) filterInput.select();
            }, 50);
        }
    }

    // Select an item from Defeito dropdown
    function selectDefeitoItem(item) {
        txtDefeito.value = item.problema;
        dropdownDefeito.style.display = "none";

        // Auto suggest corresponding technical report if current laudo is empty or matches previous auto suggestion
        const currentLaudo = txtLaudo.value.trim();
        if (!currentLaudo || currentLaudo === lastAutoSuggestedLaudo) {
            txtLaudo.value = item.laudo;
            lastAutoSuggestedLaudo = item.laudo;
        }
    }

    // Render dropdown for Laudo
    function renderDropdownLaudo(filterText = "", autoFocusSearch = false) {
        dropdownLaudo.innerHTML = "";

        // Search bar at the top of dropdown
        const searchBar = document.createElement("div");
        searchBar.className = "dropdown-search-bar";
        searchBar.innerHTML = `<input type="text" class="dropdown-filter-input" placeholder="Pesquisar laudo... (ou role a lista abaixo)" value="${escapeHtml(filterText)}" autocomplete="off">`;
        dropdownLaudo.appendChild(searchBar);

        const filterInput = searchBar.querySelector(".dropdown-filter-input");

        // Scroll container for items
        const scrollContainer = document.createElement("div");
        scrollContainer.className = "dropdown-items-scroll";
        dropdownLaudo.appendChild(scrollContainer);

        function fillItems(currentQuery) {
            scrollContainer.innerHTML = "";
            const groups = getCategorizedItems(currentQuery, "laudo");
            const categories = Object.keys(groups);

            if (categories.length === 0) {
                scrollContainer.innerHTML = `<div class="suggest-empty">Nenhum laudo encontrado para "${escapeHtml(currentQuery)}"</div>`;
                return;
            }

            categories.forEach(cat => {
                const header = document.createElement("div");
                header.className = "suggest-category-header";
                header.textContent = cat;
                scrollContainer.appendChild(header);

                groups[cat].forEach(item => {
                    const el = document.createElement("div");
                    el.className = "suggest-item";
                    el.innerHTML = `
                        <div class="laudo-item-title">${highlightMatch(item.problema, currentQuery)}</div>
                        <div class="laudo-item-text">${highlightMatch(item.laudo, currentQuery)}</div>
                    `;
                    el.addEventListener("mousedown", (e) => {
                        e.preventDefault();
                        selectLaudoItem(item);
                    });
                    scrollContainer.appendChild(el);
                });
            });
        }

        fillItems(filterText);

        filterInput.addEventListener("input", (e) => {
            fillItems(e.target.value);
        });

        filterInput.addEventListener("keydown", (e) => {
            if (e.key === "Escape") {
                dropdownLaudo.style.display = "none";
            }
        });

        dropdownLaudo.style.display = "flex";

        if (autoFocusSearch) {
            setTimeout(() => {
                filterInput.focus();
                if (filterInput.value) filterInput.select();
            }, 50);
        }
    }

    // Select an item from Laudo dropdown
    function selectLaudoItem(item) {
        txtLaudo.value = item.laudo;
        lastAutoSuggestedLaudo = item.laudo;
        dropdownLaudo.style.display = "none";
    }

    // Toggle button listeners
    if (btnToggleDefeito) {
        btnToggleDefeito.addEventListener("click", () => {
            dropdownLaudo.style.display = "none";
            if (dropdownDefeito.style.display === "flex") {
                dropdownDefeito.style.display = "none";
            } else {
                renderDropdownDefeito("", true);
            }
        });
    }

    if (btnToggleLaudo) {
        btnToggleLaudo.addEventListener("click", () => {
            dropdownDefeito.style.display = "none";
            if (dropdownLaudo.style.display === "flex") {
                dropdownLaudo.style.display = "none";
            } else {
                renderDropdownLaudo("", true);
            }
        });
    }

    // Also support typing directly inside txtDefeito
    txtDefeito.addEventListener("input", () => {
        const val = txtDefeito.value.trim();
        if (val.length >= 2) {
            dropdownLaudo.style.display = "none";
            renderDropdownDefeito(val, false);
        } else {
            dropdownDefeito.style.display = "none";
        }
    });

    // Also support typing directly inside txtLaudo
    txtLaudo.addEventListener("input", () => {
        const val = txtLaudo.value.trim();
        if (val.length >= 2) {
            dropdownDefeito.style.display = "none";
            renderDropdownLaudo(val, false);
        } else {
            dropdownLaudo.style.display = "none";
        }
    });

    // Close on outside click
    document.addEventListener("click", (e) => {
        if (!e.target.closest("#boxDefeito") && !e.target.closest("#btnToggleDefeito")) {
            dropdownDefeito.style.display = "none";
        }
        if (!e.target.closest("#boxLaudo") && !e.target.closest("#btnToggleLaudo")) {
            dropdownLaudo.style.display = "none";
        }
    });

    // Close on Escape anywhere
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            dropdownDefeito.style.display = "none";
            dropdownLaudo.style.display = "none";
        }
    });
}
