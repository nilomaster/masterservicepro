window.loadData = function (data) {
    if (data.id > 0) {
        document.getElementById("pageTitle").innerText = "Editar Cliente";
    }

    if (data.cliente) {
        const c = data.cliente;
        document.getElementById("nome").value = c.Nome || "";
        document.getElementById("cpf").value = formatCpfCnpj(c.CpfCnpj || "");
        document.getElementById("telefone").value = formatPhone(c.Telefone || "");
        document.getElementById("whatsapp").value = formatPhone(c.WhatsApp || "");
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

// Dynamic mask for CPF (11 digits) or CNPJ (14 digits)
function formatCpfCnpj(val) {
    if (!val) return '';
    if (val.trim().toUpperCase() === 'N/A') return 'N/A';
    const digits = val.replace(/\D/g, '').slice(0, 14);
    if (!digits) return '';

    if (digits.length <= 11) {
        let s = digits;
        if (digits.length > 3) s = digits.slice(0, 3) + '.' + digits.slice(3);
        if (digits.length > 6) s = s.slice(0, 7) + '.' + digits.slice(6);
        if (digits.length > 9) s = s.slice(0, 11) + '-' + digits.slice(9);
        return s;
    } else {
        let s = digits;
        s = digits.slice(0, 2) + '.' + digits.slice(2);
        if (digits.length > 5) s = s.slice(0, 6) + '.' + digits.slice(5);
        if (digits.length > 8) s = s.slice(0, 10) + '/' + digits.slice(8);
        if (digits.length > 12) s = s.slice(0, 15) + '-' + digits.slice(12);
        return s;
    }
}

// Dynamic mask for landline (10 digits) or mobile (11 digits)
function formatPhone(val) {
    if (!val) return '';
    if (val.trim().toUpperCase() === 'N/A') return 'N/A';
    const digits = val.replace(/\D/g, '').slice(0, 11);
    if (!digits) return '';

    if (digits.length <= 2) {
        return `(${digits}`;
    }
    if (digits.length <= 6) {
        return `(${digits.slice(0, 2)}) ${digits.slice(2)}`;
    }
    if (digits.length <= 10) {
        return `(${digits.slice(0, 2)}) ${digits.slice(2, 6)}-${digits.slice(6)}`;
    }
    return `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7)}`;
}

// Attach mask listeners with cursor position retention
function bindMask(element, maskFn) {
    if (!element) return;
    
    element.addEventListener('input', function () {
        if (this.value.trim().toUpperCase() === 'N/A') return;
        
        const start = this.selectionStart;
        const prevLen = this.value.length;
        const formatted = maskFn(this.value);
        this.value = formatted;
        
        if (document.activeElement === this && start !== null) {
            const diff = formatted.length - prevLen;
            const newPos = Math.max(0, start + diff);
            this.setSelectionRange(newPos, newPos);
        }
    });

    element.addEventListener('blur', function () {
        if (this.value.trim().toUpperCase() === 'N/A') return;
        this.value = maskFn(this.value);
    });
}

// Bind masks to inputs
const inputCpf = document.getElementById('cpf');
const inputTel = document.getElementById('telefone');
const inputWpp = document.getElementById('whatsapp');

bindMask(inputCpf, formatCpfCnpj);
bindMask(inputTel, formatPhone);
bindMask(inputWpp, formatPhone);

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
        if (el && el.value.trim() === "") el.value = "N/A";
    });
});

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const nome = document.getElementById("nome").value.trim();
    if (nome === "") {
        alert("O nome do cliente e obrigatorio.");
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

