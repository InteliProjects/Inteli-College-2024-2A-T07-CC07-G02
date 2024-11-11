import React from 'react';
import { useNavigate } from 'react-router-dom';
import './header.css';
import logoVivoBranco from '../assets/logoVivoBranco.png';
import { FaShoppingCart } from 'react-icons/fa';

function Header() {
  const navigate = useNavigate();

  return (
    <div>
      <header className="header">
        <img src={logoVivoBranco} alt="logoVivoBranco" className='logo-vivo-branco'/>
        <nav>
          <ul className="nav-items">
            <li>
              <button className='login' onClick={() => navigate('/login')}>
                Login
              </button>
            </li>
            <li>
              <button className='pesquisar' onClick={() => navigate('/products')}>
                Pesquisar
              </button>
            </li>
            <li>
              <button className='carrinho' onClick={() => navigate('/cart')}>
                Carrinho <FaShoppingCart />
              </button>
            </li>
          </ul>
        </nav>
      </header>
    </div>
  );
}

export default Header;
