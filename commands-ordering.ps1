# Build Ordering.API's image
docker image build -t ordering-api -f ./Services/Ordering/Ordering.API/Dockerfile .

# Remove existing Ordering.API's container
docker container rm catalog-api

# Run Ordering.API's container
docker network create eshop-network
docker volume create ordering-sqlserver-volume
docker container run -d --name ordering-sqlserver `
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Pass@word" `
    -p 1433:1433 `
    --network eshop-network `
    -v ordering-sqlserver-volume:/var/opt/mssql `
    mcr.microsoft.com/mssql/server:2017-latest 
docker container run -d --name eshop-rabbitmq `
    -p 5672:5672 -p 15672:15672 `
    --network eshop-network `
    rabbitmq:4-management-alpine
docker container run -it --name ordering-api `
    -e "ConnectionStrings__SQLServer=Server=ordering-sqlserver,1433;`
        Initial Catalog=eShop.Services.OrderingDB;User Id=sa;`
        Password=Pass@word;TrustServerCertificate=True" `
    -e "EventBusConnection=eshop-rabbitmq" `
    --network eshop-network `
    -p 5001:5001 -p 7271:7271 `
    ordering-api

# Build all images via Docker Compose
docker-compose build

# Run all containers via Docker Compose
docker-compose up

# Remove all containers via Docker Compose
docker-compose down