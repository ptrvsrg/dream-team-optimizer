# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY DreamTeamOptimizer.MsEmployee.sln /app/DreamTeamOptimizer.MsEmployee.sln

COPY src/DreamTeamOptimizer.Core /app/src/DreamTeamOptimizer.Core
COPY src/DreamTeamOptimizer.MsEmployee /app/src/DreamTeamOptimizer.MsEmployee

COPY tests/DreamTeamOptimizer.Core.Tests /app/tests/DreamTeamOptimizer.Core.Tests
COPY tests/DreamTeamOptimizer.MsEmployee.Tests /app/tests/DreamTeamOptimizer.MsEmployee.Tests

WORKDIR /app

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependecies
RUN dotnet restore DreamTeamOptimizer.MsEmployee.sln

# Build the application
RUN dotnet build DreamTeamOptimizer.MsEmployee.sln \
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

ENTRYPOINT ["./DreamTeamOptimizer.MsEmployee"]