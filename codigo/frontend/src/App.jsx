import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/login';
import Products from './pages/produtos';
import Buy from './pages/compra';
import Cart from './pages/carrinho';
import Final from './pages/final';
import Admin from './pages/administrador';

export default function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/admin" element={<Admin />} />
          <Route path="/products" element={<Products />} />
          <Route path="/compra/:productName/:imageUrl" element={<Buy />} />
          <Route path="/cart" element={<Cart />} />
          <Route path="/final" element={<Final />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </div>
    </Router>
  );
}
