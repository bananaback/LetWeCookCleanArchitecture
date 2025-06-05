# ----------- Build Stage -----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only .csproj files first to leverage Docker layer caching
COPY *.sln ./
COPY LetWeCook.Domain/*.csproj LetWeCook.Domain/
COPY LetWeCook.Application/*.csproj LetWeCook.Application/
COPY LetWeCook.Infrastructure/*.csproj LetWeCook.Infrastructure/
COPY LetWeCook.Web/*.csproj LetWeCook.Web/

# Restore NuGet packages
RUN dotnet restore LetWeCook.Web/LetWeCook.Web.csproj

# Copy the rest of the source
COPY . .

# Publish the application
WORKDIR /src/LetWeCook.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# ----------- Runtime Stage -----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Set environment variables for HTTP and Production environment
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:5015 \
    ASPNETCORE_ENVIRONMENT=Production

# Expose port 5015 for HTTP
EXPOSE 5015

# Start the app
ENTRYPOINT ["dotnet", "LetWeCook.Web.dll"]