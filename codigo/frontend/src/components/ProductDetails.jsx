import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom'; 
import axios from 'axios';
import './productDetails.css';

function ProductDetails() {
  const { productName, imageUrl } = useParams();
  const [cep, setCep] = useState('');
  const [isCepValid, setIsCepValid] = useState(false);
  const [deliveryTimeMessage, setDeliveryTimeMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false); 
  const [sku, setSku] = useState('');

  // Recuperar o SKU do sessionStorage
  useEffect(() => {
    const savedSku = sessionStorage.getItem('selectedSku');
    if (savedSku) {
      setSku(savedSku);
    }
  }, []);

  const handleCepChange = (e) => {
    const newCep = e.target.value;
    setCep(newCep);
    validateCep(newCep);
  };

  const validateCep = (cep) => {
    const cepRegex = /^[0-9]{5}-?[0-9]{3}$/;
    setIsCepValid(cepRegex.test(cep));
  };

  const handleDeliveryCalculation = async () => {
    setIsLoading(true); 

    try {
      // Fazer a requisição com sku e cep
      const response = await axios.post('http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/DeliveryTime', {
        sku, 
        cep  
      });

      const { loja, tempoEntrega } = response.data;  // Parsear a resposta para obter loja e tempo de entrega
      setDeliveryTimeMessage(`Entrega de aproximadamente ${tempoEntrega} dia(s) útil para o CEP ${cep} da loja ${loja}`);
    } catch (error) {
      console.error('Erro ao calcular o tempo de entrega:', error);
      setDeliveryTimeMessage(`Produto indisponível para entrega no CEP ${cep}`);
    } finally {
      setIsLoading(false); 
    }
  };

  const handleAddToCart = () => {
    const savedCart = JSON.parse(localStorage.getItem('cart')) || [];
    const newProduct = { sku, productName, imageUrl, quantity: 1 };
    
    // Verifica se o produto já está no carrinho
    const productExists = savedCart.find(item => item.sku === sku);
    
    if (!productExists) {
      savedCart.push(newProduct);  // Adiciona novo produto
    } else {
      productExists.quantity += 1;  // Incrementa a quantidade se o produto já existir
    }
  
    localStorage.setItem('cart', JSON.stringify(savedCart));  // Atualiza o localStorage
    alert('Produto adicionado ao carrinho!');
  };   

  return (
    <div className="centralized-box">
      <img src={decodeURIComponent(imageUrl)} alt="Produto" className="product-image" />
      <div className="details-boxes">
        <span className="free-shipping-label">Frete grátis</span>
        <p className="product-code">{productName}</p>
        <div className="price-box">
          <div className="price-info">
            <h2 className="price">R$ 1.999,00</h2>
            <p className="payment-info">À vista ou em 12x sem juros</p>
          </div>
          <button className="add-to-cart-button" onClick={handleAddToCart}>
            Colocar no carrinho
          </button>
        </div>

        <div className="delivery-box">
          <label htmlFor="cep">Prazo de entrega</label>
          <div className="delivery-input-container">
            <input
              type="text"
              id="cep"
              placeholder="CEP"
              value={cep}
              onChange={handleCepChange}
            />
            <button className="confirm-button" onClick={handleDeliveryCalculation} disabled={!isCepValid || isLoading}>
              {isLoading ? 'Calculando...' : 'Confirmar'}
            </button>
          </div>
          {deliveryTimeMessage && (
            <p className={`delivery-message ${deliveryTimeMessage.includes('indisponível') ? 'error' : 'success'}`}>
              {deliveryTimeMessage}
            </p>
          )}
        </div>
      </div>
    </div>
  );
}

export default ProductDetails;
