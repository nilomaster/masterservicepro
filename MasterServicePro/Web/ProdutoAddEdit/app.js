let currentBase64 = null;
let currentFileName = null;

window.loadData = function (data) {
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

