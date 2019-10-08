# base image
FROM node

EXPOSE 4200
EXPOSE 9876

WORKDIR /usr/src/app

# add `/usr/src/app/node_modules/.bin` to $PATH
ENV PATH /usr/src/app/node_modules/.bin:$PATH

# install and cache app dependencies
RUN npm install -g @angular/cli@latest
RUN npm install

# start app
ENTRYPOINT npm install && npm start