.DOTNET = dotnet
.DOCKER = docker
.DOCKER_IMAGE_PREFIX = ptrvsrg/dream-team-optimizer

.TOOLS_DIR = ./tools

.GREEN_COLOR = \033[0;32m
.NO_COLOR = \033[0m

.PHONY: clean
clean:
	$(.DOTNET) clean

.PHONY: build.console
build.console:
	$(.DOTNET) restore DreamTeamOptimizer.ConsoleApp.sln
	$(.DOTNET) publish DreamTeamOptimizer.ConsoleApp.sln -c Release -o out/console

.PHONY: build.ms-employee
build.ms-employee:
	$(.DOTNET) restore DreamTeamOptimizer.MsEmployee.sln
	$(.DOTNET) publish DreamTeamOptimizer.MsEmployee.sln -c Release -o out/ms-employee

.PHONY: build.ms-hr-manager
build.ms-hr-manager:
	$(.DOTNET) restore DreamTeamOptimizer.MsHrManager.sln
	$(.DOTNET) publish DreamTeamOptimizer.MsHrManager.sln -c Release -o out/ms-hr-manager

.PHONY: build.ms-hr-director
build.ms-hr-director:
	$(.DOTNET) restore DreamTeamOptimizer.MsHrDirector.sln
	$(.DOTNET) publish DreamTeamOptimizer.MsHrDirector.sln -c Release -o out/ms-hr-director

.PHONY: build-image.console
build-image.console:
	$(.DOCKER) build -f src/DreamTeamOptimizer.ConsoleApp/Dockerfile -t $(.DOCKER_IMAGE_PREFIX)-console:latest .

.PHONY: build-image.ms-employee
build-image.ms-employee:
	$(.DOCKER) build -f src/DreamTeamOptimizer.MsEmployee/Dockerfile -t $(.DOCKER_IMAGE_PREFIX)-ms-employee:latest .

.PHONY: build-image.ms-hr-manager
build-image.ms-hr-manager:
	$(.DOCKER) build -f src/DreamTeamOptimizer.MsHrManager/Dockerfile -t $(.DOCKER_IMAGE_PREFIX)-ms-hr-manager:latest .

.PHONY: build-image.ms-hr-director
build-image.ms-hr-director:
	$(.DOCKER) build -f src/DreamTeamOptimizer.MsHrDirector/Dockerfile -t $(.DOCKER_IMAGE_PREFIX)-ms-hr-director:latest .

.PHONY: prepare-hooks
prepare-hooks:
	$(SHELL) $(.TOOLS_DIR)/setup-git-hooks.sh

.PHONY: help
help:
	@echo "Available commands:"
	@echo "	make help			${.GREEN_COLOR}Display this message${.NO_COLOR}"
	@echo "	make clean			${.GREEN_COLOR}Clean build and logs directories${.NO_COLOR}"
	@echo "	make build.console		${.GREEN_COLOR}Build executable file and library for console application${.NO_COLOR}"
	@echo "	make build.ms-employee		${.GREEN_COLOR}Build executable file and library for ms-employee application${.NO_COLOR}"
	@echo "	make build.ms-hr-manager	${.GREEN_COLOR}Build executable file and library for ms-hr-manager application${.NO_COLOR}"
	@echo "	make build.ms-hr-director	${.GREEN_COLOR}Build executable file and library for ms-hr-director application${.NO_COLOR}"
	@echo "	make build-image.console	${.GREEN_COLOR}Build Docker image for console application${.NO_COLOR}"
	@echo "	make build-image.ms-employee	${.GREEN_COLOR}Build Docker image for ms-employee application${.NO_COLOR}"
	@echo "	make build-image.ms-hr-manager	${.GREEN_COLOR}Build Docker image for ms-hr-manager application${.NO_COLOR}"
	@echo "	make build-image.ms-hr-director	${.GREEN_COLOR}Build Docker image for ms-hr-director application${.NO_COLOR}"
	@echo "	make prepare-hooks		${.GREEN_COLOR}Setup git hooks${.NO_COLOR}"


.DEFAULT_GOAL := help