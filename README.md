# Inteli - Instituto de Tecnologia e Liderança

<table>
<tr>
<td>
<a href= "https://www.vivo.com.br/"><img src="doc/assets/vivo_logo.png" alt="Vivo" border="0" width="100%"></a>
</td>
<td><a href= "https://www.inteli.edu.br/"><img src="doc/assets/inteli_logo.png" alt="Inteli - Instituto de Tecnologia e Liderança" border="0" width="100%"></a>
</td>
</tr>
</table>

# Projeto: Solução de inventário distribuído para a gestão de estoque em tempo real

# Grupo 3: Nimbus

## 👨‍🎓 Integrantes: 
- <a href="https://www.linkedin.com/in/beatriz-monsanto/">Beatriz Monsanto</a>
- <a href="https://www.linkedin.com/in/eduardo-simonis-ferrari/">Eduardo Simonis Seabra Martins Ferrari
- <a href="https://www.linkedin.com/in/filipe-calabro-3b3517243/">Filipe Calabro</a>
- <a href="https://www.linkedin.com/in/gabriel-demacedosantos/">Gabriel de Macedo</a>
- <a href="https://www.linkedin.com/in/pedro-bannwart-0565aa264/">Pedro Bannwart</a>
- <a href="https://www.linkedin.com/in/tommygoto/">Tommy Goto</a>

## 📜 Descrição
&emsp;&emsp; Esse projeto aborda o problema de falta de sincronização dos estoques da Vivo. A carga massiva de dados, além de muitas requisições de compra simultâneas no E-commerce da empresa causam travamentos e lentidões na plataforma, que não tem capacidade computacional para se adaptar as necessidades de requisições. O objetivo principal é migrar esse sistema para a cloud, para assim criar uma aplicação que consiga lidar com diversas atualizações vindas de multiplas fontes e garantir o sistema receba corretamente as mudanças, porém mantendo os custos operacionais os mais baixos o possível.

&emsp;&emsp; Para isso, foi desenvolvida uma de infraestrutura sistema distribuido no serviço cloud da Amazon AWS, com capacidade de elasticidade dos recursos computacionais, com sua potência e custos se adaptando a demanda da plataforma, além de utilizar a tecnologias de bancos de dados rápidos e alta disponibilidade.

&emsp;&emsp; Foram também criados diversos testes, como teste de carga e de microsserviços, que conseguem assegurar que o sistema tem a capacidade de suportar cargas e acessos massívos, garantindo que seu funcionamento é mantido em qualquer situação.


## 📁 Configuração para desenvolvimento
&emsp;&emsp; Essa seção descreve as configuração e tecnologias utilizadas no sistema, e os requisitos necessários para executar o serviço. A aplicação foi desenvolvida em DOTNET, com o uso de React para seu frontend, podendo ser executada utilizando Docker. Ela foi criada com diversos serviços da AWS, que garantem que o sistema funcione corretamente com todos os requisitos definidos. 

&emsp;&emsp; Para executar o sistema, é necessário ter instalado o [Docker](https://www.docker.com/) e instalar todas as dependências do projeto. 

&emsp;&emsp; Para instalar as dependências do projeto, execute o comando dentro do diretório do frontend:

```bash
npm install
```

&emsp;&emsp; Após a instalação das dependências, para rodar o backend, você pode fazê-lo através do Docker indo para o diretório codigo e executando o comando:

```bash
docker-compose up
```

&emsp;&emsp; Para rodar o frontend, você pode fazer o mesmo:

```bash
npm start
```

&emsp;&emsp; Após a execução dos comandos, a aplicação estará disponível em `http://localhost:3000/`.



## 🗃 Tags:
- [SPRINT 1](https://github.com/Inteli-College/2024-2A-T07-CC07-G02/releases/tag/Sprint1)
    - MVP com deploy da aplicação com arquitetura básica;
    - Entendimento de Negócio;
    - Requisitos Funcionais e Não Funcionais;
    - Entendimento do Usuário
- [SPRINT 2](https://github.com/Inteli-College/2024-2A-T07-CC07-G02/releases/tag/SPRINT2)
    - Arquitetura corporativa;
    - Artigo (versão 1);
    - Back-end;
    - Arquitetura corporativa;
    - Infraestrutura;
    - Front-end
- [SPRINT 3](https://github.com/Inteli-College/2024-2A-T07-CC07-G02/releases/tag/SPRINT3)
    - Modelagem;
    - Relatório técnico;
    - Artigo (versão 2);
    - Integração front-end e back-end
- [SPRINT 4](https://github.com/Inteli-College/2024-2A-T07-CC07-G02/releases/tag/SPRINT4)
    - Definição da aplicação;
    - Testes do sistema;
    - Artivo (versão 3)
- [SPRINT 5](https://github.com/Inteli-College/2024-2A-T07-CC07-G02/releases/tag/Sprint1)
    - Aprimoramento dos testes;
    - Organização de repositório do Github;
    - Artigo completo



## Licença

<img src="doc\assets\CC.png" border="0" width="50%">

<img src="doc\assets\Icon.png" border="0" width="50%">

<br>

&emsp;&emsp; <a href="https://creativecommons.org/licenses/by/4.0/?ref=chooser-v1">Attribution 4.0 International</a>
