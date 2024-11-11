import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Header from '../components/header';
import './carrinho.css';

function Cart() {
  const [cart, setCart] = useState([]);
  const [cep, setCep] = useState('');
  const [isCepValid, setIsCepValid] = useState(false);
  const [isCepApplied, setIsCepApplied] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [deliveryTimeMessage, setDeliveryTimeMessage] = useState('');
  const [hasError, setHasError] = useState(false);
  const [checkoutErrors, setCheckoutErrors] = useState([]);
  const [checkoutErrorMessage, setCheckoutErrorMessage] = useState('');
  const [showErrorMessage, setShowErrorMessage] = useState(false); // Controla a exibição da mensagem de erro

  const navigate = useNavigate();

  useEffect(() => {
    const savedCart = JSON.parse(localStorage.getItem('cart')) || [];
    setCart(savedCart);
  }, []);

  const handleIncreaseQuantity = (index) => {
    const newCart = [...cart];
    newCart[index].quantity = (newCart[index].quantity || 1) + 1;
    setCart(newCart);
    localStorage.setItem('cart', JSON.stringify(newCart));
  };

  const handleDecreaseQuantity = (index) => {
    const newCart = [...cart];
    if (newCart[index].quantity > 1) {
      newCart[index].quantity -= 1;
    } else {
      newCart.splice(index, 1);
    }
    setCart(newCart);
    localStorage.setItem('cart', JSON.stringify(newCart));
  };

  const handleCepChange = (e) => {
    const inputCep = e.target.value;
    setCep(inputCep);
    validateCep(inputCep);
  };

  const validateCep = (inputCep) => {
    const cepPattern = /^[0-9]{5}-?[0-9]{3}$/;
    setIsCepValid(cepPattern.test(inputCep));
  };

  const handleApplyCep = async () => {
    if (!cep || !isCepValid || cart.length === 0) return;

    setIsLoading(true);
    setHasError(false);

    try {
      let maxTempoEntrega = 0;
      let lojaMaxTempo = '';

      const updatedCart = [];

      for (const item of cart) {
        const { sku } = item;

        const response = await axios.post('http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/DeliveryTime', {
          sku,
          cep
        });

        const { loja, tempoEntrega } = response.data;

        if (parseInt(tempoEntrega) > maxTempoEntrega) {
          maxTempoEntrega = parseInt(tempoEntrega);
          lojaMaxTempo = loja;
        }

        updatedCart.push({
          ...item,
          lojaMaxTempo: loja 
        });
      }

      setCart(updatedCart);
      localStorage.setItem('cart', JSON.stringify(updatedCart));

      setDeliveryTimeMessage(`Entrega de aproximadamente ${maxTempoEntrega} dia(s) útil para o CEP ${cep} da loja ${lojaMaxTempo}`);
      setIsCepApplied(true);
    } catch (error) {
      console.error('Erro ao calcular o tempo de entrega:', error);
      setDeliveryTimeMessage(`Algum dos produtos do carrinho não está disponível no CEP ${cep}`);
      setHasError(true);
      setIsCepApplied(true);
    } finally {
      setIsLoading(false);
    }
  };

  const handleChangeCep = () => {
    setIsCepApplied(false);
    setCep('');
    setIsCepValid(false);
    setDeliveryTimeMessage('');
    setHasError(false);
  };

  const handleCheckout = async () => {
    setIsLoading(true);
    setCheckoutErrorMessage('');
    setCheckoutErrors([]);
    setShowErrorMessage(false);

    try {
      const response = await axios.post('http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/inventory/process-cart', {
        items: cart.map(item => ({
          sku: item.sku,
          officeNum: item.lojaMaxTempo,
          quantity: item.quantity
        }))
      });

      if (response.status === 200) {
        setCart([]);
        localStorage.removeItem('cart');
        navigate('/final'); 
      }
    } catch (error) {
      console.error('Erro ao processar o carrinho:', error);

      if (error.response && error.response.data && error.response.data.errors) {
        setCheckoutErrors(error.response.data.errors);
      } else {
        setCheckoutErrorMessage(`Um dos produtos do carrinho não está em estoque para entregar no CEP ${cep}`);
      }
      setShowErrorMessage(true);

      // Remove a mensagem de erro após 10 segundos
      setTimeout(() => {
        setShowErrorMessage(false);
      }, 10000);
    } finally {
      setIsLoading(false);
    }
  };

  const totalItemsInCart = () => {
    return cart.reduce((total, item) => total + (item.quantity || 1), 0);
  };

  return (
    <div>
      <Header />
      <div className="cart-main-container">
        <div className="cart-left-box">
          <div className="cart-container">
            <h2>Carrinho ({totalItemsInCart()})</h2>
            <div className="cart-items">
              {cart.map((item, index) => (
                <div key={index} className="cart-item">
                  <img
                    src={decodeURIComponent(item.imageUrl)}
                    alt="Produto"
                    className="cart-item-image"
                  />
                  <div className="cart-item-details">
                    <p className="cart-item-code">{item.productName}</p>
                    <div className="quantity-controls">
                      <button onClick={() => handleDecreaseQuantity(index)}>-</button>
                      <input type="text" value={item.quantity || 1} readOnly />
                      <button onClick={() => handleIncreaseQuantity(index)}>+</button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
        <div className="cart-right-box">
          <div className="order-summary">
            <h3>Resumo do pedido</h3>
            <hr />
            <div className="delivery-section">
              {isCepApplied ? (
                <>
                  <p className={`delivery-message ${hasError ? 'error' : 'success'}`}>
                    {deliveryTimeMessage}
                  </p>
                  <button onClick={handleChangeCep} className="change-cep-button">
                    Alterar CEP
                  </button>
                </>
              ) : (
                <>
                  {isLoading ? (
                    <div className="spinner-container">
                      <div className="spinner"></div>
                    </div>
                  ) : (
                    <>
                      <label htmlFor="cep">Entrega</label>
                      <input
                        type="text"
                        id="cep"
                        placeholder="CEP"
                        value={cep}
                        onChange={handleCepChange}
                      />
                      <button
                        onClick={handleApplyCep}
                        className="apply-cep-button"
                        disabled={!isCepValid}
                      >
                        Aplicar CEP
                      </button>
                    </>
                  )}
                </>
              )}
            </div>
            <button className="checkout-button" onClick={handleCheckout}>
              Finalizar Compra
            </button>
          </div>
        </div>
      </div>

      {/* Exibir mensagens de erro no canto inferior */}
      {showErrorMessage && (
        <div className="success-box" style={{ backgroundColor: 'red' }}>
          {checkoutErrors.length > 0
            ? checkoutErrors.map((error, index) => <p key={index}>{error}</p>)
            : <p>{checkoutErrorMessage}</p>
          }
        </div>
      )}
    </div>
  );
}

export default Cart;