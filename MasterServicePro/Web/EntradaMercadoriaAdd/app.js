let itens = [];
let valorTotal = 0;

// Integração com WebView2
function sendToHost(message) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage(message);
    } else {
        console.log('DEV MSG:', message);
    }
}

// Receber mensagens do C#
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        try {
            const data = JSON.parse(event.data);
            if (data.action === "load_fornecedores") {
                renderFornecedores(data.fornecedores);
            } else if (data.action === "produto_selecionado") {
                solicitarQuantidadePreco(data.produto);
            } else if (data.action === "resultados_busca") {
                renderResultadosBusca(data.produtos);
            }
        } catch (e) {
            console.error("Erro ao fazer parse da mensagem:", e);
        }
    });
}

function renderFornecedores(fornecedores) {
    const cbo = document.getElementById('cboFornecedores');
    let html = '<option value="">Selecione um fornecedor...</option>';
    if (fornecedores) {
        fornecedores.forEach(f => {
            html += `<option value="${f.Id}">${f.Nome}</option>`;
        });
    }
    cbo.innerHTML = html;
}

function adicionarItem() {
    // Abre a modal HTML
    const modal = document.getElementById('modal-busca-produto');
    const content = document.getElementById('modal-busca-content');
    const input = document.getElementById('txtBuscaProduto');
    
    modal.classList.remove('hidden');
    modal.classList.add('flex');
    
    // Pequeno delay para a animação de entrada
    setTimeout(() => {
        modal.classList.remove('opacity-0');
        content.classList.remove('scale-95');
        input.value = '';
        input.focus();
        pesquisarProduto(''); // Busca inicial vazia
    }, 10);
}

function fecharBuscaProduto() {
    const modal = document.getElementById('modal-busca-produto');
    const content = document.getElementById('modal-busca-content');
    
    modal.classList.add('opacity-0');
    content.classList.add('scale-95');
    
    setTimeout(() => {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    }, 200); // Tempo da transição
}

let buscaTimeout = null;
function pesquisarProduto(query) {
    clearTimeout(buscaTimeout);
    buscaTimeout = setTimeout(() => {
        sendToHost({ action: 'buscar_produto', query: query });
    }, 300);
}

function renderResultadosBusca(produtos) {
    const container = document.getElementById('lista-produtos-busca');
    
    if (!produtos || produtos.length === 0) {
        container.innerHTML = `
            <div class="flex flex-col items-center justify-center h-full text-slate-500 py-10">
                <i class="fas fa-box-open text-3xl mb-2 opacity-50"></i>
                <p class="text-sm">Nenhum produto encontrado.</p>
            </div>
        `;
        return;
    }
    
    let html = '';
    produtos.forEach(p => {
        const marcaStr = p.Marca ? `<span class="text-xs text-slate-500">${p.Marca}</span>` : '';
        const codigoStr = p.CodigoBarras ? `<span class="text-xs px-2 py-0.5 bg-slate-700 rounded text-slate-400">${p.CodigoBarras}</span>` : '';
        const precoStr = p.PrecoCusto ? p.PrecoCusto.toLocaleString('pt-BR', {style: 'currency', currency: 'BRL'}) : 'R$ 0,00';
        
        // Passamos as propriedades essenciais
        const prodJson = JSON.stringify({
            Id: p.Id,
            Nome: p.Nome,
            PrecoCusto: p.PrecoCusto
        }).replace(/"/g, '&quot;');
        
        html += `
            <div class="flex items-center justify-between p-3 border-b border-slate-700/50 hover:bg-slate-700/50 cursor-pointer rounded transition-colors group" onclick="selecionarProdutoBusca(${prodJson})">
                <div class="flex flex-col gap-1">
                    <span class="text-white font-medium group-hover:text-primary-400 transition-colors">${p.Nome}</span>
                    <div class="flex items-center gap-2">
                        ${codigoStr}
                        ${marcaStr}
                    </div>
                </div>
                <div class="flex flex-col items-end">
                    <span class="text-emerald-400 font-semibold">${precoStr}</span>
                    <span class="text-xs text-slate-500">Estoque: <span class="${p.EstoqueAtual > 0 ? 'text-slate-300' : 'text-danger-400'}">${p.EstoqueAtual}</span></span>
                </div>
            </div>
        `;
    });
    
    container.innerHTML = html;
}

function selecionarProdutoBusca(produto) {
    fecharBuscaProduto();
    solicitarQuantidadePreco(produto);
}

function solicitarQuantidadePreco(produto) {
    Swal.fire({
        title: produto.Nome,
        html: `
            <div class="text-left">
                <label class="block text-sm text-slate-400 mb-1 mt-4">Quantidade</label>
                <input id="swal-qtd" type="number" min="1" step="1" value="1" class="swal2-input w-full mx-0 h-10 px-3 text-sm" style="max-width: 100%;">
                
                <label class="block text-sm text-slate-400 mb-1 mt-4">Preço de Custo Unitário (R$)</label>
                <input id="swal-preco" type="number" min="0" step="0.01" value="${produto.PrecoCusto.toFixed(2)}" class="swal2-input w-full mx-0 h-10 px-3 text-sm" style="max-width: 100%;">
            </div>
        `,
        focusConfirm: false,
        showCancelButton: true,
        confirmButtonText: 'Adicionar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#6366f1',
        cancelButtonColor: '#334155',
        preConfirm: () => {
            const qtd = parseInt(document.getElementById('swal-qtd').value);
            const preco = parseFloat(document.getElementById('swal-preco').value);
            
            if (isNaN(qtd) || qtd <= 0) {
                Swal.showValidationMessage('A quantidade deve ser maior que 0');
                return false;
            }
            if (isNaN(preco) || preco < 0) {
                Swal.showValidationMessage('O preço de custo é inválido');
                return false;
            }
            
            return { quantidade: qtd, precoCusto: preco };
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const input = result.value;
            
            // Verifica se o produto já existe na lista
            const existente = itens.find(i => i.IdProduto === produto.Id);
            if (existente) {
                existente.Quantidade += input.quantidade;
                existente.PrecoCusto = input.precoCusto; // Atualiza pro preço digitado agora
                existente.SubTotal = existente.Quantidade * existente.PrecoCusto;
            } else {
                itens.push({
                    IdProduto: produto.Id,
                    NomeProduto: produto.Nome,
                    Quantidade: input.quantidade,
                    PrecoCusto: input.precoCusto,
                    SubTotal: input.quantidade * input.precoCusto
                });
            }
            
            renderItens();
        }
    });
}

function removerItem(idProduto) {
    itens = itens.filter(i => i.IdProduto !== idProduto);
    renderItens();
}

function formatMoney(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value || 0);
}

