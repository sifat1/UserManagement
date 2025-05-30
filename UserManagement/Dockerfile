# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the full solution first (if you use a .sln file)
# COPY *.sln .  # Optional

# Copy only .csproj first to leverage caching
COPY UserManagement/UserManagement.csproj ./UserManagement/
RUN dotnet restore ./UserManagement/UserManagement.csproj

# Copy the rest of the project
COPY UserManagement/ ./UserManagement/

# Publish
WORKDIR /src/UserManagement
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "UserManagement.dll"]
