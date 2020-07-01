.PHONY: init_broker shutdown_broker

init_broker:	
	@echo command | date
	@echo Initializing Kafka broker
	docker-compose -f docker-compose.yml up -d

shutdown_broker:
	@echo command | date
	@echo Shutting down kafka broker
	docker-compose -f docker-compose.yml down

restore:
	dotnet restore src/KafkaFlow.sln

build:
	dotnet build src/KafkaFlow.sln

test:
	@echo command | date
	@echo Running unit tests
	dotnet test src/KafkaFlow.UnitTests/KafkaFlow.UnitTests.csproj
	make init_broker
	@echo Running integration tests
	dotnet test src/KafkaFlow.IntegrationTests/KafkaFlow.IntegrationTests.csproj -c Release
	make shutdown_broker
