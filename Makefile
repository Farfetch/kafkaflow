.PHONY: init_broker shutdown_broker

init_broker:	
	@echo command | date
	@echo Initializing Kafka broker
	docker compose -f docker-compose.yml up -d

shutdown_broker:
	@echo command | date
	@echo Shutting down kafka broker
	docker compose -f docker-compose.yml down

restore:
	dotnet restore KafkaFlow.sln

build:
	dotnet build KafkaFlow.sln

unit_tests:
	@echo command | date
	@echo Running unit tests
	dotnet test tests/KafkaFlow.UnitTests/KafkaFlow.UnitTests.csproj --framework netcoreapp2.1 --logger "console;verbosity=detailed"
	dotnet test tests/KafkaFlow.UnitTests/KafkaFlow.UnitTests.csproj --framework netcoreapp3.1 --logger "console;verbosity=detailed"

integration_tests:
	@echo command | date
	make init_broker
	@echo Running integration tests
	dotnet test tests/KafkaFlow.IntegrationTests/KafkaFlow.IntegrationTests.csproj -c Release --logger "console;verbosity=detailed"
	make shutdown_broker
