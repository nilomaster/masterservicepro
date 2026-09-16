let isEdit = false;

window.loadData = function (data) {
    if (data.isEdit) {
        isEdit = true;
        document.getElementById("pageTitle").innerText = "Editar Usuário";
        
        const u = data.usuario;
        document.getElementById("username").value = u.Username || "";
        document.getElementById("nivel").value = u.Nivel || "Funcionário";
        
        if (u.Username && u.Username.toLowerCase() === "admin") {
            document.getElementById("nivel").disabled = true;
        }
    }
};

document.getElementById('btnCancel').addEventListener('click', () => {
    window.chrome.webview.postMessage({ action: "CANCEL" });
});

document.getElementById('btnConfirm').addEventListener('click', () => {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;
    const nivel = document.getElementById("nivel").value;

    if (username === "") {
        alert("O nome de usuário é obrigatório!");
        return;
    }

    if (!isEdit && password === "") {
        alert("A senha é obrigatória para um novo usuário!");
        return;
    }

    const data = {
        Username: username,
        Senha: password,
        Nivel: nivel
    };

    window.chrome.webview.postMessage({ action: "SAVE", usuario: data });
});

