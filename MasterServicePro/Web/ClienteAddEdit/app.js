window.loadData = function (data) {
    if (data.id > 0) {
        document.getElementById("pageTitle").innerText = "Editar Cliente";
    }

    if (data.cliente) {
        const c = data.cliente;
        document.getElementById("nome").value = c.Nome || "";
        document.getElementById("cpf").value = c.CpfCnpj || "";
        document.getElementById("telefone").value = c.Telefone || "";
        document.getElementById("whatsapp").value = c.WhatsApp || "";
        document.getElementById("email").value = c.Email || "";
        document.getElementById("endereco").value = c.Endereco || "";
        document.getElementById("historico").value = c.Historico || "";
        document.getElementById("saldo").value = formatMoneyInput(c.Saldo);
    }
};

function formatMoneyInput(val) {
    if (val === undefined || val === null) val = 0;
    let s = val.toFixed(2).replace(".", ",");
    return s.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
}

document.getElementById('saldo').addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');
    if (value === "") value = "0";
    value = (parseInt(value, 10) / 100).toFixed(2) + '';
    value = value.replace(".", ",");
    value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    e.target.value = value;
});

document.getElementById('btnFillNA').addEventListener('click', () => {
    const fields = ['cpf', 'telefone', 'whatsapp', 'email', 'endereco', 'historico'];
    fields.forEach(f => {
        const el = document.getElementById(f);
        if (el.value.trim() === "") el.value = "N/A";
    });
});

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const nome = document.getElementById("nome").value.trim();
    if (nome === "") {
        alert("O nome do cliente é obrigatório.");
        return;
    }

    const parseMoney = (id) => {
        const val = document.getElementById(id).value;
        return parseFloat(val.replace(/\./g, '').replace(',', '.'));
    };

    const data = {
        Nome: nome,
        CpfCnpj: document.getElementById("cpf").value.trim(),
        Telefone: document.getElementById("telefone").value.trim(),
        WhatsApp: document.getElementById("whatsapp").value.trim(),
        Email: document.getElementById("email").value.trim(),
        Endereco: document.getElementById("endereco").value.trim(),
        Historico: document.getElementById("historico").value.trim(),
        Saldo: parseMoney("saldo")
    };

    window.chrome.webview.postMessage({ action: "SAVE", cliente: data });
});

