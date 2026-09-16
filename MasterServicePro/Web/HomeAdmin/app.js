// app.js

let barChartInstance = null;
let pieChartInstance = null;

// Formatação de Moeda
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value || 0);
}

function renderTrend(elementId, percentage) {
    const el = document.getElementById(elementId);
    if (!el) return;
    if (percentage > 0) {
        el.innerHTML = `↑ +${percentage.toFixed(1)}% vs anterior`;
        el.className = 'trend-indicator trend-up';
    } else if (percentage < 0) {
        el.innerHTML = `↓ ${percentage.toFixed(1)}% vs anterior`;
        el.className = 'trend-indicator trend-down';
    } else {
        el.innerHTML = `- igual ao anterior`;
        el.className = 'trend-indicator trend-neutral';
    }
}

// Configuração padrão do Chart.js para Dark Theme
Chart.defaults.color = '#94A3B8';
Chart.defaults.font.family = "'Inter', sans-serif";

function renderBarChart(labels, data) {
    const ctx = document.getElementById('barChart').getContext('2d');
    
    if (barChartInstance) {
        barChartInstance.destroy();
    }

    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(99, 102, 241, 0.8)');
    gradient.addColorStop(1, 'rgba(99, 102, 241, 0.2)');

    barChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Faturamento Diário',
                data: data,
                backgroundColor: gradient,
                borderColor: '#6366F1',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return formatCurrency(context.raw);
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(51, 56, 69, 0.5)' },
                    ticks: {
                        callback: function(value) {
                            return 'R$ ' + value;
                        }
                    }
                },
                x: {
                    grid: { display: false }
                }
            }
        }
    });
}

function renderPieChart(pieVendas, pieOs) {
    const ctx = document.getElementById('pieChart').getContext('2d');
    
    if (pieChartInstance) {
        pieChartInstance.destroy();
    }

    pieChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Vendas Diretas', 'Ordens de Serviço'],
            datasets: [{
                data: [pieVendas, pieOs],
                backgroundColor: [
                    '#22C55E', // Green
                    '#6366F1'  // Indigo
                ],
                borderWidth: 0,
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { padding: 20 }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + formatCurrency(context.raw);
                        }
                    }
                }
            }
        }
    });
}

// Comunicação com C#
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        try {
            const msg = typeof event.data === 'string' ? JSON.parse(event.data) : event.data;
            
            if (msg.action === 'load_metrics') {
                // Preenche os cards
                document.getElementById('valDiaria').innerText = formatCurrency(msg.diaria);
                document.getElementById('valSemanal').innerText = formatCurrency(msg.semanal);
                document.getElementById('valMensal').innerText = formatCurrency(msg.mensal);

                // Preenche trends
                renderTrend('trendDiaria', msg.percDiaria);
                renderTrend('trendSemanal', msg.percSemanal);
                renderTrend('trendMensal', msg.percMensal);

                // Preenche os alertas
                document.getElementById('lblEstoqueBaixo').innerText = msg.estoqueBaixo + " produtos em falta";
                document.getElementById('lblOsAtraso').innerText = msg.osAtraso + " ordens paradas";
                document.getElementById('lblContasHoje').innerText = msg.contasHoje + " vencendo hoje";

                // Renderiza os gráficos
                renderBarChart(msg.barLabels, msg.barData);
                renderPieChart(msg.pieVendas, msg.pieOs);
            }
        } catch(e) { console.error(e); }
    });
}

function refreshData() {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'refresh_data' });
    }
}

function abrirCorrecoes() {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: 'abrir_correcoes' });
    }
}
