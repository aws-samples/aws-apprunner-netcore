FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["todo_app.csproj", ""]
RUN dotnet restore "./todo_app.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "todo_app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "todo_app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "todo_app.dll"]