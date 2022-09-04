FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ExpensesBackend.sln ./
COPY ExpensesBackend/ExpensesBackend.csproj ./ExpensesBackend/ExpensesBackend.csproj
COPY ExpensesBackendLib/ExpensesBackendLib.csproj ./ExpensesBackendLib/ExpensesBackendLib.csproj
COPY ExpensesBackendLibTest/ExpensesBackendLibTest.csproj ./ExpensesBackendLibTest/ExpensesBackendLibTest.csproj
RUN dotnet restore

COPY . ./
RUN dotnet build ExpensesBackend.sln
RUN dotnet test
RUN dotnet publish ExpensesBackend -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN apt update && apt install gettext-base
WORKDIR /app
COPY --from=build-env /app/out .
COPY ExpensesBackend/config.ini.tpl .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80/tcp
ENTRYPOINT ["/bin/bash", "-c", "envsubst < config.ini.tpl > config.ini && ./ExpensesBackend"]
