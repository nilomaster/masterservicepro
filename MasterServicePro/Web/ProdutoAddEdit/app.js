let currentBase64 = null;
let currentFileName = null;

// Catalog of standard cellphone brands and their popular models (ASCII only)
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

function populateMarcasDatalist() {
    const dl = document.getElementById("marcasList");
    if (!dl) return;
    dl.innerHTML = "";
    Object.keys(CATALOGO_MARCAS_MODELOS).sort().forEach(m => {
        const opt = document.createElement("option");
        opt.value = m;
        dl.appendChild(opt);
    });
}

function updateModelosDatalist(selectedMarca) {
    const dl = document.getElementById("modelosList");
    if (!dl) return;
    dl.innerHTML = "";
    const clean = (selectedMarca || "").trim().toLowerCase();
    const matched = Object.keys(CATALOGO_MARCAS_MODELOS).find(k => k.toLowerCase() === clean);
    const list = matched ? CATALOGO_MARCAS_MODELOS[matched] : Object.values(CATALOGO_MARCAS_MODELOS).flat();
    Array.from(new Set(list)).sort().forEach(mod => {
        const opt = document.createElement("option");
        opt.value = mod;
        dl.appendChild(opt);
    });
}

window.loadData = function (data) {
    populateMarcasDatalist();
    updateModelosDatalist(data.produto ? data.produto.Marca : "");
    if (data.id > 0) {
        document.getElementById("pageTitle").innerText = "Editar Produto";
    }

    const cboCategoria = document.getElementById("categoria");
    data.categorias.forEach(c => {
        const option = document.createElement("option");
        option.value = c.Id;
        option.innerText = c.Nome;
        cboCategoria.appendChild(option);
    });

    if (data.fornecedores) {
        const cboFornecedor = document.getElementById("fornecedor");
        data.fornecedores.forEach(f => {
            const option = document.createElement("option");
            option.value = f.Id;
            option.innerText = f.Nome;
            cboFornecedor.appendChild(option);
        });
    }

    if (data.produto) {
        const p = data.produto;
        document.getElementById("nome").value = p.Nome || "";
        document.getElementById("codigoBarras").value = p.CodigoBarras || "";
        document.getElementById("marca").value = p.Marca || "";
        updateModelosDatalist(p.Marca || "");
        document.getElementById("modelo").value = p.Modelo || "";
        if (p.IdCategoria) document.getElementById("categoria").value = p.IdCategoria;
        if (p.IdFornecedor) document.getElementById("fornecedor").value = p.IdFornecedor;
        if (p.Garantia) document.getElementById("garantia").value = p.Garantia;
        
        document.getElementById("precoCusto").value = formatMoneyInput(p.PrecoCusto);
        document.getElementById("precoVenda").value = formatMoneyInput(p.PrecoVenda);
        document.getElementById("estoque").value = p.Estoque || 0;
        document.getElementById("estoqueMinimo").value = p.EstoqueMinimo || 0;

        if (p.ImagemUrlBase64) {
            const preview = document.getElementById("imagemPreview");
            const icon = document.getElementById("previewIcon");
            const text = document.getElementById("previewText");
            preview.src = p.ImagemUrlBase64;
            preview.style.display = "block";
            icon.style.display = "none";
            if (text) text.style.display = "none";
        }
    }
};

function formatMoneyInput(val) {
    if (val === undefined || val === null) val = 0;
    let s = val.toFixed(2).replace(".", ",");
    return s.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
}

function handleMoneyInput(e) {
    let value = e.target.value.replace(/\D/g, '');
    if (value === "") value = "0";
    value = (parseInt(value, 10) / 100).toFixed(2) + '';
    value = value.replace(".", ",");
    value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    e.target.value = value;
}

document.getElementById('precoCusto').addEventListener('input', handleMoneyInput);
document.getElementById('precoVenda').addEventListener('input', handleMoneyInput);

const inputMarca = document.getElementById('marca');
if (inputMarca) {
    inputMarca.addEventListener('input', () => updateModelosDatalist(inputMarca.value));
    inputMarca.addEventListener('change', () => updateModelosDatalist(inputMarca.value));
}

document.getElementById('imagemProduto').addEventListener('change', function(e) {
    const file = e.target.files[0];
    if (file) {
        currentFileName = file.name;
        const reader = new FileReader();
        reader.onload = function(event) {
            currentBase64 = event.target.result;
            const preview = document.getElementById("imagemPreview");
            const icon = document.getElementById("previewIcon");
            const text = document.getElementById("previewText");
            preview.src = currentBase64;
            preview.style.display = "block";
            icon.style.display = "none";
            if (text) text.style.display = "none";
        };
        reader.readAsDataURL(file);
    } else {
        currentBase64 = null;
        currentFileName = null;
        document.getElementById("imagemPreview").style.display = "none";
        document.getElementById("previewIcon").style.display = "block";
        const text = document.getElementById("previewText");
        if (text) text.style.display = "block";
    }
});

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnGerarCodigo').addEventListener('click', () => {
    const rnd = Math.floor(Math.random() * (999999999 - 100000000 + 1)) + 100000000;
    document.getElementById('codigoBarras').value = "789" + rnd;
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const nome = document.getElementById("nome").value.trim();
    if (nome === "") {
        alert("O nome do produto é obrigatório.");
        return;
    }

    const parseMoney = (id) => {
        const val = document.getElementById(id).value;
        return parseFloat(val.replace(/\./g, '').replace(',', '.'));
    };

    const data = {
        Nome: nome,
        CodigoBarras: document.getElementById("codigoBarras").value.trim(),
        Marca: document.getElementById("marca").value.trim(),
        Modelo: document.getElementById("modelo").value.trim(),
        IdCategoria: parseInt(document.getElementById("categoria").value),
        IdFornecedor: parseInt(document.getElementById("fornecedor").value) || null,
        Garantia: document.getElementById("garantia").value,
        PrecoCusto: parseMoney("precoCusto"),
        PrecoVenda: parseMoney("precoVenda"),
        Estoque: parseInt(document.getElementById("estoque").value) || 0,
        EstoqueMinimo: parseInt(document.getElementById("estoqueMinimo").value) || 0,
        ImagemBase64: currentBase64,
        ImagemFileName: currentFileName
    };

    window.chrome.webview.postMessage({ action: "SAVE", produto: data });
});

