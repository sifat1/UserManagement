# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY UserManagement/UserManagement.csproj ./UserManagement/
RUN dotnet restore ./UserManagement/UserManagement.csproj

# Copy the rest of the code
COPY UserManagement/ ./UserManagement/
WORKDIR /src/UserManagement
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserManagement.dll"]
