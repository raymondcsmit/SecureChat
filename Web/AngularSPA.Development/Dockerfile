# base image
FROM node

EXPOSE 4200
EXPOSE 9876

WORKDIR /app
COPY Web/AngularSPA/Client/package.json .
# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install and cache app dependencies
RUN ls
RUN npm install -g @angular/cli@latest
RUN npm install

# start app
ENTRYPOINT npm install && npm start