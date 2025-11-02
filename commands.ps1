# Build Catalog.API's image
docker image build -t catalog-api -f ./Services/Catalog/Catalog.API/Dockerfile .

# Run Catalog.API's container
docker network create catalog-network
docker volume create catalog-sqlserver-volume
docker container run -d --name catalog-sqlserver `
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Pass@word" `
    -p 1433:1433 `
    --network catalog-network `
    -v catalog-sqlserver-volume:/var/opt/mssql `
    mcr.microsoft.com/mssql/server:2017-latest 
docker container run -d --name catalog-rabbitmq `
    -p 5672:5672 -p 15672:15672 `
    --network catalog-network `
    rabbitmq:4-management-alpine
docker container run -it --name catalog-api `
    -e "ConnectionStrings__SQLServer=Server=catalog-sqlserver;`
        Initial Catalog=eShop.Services.CatalogDB;User Id=sa;`
        Password=Pass@word;TrustServerCertificate=True" `
    -e "EventBusConnection=catalog-rabbitmq" `
    --network catalog-network `
    -p 5003:5003 -p 7271:7271 `
    catalog-api