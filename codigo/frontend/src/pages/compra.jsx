import React from 'react';
import Header from '../components/header';
import ProductDetails from '../components/ProductDetails';
import './compra.css';

function Buy() {
  return (
    <div>
      <Header />
      <div className="buy-page-content">
        <ProductDetails />
      </div>
    </div>
  );
}

export default Buy;
