DOTNET = dotnet
DOCKER = docker
DOCKER_IMAGE_PREFIX = ptrvsrg/dream-team-optimizer

TOOLS_DIR = ./tools

GREEN_COLOR = \033[0;32m
NO_COLOR = \033[0m

.PHONY: clean
clean:
	$(DOTNET) clean

.PHONY: build.console
build.console:
	$(DOTNET) restore DreamTeamOptimizer.ConsoleApp.sln
	$(DOTNET) publish DreamTeamOptimizer.ConsoleApp.sln -c Release -o out/console

.PHONY: build-image.console
build-image.console:
	$(DOCKER) build -f DreamTeamOptimizer.ConsoleApp/Dockerfile -t $(DOCKER_IMAGE_PREFIX)-console .

.PHONY: prepare-hooks
prepare-hooks:
	$(SHELL) $(TOOLS_DIR)/setup-git-hooks.sh

.PHONY: help
help:
	@echo "Available commands:"
	@echo "	make help			${GREEN_COLOR}Display this message${NO_COLOR}"
	@echo "	make clean			${GREEN_COLOR}Clean build and logs directories${NO_COLOR}"
	@echo "	make build.console		${GREEN_COLOR}Build executable file and library for console application${NO_COLOR}"
	@echo "	make build-image.console	${GREEN_COLOR}Build Docker image for console application${NO_COLOR}"
	@echo "	make prepare-hooks		${GREEN_COLOR}Setup git hooks${NO_COLOR}"


.DEFAULT_GOAL := help