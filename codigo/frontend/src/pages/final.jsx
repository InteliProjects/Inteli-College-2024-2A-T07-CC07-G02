import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; // Importe o hook useNavigate
import Header from '../components/header';
import './final.css';

function Final() {
  const [isLoading, setIsLoading] = useState(true); // Estado para controle de carregamento
  const navigate = useNavigate(); // Defina o hook useNavigate

  useEffect(() => {
    // Simula um carregamento de 2 segundos
    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 2000);

    return () => clearTimeout(timer); // Limpa o timer caso o componente seja desmontado
  }, []);

  return (
    <div>
      <Header />
      <div className="final-main-container">
        {isLoading ? (
          <div className="loading-container">
            <div className="spinner"></div>
            <p className="loading-text">Finalizando a compra...</p>
          </div>
        ) : (
          <div className="thank-you-message-container">
            <h1>Obrigado por comprar com a VIVO!</h1>
            <button className="inicio-button" onClick={() => navigate('/products')}>
              Início
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default Final;
