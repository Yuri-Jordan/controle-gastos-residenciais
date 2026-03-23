FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj", "ControleGastosResidenciais.API/"]
COPY ["ControleGastosResidenciais.Core/ControleGastosResidenciais.Core.csproj", "ControleGastosResidenciais.Core/"]
COPY ["ControleGastosResidenciais.Infraestrutura/ControleGastosResidenciais.Infraestrutura.csproj", "ControleGastosResidenciais.Infraestrutura/"]
RUN dotnet restore "ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj"

# Copy all files and build
COPY . .
WORKDIR "/src/ControleGastosResidenciais.API"
RUN dotnet build "ControleGastosResidenciais.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ControleGastosResidenciais.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControleGastosResidenciais.API.dll"]