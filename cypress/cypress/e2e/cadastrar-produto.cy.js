function gerarStringAleatoria(tamanho) {
  const caracteres = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let resultado = '';
  
  for (let i = 0; i < tamanho; i++) {
    const indice = Math.floor(Math.random() * caracteres.length);
    resultado += caracteres[indice];
  }
  
  return resultado;
}

function gerarNumeroAleatorio(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function gerarPrecoAleatorio(min, max) {
  const preco = (Math.random() * (max - min) + min).toFixed(2);
  return parseFloat(preco);
}

function gerarDataAleatoriaFormatada(inicio, fim) {
  const inicioTimestamp = new Date(inicio).getTime();
  const fimTimestamp = new Date(fim).getTime();
  const timestampAleatorio = Math.floor(Math.random() * (fimTimestamp - inicioTimestamp + 1)) + inicioTimestamp;
  const dataAleatoria = new Date(timestampAleatorio);

  // Formatar como "YYYY-MM-DD"
  const ano = dataAleatoria.getFullYear();
  const mes = String(dataAleatoria.getMonth() + 1).padStart(2, '0'); // Mês começa em 0, então somamos 1
  const dia = String(dataAleatoria.getDate()).padStart(2, '0'); // Garantir que tenha dois dígitos

  return `${ano}-${mes}-${dia}`;
}


describe('template spec', () => {
  it('passes', () => {
    cy.visit('http://127.0.0.1:5500/cadastro.html')
    const id = gerarNumeroAleatorio(1, 10)
    const nome = gerarStringAleatoria(6)
    const preco = gerarPrecoAleatorio(5.50, 10)
    const qtd_estoque = gerarNumeroAleatorio (1, 100)
    const data = gerarDataAleatoriaFormatada('2024-10-01', '2024-10-31')

    cy.get('#registerId').type(id)
    cy.get('#registerName').type(nome)
    cy.get('#registerPrice').type(preco)
    cy.get('#registerQtd').type(qtd_estoque)
    cy.get('input[type="date"]').type(data);

    cy.get('#btnClick').click()
    cy.get('h2').should('contain', 'Cadastrar')  
  })
})