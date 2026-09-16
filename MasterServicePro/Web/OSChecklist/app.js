const itemsToTest = [
    "Touchscreen / Display",
    "Vidro / Carcaça",
    "Câmera Frontal",
    "Câmera Traseira",
    "Conector de Carga",
    "Microfone",
    "Alto-falante",
    "Botões Físicos",
    "Wi-Fi / Bluetooth",
    "Sinal de Chip",
    "Biometria / FaceID",
    "Bateria / Saúde"
];

let states = {};

document.addEventListener('DOMContentLoaded', () => {
    // Initialize default states
    itemsToTest.forEach(item => states[item] = "Não Testado");
    renderGrid();
});

function sendAction(actionName, data = null) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: actionName, data: data });
    }
}

// Called from C#
function loadData(dataString) {
    if (!dataString) return;
    
    let mainPart = dataString;
    let obs = "";
    
    const obsIdx = dataString.indexOf("[OBS]");
    if (obsIdx >= 0) {
        mainPart = dataString.substring(0, obsIdx);
        obs = dataString.substring(obsIdx + 5);
    }
    
    document.getElementById('txtObs').value = obs;
    
    const pairs = mainPart.split('|').filter(p => p);
    pairs.forEach(p => {
        const parts = p.split(':');
        if (parts.length === 2 && states[parts[0]] !== undefined) {
            states[parts[0]] = parts[1];
        }
    });
    
    renderGrid();
}

function renderGrid() {
    const grid = document.getElementById('checklistGrid');
    grid.innerHTML = '';
    
    itemsToTest.forEach(item => {
        const state = states[item];
        let stateClass = 'status-nao-testado';
        if (state === 'OK') stateClass = 'status-ok';
        if (state === 'Defeito') stateClass = 'status-defeito';
        
        const div = document.createElement('div');
        div.className = 'check-item';
        div.innerHTML = `
            <span>${item}</span>
            <button class="status-btn ${stateClass}" onclick="toggleState('${item}')">${state}</button>
        `;
        grid.appendChild(div);
    });
}

function toggleState(item) {
    const current = states[item];
    if (current === 'Não Testado') states[item] = 'OK';
    else if (current === 'OK') states[item] = 'Defeito';
    else states[item] = 'Não Testado';
    
    renderGrid();
}

function saveChecklist() {
    let list = [];
    itemsToTest.forEach(item => {
        list.push(`${item}:${states[item]}`);
    });
    
    const result = list.join('|') + "[OBS]" + document.getElementById('txtObs').value.trim();
    sendAction('SAVE', result);
}
