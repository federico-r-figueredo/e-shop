# Build Catalog.API's image
docker image build -t catalog-api -f ./Services/Catalog/Catalog.API/Dockerfile .

# Remove existing Catalog.API's container
docker container -f rm catalog-api

# Run Catalog.API's container
docker network create catalog-network
docker volume create catalog-sqlserver-volume
docker container run -d --name catalog-sqlserver `
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Pass@word" `
    -p 1434:1433 `
    --network eshop-network `
    -v catalog-sqlserver-volume:/var/opt/mssql `
    mcr.microsoft.com/mssql/server:2017-latest 
docker container run -d --name eshop-rabbitmq `
    -p 5672:5672 -p 15672:15672 `
    --network eshop-network `
    rabbitmq:4-management-alpine
docker container run -it --name catalog-api `
    -e "ConnectionStrings__SQLServer=Server=catalog-sqlserver,1434;`
        Initial Catalog=eShop.Services.CatalogDB;User Id=sa;`
        Password=Pass@word;TrustServerCertificate=True" `
    -e "EventBusConnection=eshop-rabbitmq" `
    --network eshop-network `
    -p 5003:5003 -p 7273:7273 `
    catalog-api

# Build all images via Docker Compose
docker-compose build

# Run all containers via Docker Compose
docker-compose up

# Remove all containers via Docker Compose
docker-compose down