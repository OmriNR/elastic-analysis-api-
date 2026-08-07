# Stage 1: build
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Stage 2: serve with nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

# Rendered via the base image's docker-entrypoint.d envsubst step on container start.
# Defaults to the docker-compose service name; Render overrides this with StoreAPI's
# internal hostname (see render.yaml).
ENV STOREAPI_HOST=storeapi:80
# Populates $NGINX_LOCAL_RESOLVERS from /etc/resolv.conf for the resolver directive above.
ENV NGINX_ENTRYPOINT_LOCAL_RESOLVERS=1

EXPOSE 80
