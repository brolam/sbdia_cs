FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# Define working directory
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80
WORKDIR /sbdia_cs
COPY . .
RUN curl https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs
RUN dotnet tool install --global dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools
ENV PATH $PATH:/Backend/ClientApp/node_modules/.bin:$PATH
RUN npm install --prefix Backend/ClientApp --silent