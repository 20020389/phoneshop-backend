# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY PhoneShop.csproj ./
COPY appsettings.json ./
RUN dotnet restore
RUN npm
RUN npm run db --build --clear

# copy everything else and build app
COPY . .
WORKDIR /app
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "PhoneShop.dll"]