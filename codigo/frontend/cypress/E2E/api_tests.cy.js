// Grupo de testes que verifica o funcionamento dos endpoints da API
describe('Testes Integração dos Endpoints da API com o Front-end', () => {
    const baseUrl = 'http://lb-519e665-154492853.us-east-1.elb.amazonaws.com:80'; // URL base da API
  
    // Teste para o endpoint GET /products
    it('Teste GET /products', () => {
        // Faz uma requisição GET para o endpoint de produtos
        cy.request(`${baseUrl}/products`)
          .then((response) => {
            // Verifica se a resposta tem o status 200 (sucesso)
            expect(response.status).to.eq(200);
      
            // Verifica se a resposta é um objeto JSON válido
            expect(response.headers['content-type']).to.include('application/json');
            
            // Verifica se o corpo da resposta é um array ou objeto JSON
            expect(response.body).to.be.an('array'); // ou 'array' se for uma lista de produtos
          });
      });
      
  
    // Teste para o endpoint POST /DeliveryTime
    it('Teste POST /DeliveryTime', () => {
      // Faz uma requisição POST enviando o SKU do produto e o CEP
      cy.request({
        method: 'POST',
        url: `${baseUrl}/DeliveryTime`,
        body: { sku: 'DFHD6853I2244', cep: '34567-890' }, // Dados enviados na requisição
      }).then((response) => {
        // Verifica se a resposta tem o status 200 (sucesso)
        expect(response.status).to.eq(200);
        // Verifica se a resposta contém as propriedades esperadas ('tempoEntrega' e 'loja')
        expect(response.body).to.have.property('tempoEntrega');
        expect(response.body).to.have.property('loja');
      });
    });
  
    // Teste para o endpoint POST /inventory/process-cart
    it('Teste POST /inventory/process-cart', () => {
      // Define os itens do carrinho que serão enviados no corpo da requisição
      const cart = [
        {
          sku: 'DFHD6853I2244',
          loja: 'LOJ225',
          quantity: 1,
        },
      ];
  
      // Faz uma requisição POST com os itens do carrinho
      cy.request({
        method: 'POST',
        url: `${baseUrl}/inventory/process-cart`,
        body: {
          // Mapeia os itens do carrinho para o formato esperado pela API
          items: cart.map((item) => ({
            sku: item.sku,
            officeNum: item.loja,
            quantity: item.quantity,
          })),
        },
        headers: {
          'Content-Type': 'application/json', // Define o tipo de conteúdo como JSON
        },
      }).then((response) => {
        // Verifica se a resposta tem o status 200 (sucesso)
        expect(response.status).to.eq(200);
        // Verifica se a resposta contém as propriedades esperadas ('status' e 'message')
        expect(response.body).to.have.property('status');
        expect(response.body).to.have.property('message');
      });
    });
  
    // Teste para o upload de um arquivo Excel de inventário
    it('Teste POST /UploadExcel/upload-inventory', () => {
      const filePath = 'inventory.xlsx'; // Nome do arquivo na pasta cypress/fixtures
  
      // Carrega o arquivo Excel como conteúdo binário
      cy.fixture(filePath, 'binary').then((fileContent) => {
        // Converte o conteúdo binário em um Blob
        const blob = Cypress.Blob.binaryStringToBlob(
          fileContent,
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        );
  
        // Cria um FormData e adiciona o arquivo
        const formData = new FormData();
        formData.append('file', blob, filePath); // Adiciona o arquivo como parte do form-data
  
        // Faz a requisição POST enviando o arquivo Excel
        cy.request({
          method: 'POST',
          url: `${baseUrl}/UploadExcel/upload-inventory`,
          headers: {
            'Content-Type': 'multipart/form-data', // Define o tipo de conteúdo como multipart/form-data
          },
          body: formData, // Envia o arquivo como parte do corpo da requisição
          encoding: 'binary', // Configuração adicional para manipular o upload de binário corretamente
        }).then((response) => {
          // Verifica se a resposta tem o status 200 (sucesso)
          expect(response.status).to.eq(200);

          // Converte o ArrayBuffer para string antes de verificar o conteúdo
          const decoder = new TextDecoder('utf-8');
          const responseBody = decoder.decode(response.body);

          // Verifica se a resposta contém a mensagem 'added to Inventory'
          expect(responseBody).to.contain('added to Inventory');
        });
      });
    });
  
    // Teste para o upload de um arquivo Excel de lojas
    it('Teste POST /UploadExcel/upload-stores', () => {
      const filePath = 'stores.xlsx'; // Nome do arquivo na pasta cypress/fixtures
  
      // Carrega o arquivo Excel como conteúdo binário
      cy.fixture(filePath, 'binary').then((fileContent) => {
        // Converte o conteúdo binário em um Blob
        const blob = Cypress.Blob.binaryStringToBlob(
          fileContent,
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        );
  
        // Cria um FormData e adiciona o arquivo
        const formData = new FormData();
        formData.append('file', blob, filePath); // Adiciona o arquivo como parte do form-data
  
        // Faz a requisição POST enviando o arquivo Excel
        cy.request({
          method: 'POST',
          url: `${baseUrl}/UploadExcel/upload-stores`,
          headers: {
            'Content-Type': 'multipart/form-data', // Define o tipo de conteúdo como multipart/form-data
          },
          body: formData, // Envia o arquivo como parte do corpo da requisição
          encoding: 'binary', // Configuração adicional para manipular o upload de binário corretamente
        }).then((response) => {
          // Verifica se a resposta tem o status 200 (sucesso)
          expect(response.status).to.eq(200);
          
          // Converte o ArrayBuffer para string antes de verificar o conteúdo
          const decoder = new TextDecoder('utf-8');
          const responseBody = decoder.decode(response.body);

          // Verifica se a resposta contém a mensagem 'added to Inventory'
          expect(responseBody).to.contain('added to Store');
        });
      });
    });

});
  