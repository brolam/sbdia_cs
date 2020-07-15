FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# Define working directory
WORKDIR /sbdia_cs
EXPOSE 6010
RUN curl http://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs
RUN dotnet tool install --global dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools