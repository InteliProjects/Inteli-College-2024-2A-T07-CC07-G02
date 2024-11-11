# Etapa de construção: usar a imagem base do .NET SDK para preparar o backend .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend-dotnet
WORKDIR /app/backend-dotnet
COPY ./backend/src-dotnet/*.csproj .
RUN dotnet restore

# Copiar o restante do código .NET e compilar a aplicação
COPY ./backend/src-dotnet/ .
RUN dotnet publish -c Release -o /out

# Etapa de construção: usar a imagem do Node.js para preparar o frontend
FROM node:16-alpine AS build-frontend
WORKDIR /app/frontend
COPY ./frontend/package*.json ./
RUN npm install
COPY ./frontend/ .
RUN npm run build

# Combinação final: usar a imagem base do .NET runtime para combinar os serviços
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar o backend .NET compilado da etapa de construção
COPY --from=build-backend-dotnet /out ./backend-dotnet/

# Copiar o frontend React compilado da etapa de construção
COPY --from=build-frontend /app/frontend/build ./frontend/

# Adicionar o script de inicialização que executa MySQL, backend e frontend
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]

# Expor as portas usadas pelos serviços
EXPOSE 8000