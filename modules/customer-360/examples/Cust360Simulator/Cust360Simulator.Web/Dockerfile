FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["Cust360Simulator.Web/Cust360Simulator.Web.csproj", "Cust360Simulator.Web/"]
RUN dotnet restore "Cust360Simulator.Web/Cust360Simulator.Web.csproj"
COPY . .
WORKDIR "/src/Cust360Simulator.Web"
RUN dotnet build "Cust360Simulator.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Cust360Simulator.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cust360Simulator.Web.dll"]