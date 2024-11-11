describe('Testes de Estoque após Compra', () => {
  const baseUrl = 'http://lb-519e665-154492853.us-east-1.elb.amazonaws.com:80';
  const skuToBuy = 'DFHD6853I2244';
  let initialQuantity;

  // Primeiro, obtenha a quantidade inicial do produto no endpoint /inventory
  it('Teste GET /inventory - Verifica quantidade inicial e loja', () => {
    cy.request(`${baseUrl}/inventory`).then((response) => {
      // Verifica se a resposta tem o status 200 (sucesso)
      expect(response.status).to.eq(200);

      // Encontre o produto específico e armazene a quantidade inicial
      const product = response.body.find((product) => product.sku === skuToBuy);
      expect(product).to.exist;

      initialQuantity = product.quantity;
      const officeNum = product.officeNum; // Armazena o número da loja (officeNum)

      // Exibe a loja e a quantidade inicial
      cy.log(`Quantidade inicial do produto: ${initialQuantity} na loja: ${officeNum}`);
    });
  });

  // Depois, realize a compra (POST /inventory/process-cart)
  it('Teste POST /inventory/process-cart - Realiza a compra', () => {
    const cart = [
      {
        sku: skuToBuy,
        loja: 'LOJ5',
        quantity: 1, // Comprando 1 unidade do produto
      },
    ];

    cy.request({
      method: 'POST',
      url: `${baseUrl}/inventory/process-cart`,
      body: {
        items: cart.map((item) => ({
          sku: item.sku,
          officeNum: item.loja,
          quantity: item.quantity,
        })),
      },
      headers: {
        'Content-Type': 'application/json',
      },
    }).then((response) => {
      expect(response.status).to.eq(200);
      expect(response.body).to.have.property('status');
      expect(response.body).to.have.property('message');
    });
  });

  // Verifique se a quantidade do produto foi diminuída após a compra no endpoint /inventory
  it('Teste GET /inventory - Verifica quantidade e loja após compra', () => {
    cy.request(`${baseUrl}/inventory`).then((response) => {
      expect(response.status).to.eq(200);

      // Encontre o produto específico após a compra
      const productAfterPurchase = response.body.find((product) => product.sku === skuToBuy);
      expect(productAfterPurchase).to.exist;

      // Verifique se a quantidade foi reduzida
      expect(productAfterPurchase.quantity).to.be.lessThan(initialQuantity);

      const officeNum = productAfterPurchase.officeNum; // Armazena o número da loja (officeNum)
      cy.log(`Quantidade após a compra: ${productAfterPurchase.quantity} na loja: ${officeNum}`);
    });
  });
});