function renderItens() {
    const tbody = document.getElementById('tbody-itens');
    const emptyState = document.getElementById('empty-state');
    const lblTotal = document.getElementById('lblTotal');
    
    if (itens.length === 0) {
        tbody.innerHTML = '';
        emptyState.classList.remove('hidden');
        lblTotal.innerText = 'R$ 0,00';
        valorTotal = 0;
        return;
    }
    
    emptyState.classList.add('hidden');
    
    valorTotal = itens.reduce((sum, item) => sum + item.SubTotal, 0);
    lblTotal.innerText = formatMoney(valorTotal);
    
    let html = '';
    itens.forEach(item => {
        html += `
            <tr class="border-b border-slate-700/50 hover:bg-slate-700/30 transition-colors">
                <td class="py-3 px-4 text-slate-200 font-medium">${item.NomeProduto}</td>
                <td class="py-3 px-4 text-slate-300 text-center">${item.Quantidade}</td>
                <td class="py-3 px-4 text-slate-300 text-right">${formatMoney(item.PrecoCusto)}</td>
                <td class="py-3 px-4 text-white font-semibold text-right">${formatMoney(item.SubTotal)}</td>
                <td class="py-3 px-4 text-center">
                    <button onclick="removerItem(${item.IdProduto})" class="text-danger-500 hover:text-danger-400 bg-slate-800 hover:bg-slate-700 w-8 h-8 rounded border border-slate-600 transition-colors" title="Remover">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>
        `;
    });
    
    tbody.innerHTML = html;
}

function gravarEntrada() {
    const idFornecedor = document.getElementById('cboFornecedores').value;
    const notaFiscal = document.getElementById('txtNota').value.trim();
    const observacao = document.getElementById('txtObs').value.trim();
    
    if (!idFornecedor) {
        Swal.fire('Atenção', 'Selecione um fornecedor!', 'warning');
        return;
    }
    
    if (itens.length === 0) {
        Swal.fire('Atenção', 'Adicione pelo menos um item à entrada!', 'warning');
        return;
    }
    
    const payload = {
        IdFornecedor: parseInt(idFornecedor),
        NumeroNota: notaFiscal,
        Observacao: observacao,
        ValorTotal: valorTotal,
        Itens: itens
    };
    
    sendToHost({ action: 'gravar', payload: payload });
}

function fechar() {
    sendToHost({ action: 'fechar' });
}

// Request inicial
document.addEventListener('DOMContentLoaded', () => {
    sendToHost({ action: 'request_init' });
});
