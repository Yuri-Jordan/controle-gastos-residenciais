# Build em múltiplos estágios para aplicação React
FROM node:18-alpine AS builder

WORKDIR /app

# Instalar ferramentas de build necessárias
RUN apk add --no-cache python3 make g++

# Copiar arquivos de package
COPY package*.json ./

# Usar npm install em vez de ci para maior flexibilidade
RUN npm install --legacy-peer-deps --no-audit --no-fund

# Copiar código fonte
COPY . .

# Construir a aplicação
RUN npm run build

# Estágio de produção
FROM nginx:alpine

# Instalar curl para health checks
RUN apk add --no-cache curl

# Copiar arquivos construídos do estágio builder
COPY --from=builder /app/build /usr/share/nginx/html

# Copiar configuração do nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Adicionar health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/ || exit 1

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]