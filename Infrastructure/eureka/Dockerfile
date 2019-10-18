FROM java:8 AS base
EXPOSE 8761

FROM java:8 AS build
WORKDIR /src
COPY . .
RUN ./mvnw clean package

FROM build AS final
WORKDIR /app
COPY --from=build /src/target/*jar app.jar
ENTRYPOINT ["java","-Dspring.profiles.active=docker","-Djava.security.egd=file:/dev/./urandom","-jar","app.jar", "--spring.config.location=classpath:/application.yml,file:/tmp/config/application.yml"]