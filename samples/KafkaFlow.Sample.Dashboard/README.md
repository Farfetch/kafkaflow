# KafkaFlow.Sample.Dashboard

This sample shows how to use KafkaFlow to expose an administration Dashboard.

## How to run

### Requirements

-   [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Start the cluster

Using your terminal of choice, start the cluster.
You can find a docker-compose file at the root of this repository. 
Position the terminal in that folder and run the following command.

```bash
docker-compose up -d
```

### Build Dashboard UI

Using your terminal of choice, navigate to `kafkaflow\src\KafkaFlow.Admin.Dashboard\ClientApp` folder and run the following command:

```bash
ng build
```

### Run the Sample

Using your terminal of choice, start the sample for the sample folder.

```bash
dotnet run
```

The dashboard UI will be available at `/kafkaflow`. 
