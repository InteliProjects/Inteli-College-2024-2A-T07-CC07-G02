import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; 
import './produtos.css';
import Header from '../components/header';
import { FaSearch } from 'react-icons/fa';
import axios from 'axios';

function Products() {
  const [productCode, setProductCode] = useState('');
  const [products, setProducts] = useState([]);
  const [currentPage, setCurrentPage] = useState(1); 
  const productsPerPage = 6; 
  const navigate = useNavigate(); 

  // Função para buscar os produtos do backend
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await axios.get('http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/products');

        // Filtrar apenas os produtos que possuem um nome e uma imagem válidos
        const validProducts = response.data.filter(product => product.productName && product.imagePath);

        setProducts(validProducts);
      } catch (error) {
        console.error('Erro ao buscar produtos:', error);
      }
    };

    fetchProducts();
  }, []);

  // Filtrar produtos com base no código do produto inserido
  const filteredProducts = products.filter(product =>
    product.productName.toLowerCase().includes(productCode.toLowerCase())
  );

  // Cálculo da paginação
  const indexOfLastProduct = currentPage * productsPerPage;
  const indexOfFirstProduct = indexOfLastProduct - productsPerPage;
  const currentProducts = filteredProducts.slice(indexOfFirstProduct, indexOfLastProduct);

  // Cálculo de quantas páginas são necessárias
  const totalPages = Math.ceil(filteredProducts.length / productsPerPage);

  // Função para avançar de página
  const handleNextPage = () => {
    setCurrentPage(prevPage => (prevPage < totalPages ? prevPage + 1 : prevPage));
  };

  // Função para retroceder de página
  const handlePreviousPage = () => {
    setCurrentPage(prevPage => (prevPage > 1 ? prevPage - 1 : prevPage));
  };

  // Função para mudar de página dinamicamente
  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  // Função para gerar os botões do meio de forma dinâmica
  const renderPageNumbers = () => {
    let pageNumbers = [];

    if (totalPages <= 3) {
      for (let i = 1; i <= totalPages; i++) {
        pageNumbers.push(i);
      }
    } else {
      if (currentPage === 1) {
        pageNumbers = [1, 2, 3];
      } else if (currentPage === totalPages) {
        pageNumbers = [totalPages - 2, totalPages - 1, totalPages];
      } else {
        pageNumbers = [currentPage - 1, currentPage, currentPage + 1];
      }
    }

    return pageNumbers;
  };

  // Função para lidar com clique no produto e redirecionar para outra página
  const handleBoxClick = (productName, imagePath, sku) => {

    sessionStorage.setItem('selectedSku', sku);

    navigate(`/compra/${encodeURIComponent(productName)}/${encodeURIComponent(imagePath)}`);
  };

  return (
    <div>
      <Header />
      <div className="search-bar">
        <div className="sku-input-container">
          <FaSearch className="search-icon" />
          <input
            type="text"
            placeholder="Informe o nome do produto"
            className="sku-input"
            value={productCode}
            onChange={(e) => setProductCode(e.target.value)}
          />
        </div>
      </div>

      <div className="product-grid">
        {currentProducts.map((product, index) => (
          <div 
            key={index} 
            className="product-box"
            onClick={() => handleBoxClick(product.productName, product.imagePath, product.sku)}
          >
            <img src={product.imagePath} alt={product.productName} className="product-image" />
            <hr className="product-divider" />
            <div className="product-code">{product.productName}</div>
          </div>
        ))}
      </div>

      {/* Paginação com botões dinâmicos */}
      <div className="pagination">
        <button onClick={handlePreviousPage} disabled={currentPage === 1}>
          &lt;&lt;
        </button>
        {renderPageNumbers().map(number => (
          <button
            key={number}
            onClick={() => handlePageChange(number)}
            className={currentPage === number ? 'active' : ''}
          >
            {number}
          </button>
        ))}
        {totalPages > 3 && currentPage < totalPages && (
          <span>...</span> // Adiciona um "..." se houver mais páginas
        )}
        <button onClick={handleNextPage} disabled={currentPage === totalPages}>
          &gt;&gt;
        </button>
      </div>
    </div>
  );
}

export default Products;
