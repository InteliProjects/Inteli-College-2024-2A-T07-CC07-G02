import React from 'react';
import TelaLogin from '../components/TelaLogin';
import logoVivoRoxo from '../assets/logoVivoRoxo.png';
import fundoVivo from '../assets/fundoVivo.png';
import './login.css';

function Login() {
  return (
    <div className="login-page">
      <div className="left-container">
        <TelaLogin />
        <div className="powered-by">
          <p>powered by</p>
          <img src={logoVivoRoxo} alt="Logo Vivo Roxo" />
        </div>
      </div>
      <div className="right-container">
        <img src={fundoVivo} alt="Imagem de Fundo" className="right-image" />
      </div>
    </div>
  );
}

export default Login;
