FROM node:17

WORKDIR /app

ENV PATH /app/node_modules/.bin:$PATH

COPY ClientApp/package.json /app/package.json
RUN npm install

COPY ClientApp/. /app

CMD ng serve --host 0.0.0.0
