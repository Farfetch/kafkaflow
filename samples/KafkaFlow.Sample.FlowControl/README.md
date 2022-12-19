# KafkaFlow.Sample.FlowControl
This is a sample that shows how to control the state of a consumer (Starting, Pausing, Stopping, etc.) programmatically.

## How to run

### Requirements
 - [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
 - [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Start the cluster
Using your terminal of choice, start the cluster.
You can find a docker-compose file at the root of this repository. 
Position the terminal in that folder and run the following command.

```bash
docker-compose up -d
```

### Run the Sample
Using your terminal of choice, start the sample for the sample folder.

```bash
dotnet run
```