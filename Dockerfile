# Usa la imagen SDK de .NET 8 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de la solución y del proyecto, y restaura dependencias
COPY ["ConviAppVS.sln", "./"]
COPY ["ConviAppWeb/ConviAppWeb.csproj", "ConviAppWeb/"]
RUN dotnet restore "ConviAppVS.sln"

# Copia el resto del código y compila la publicación
COPY . .
WORKDIR /app/ConviAppWeb
RUN dotnet publish "ConviAppWeb.csproj" -c Release -o /app/out

# Usa la imagen de runtime de .NET 8 para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expone el puerto 8080 que es el estándar de .NET 8 en Docker
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Inicia la aplicación
ENTRYPOINT ["dotnet", "ConviAppWeb.dll"]
