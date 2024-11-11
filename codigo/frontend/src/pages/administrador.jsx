import React, { useState, useRef } from 'react';
import './administrador.css';
import { useNavigate } from 'react-router-dom';
import logoVivoBranco from '../assets/logoVivoBranco.png';
import axios from 'axios';

function Admin() {
  const navigate = useNavigate();
  const [selectedProductFile, setSelectedProductFile] = useState(null);
  const [selectedStoreFile, setSelectedStoreFile] = useState(null);
  const [message, setMessage] = useState(''); // Estado para a mensagem de sucesso ou erro
  const [messageColor, setMessageColor] = useState('#4CAF50'); // Estado para a cor da mensagem (verde por padrão)

  // Usando useRef para acessar diretamente os inputs de arquivo
  const productFileInputRef = useRef(null);
  const storeFileInputRef = useRef(null);

  const handleFileUpload = (event, type) => {
    const file = event.target.files[0];
    if (type === 'produtos') {
      setSelectedProductFile(file);
    } else {
      setSelectedStoreFile(file);
    }
  };

  const removeFile = (type) => {
    if (type === 'produtos') {
      setSelectedProductFile(null);
    } else {
      setSelectedStoreFile(null);
    }
  };

  const handleSubmit = (type) => {
    if (type === 'produtos' && selectedProductFile) {
      uploadFile(selectedProductFile, 'produtos');
    } else if (type === 'lojas' && selectedStoreFile) {
      uploadFile(selectedStoreFile, 'lojas');
    }
  };

  const uploadFile = async (file, type) => {
    const formData = new FormData();
    formData.append('file', file);

    let url = '';
    if (type === 'produtos') {
      url = 'http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/UploadExcel/upload-inventory';
    } else {
      url = 'http://lb-9aee083-586275106.us-east-1.elb.amazonaws.com:80/UploadExcel/upload-stores';
    }

    try {
      const response = await axios.post(url, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      if (response.status === 200) {
        setMessage(`Arquivo "${file.name}" enviado com sucesso!`);
        setMessageColor('#4CAF50'); // Verde para sucesso

        // Remove o arquivo da box e reseta o input
        removeFile(type);
        if (type === 'produtos') {
          productFileInputRef.current.value = ''; // Resetando o input de produtos
        } else {
          storeFileInputRef.current.value = ''; // Resetando o input de lojas
        }
      } else {
        throw new Error('Erro ao enviar o arquivo');
      }

      setTimeout(() => {
        setMessage('');
      }, 3000);
    } catch (error) {
      setMessage(`Erro ao enviar o arquivo "${file.name}". Tente novamente.`);
      setMessageColor('red'); // Vermelho para erro
      setTimeout(() => {
        setMessage('');
      }, 3000);
    }
  };

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
          </ul>
        </nav>
      </header>

      <div className="admin-container">
        <h2>Tela de Administrador</h2>
        <div className="upload-section">
          <div className="upload-box">
            <input 
              type="file" 
              accept=".xlsx" 
              onChange={(event) => handleFileUpload(event, 'produtos')}
              className="file-input"
              ref={productFileInputRef} // Ref para o input de produtos
            />
            {selectedProductFile ? (
              <div className="file-preview">
                <span>{selectedProductFile.name}</span>
                <button className="remove-button" onClick={() => removeFile('produtos')}>X</button>
              </div>
            ) : (
              <p>Insira Excel de Inventário</p>
            )}
          </div>
          <div className="upload-box">
            <input 
              type="file" 
              accept=".xlsx" 
              onChange={(event) => handleFileUpload(event, 'lojas')}
              className="file-input"
              ref={storeFileInputRef} // Ref para o input de lojas
            />
            {selectedStoreFile ? (
              <div className="file-preview">
                <span>{selectedStoreFile.name}</span>
                <button className="remove-button" onClick={() => removeFile('lojas')}>X</button>
              </div>
            ) : (
              <p>Insira Excel de Lojas</p>
            )}
          </div>
        </div>

        <div className="button-section">
          <button className="submit-button" onClick={() => handleSubmit('produtos')}>Enviar Inventário</button>
          <button className="submit-button" onClick={() => handleSubmit('lojas')}>Enviar Lojas</button>
        </div>

        {message && (
          <div className="success-box" style={{ backgroundColor: messageColor }}>
            {message}
          </div>
        )}
      </div>
    </div>
  );
}

export default Admin;
