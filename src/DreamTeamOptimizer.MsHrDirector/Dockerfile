# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY DreamTeamOptimizer.MsHrDirector.sln /app/DreamTeamOptimizer.MsHrDirector.sln

COPY src/DreamTeamOptimizer.Core /app/src/DreamTeamOptimizer.Core
COPY tests/DreamTeamOptimizer.Core.Tests /app/tests/DreamTeamOptimizer.Core.Tests

COPY src/DreamTeamOptimizer.MsHrDirector /app/src/DreamTeamOptimizer.MsHrDirector
COPY tests/DreamTeamOptimizer.MsHrDirector.Tests /app/tests/DreamTeamOptimizer.MsHrDirector.Tests

WORKDIR /app

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependecies
RUN dotnet restore DreamTeamOptimizer.MsHrDirector.sln

# Build the application
RUN dotnet build DreamTeamOptimizer.MsHrDirector.sln \
    -c Release \
    -o out \
    --no-restore

# Run application
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app/out ./
COPY --from=build /app/out/appsettings.json ./appsettings.json

# Switch to a non-privileged user (defined in the base image) that the app will run under.
# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

ENTRYPOINT ["./DreamTeamOptimizer.MsHrDirector"]
