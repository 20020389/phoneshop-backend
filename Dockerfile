FROM node:16
WORKDIR /app


COPY PhoneShop.csproj ./
COPY appsettings.json ./
RUN npm
RUN npm run db --build --clear

# copy everything else and build app
COPY . .

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "PhoneShop.dll"]