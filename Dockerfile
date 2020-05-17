FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /gitlab-webhook-receiver

# Copy csproj and restore as distinct layer
COPY *.csproj ./
RUN dotnet restore

# Build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /gitlab-webhook-receiver
COPY --from=build-env /gitlab-webhook-receiver/out .
ENTRYPOINT [ "dotnet", "gitlab-webhook-receiver-aspnetcore.dll" ]
