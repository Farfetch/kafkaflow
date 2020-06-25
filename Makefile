.PHONY: init_broker shutdown_broker

init_broker:	
	@echo command | date
	@echo Initializing Kafka broker
	docker-compose -f docker-compose.yml up -d

shutdown_broker:
	@echo command | date
	@echo Shutting down kafka broker
	docker-compose -f docker-compose.yml down