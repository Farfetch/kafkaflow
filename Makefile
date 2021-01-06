.PHONY: init_broker shutdown_broker

init_broker:	
	@echo command | date
	@echo Initializing containers
	$ docker-compose up -d zookeeper broker schema-registry

shutdown_broker:
	@echo command | date
	@echo Shutting down containers
	docker-compose down

restore:
	dotnet restore src/KafkaFlow.sln

build:
	dotnet build src/KafkaFlow.sln

unit_tests:
	@echo command | date
	@echo Running unit tests
	dotnet test src/KafkaFlow.UnitTests/KafkaFlow.UnitTests.csproj

integration_tests:
	@echo command | date
	make init_broker
	@echo Running integration tests
	dotnet test src/KafkaFlow.IntegrationTests/KafkaFlow.IntegrationTests.csproj -c Release
	make shutdown_broker
