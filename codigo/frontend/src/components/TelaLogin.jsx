import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './telaLogin.css'; 
import logoNimbus from '../assets/logoNimbus.png';
import { FaEye, FaEyeSlash } from 'react-icons/fa';

function TelaLogin() {
  const navigate = useNavigate();
  const [senhaVisivel, setSenhaVisivel] = useState(false);

  const handleAdminLogin = (e) => {
    e.preventDefault();
    navigate('/admin');
  };

  const handleClientLogin = () => {
    navigate('/products');
  };

  const toggleSenhaVisivel = () => {
    setSenhaVisivel(!senhaVisivel);
  };

  return (
    <div className="login-container">
      <div className="login-header">
        <h1>Login</h1>
        <div className="cloud-icon">
          <img src={logoNimbus} alt="Logo Nimbus" width="40" height="40" />
        </div>
      </div>
      <form className="login-form" onSubmit={handleAdminLogin}>
        <input type="text" placeholder="Cpf ou email" className="input-field" />
        <div className="password-container">
          <input 
            type={senhaVisivel ? "text" : "password"}
            placeholder="Senha" 
            className="input-field" 
            inputMode="numeric" 
            pattern="[0-9]*" 
            maxLength="6" 
            required 
          />
          <span className="eye-icon" onClick={toggleSenhaVisivel}>
            {senhaVisivel ? <FaEyeSlash /> : <FaEye />}
          </span>
        </div>
        <span className="password-hint">Sua senha deve ter 6 números</span>
        <button type="submit" className="login-button">Entrar como Administrador</button>
        <div className="separator">Ou</div>
        <button type="button" className="login-client-button" onClick={handleClientLogin}>Entrar como cliente</button>
      </form>
    </div>
  );
}

export default TelaLogin;
