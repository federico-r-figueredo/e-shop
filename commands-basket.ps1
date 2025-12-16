# Build Basket.API's image
docker image build -t basket-api -f ./Services/Basket/Basket.API/Dockerfile .

# Run Basket.API's container
docker network create eshop-network
docker container run -d --name basket-redis `
    -p 6379:6379 -p 8001:8001 `
    --network eshop-network `
    redis/redis-stack:latest 
docker container run -d --name eshop-rabbitmq `
    -p 5672:5672 -p 15672:15672 `
    --network eshop-network `
    rabbitmq:4-management-alpine
docker container run -it --name basket-api `
    -e "ConnectionStrings__Redis=basket-redis" `
    -e "EventBusConnection=eshop-rabbitmq" `
    --network eshop-network `
    -p 5002:5002 -p 7172:7172 `
    basket-api

# Build all images via Docker Compose
docker-compose build

# Run all containers via Docker Compose
docker-compose up

# Remove all containers via Docker Compose
docker-compose down