# Run RabbitMQ container
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management

# Run Redis container
docker run -d --name redis -p 6379:6379 -p 8001:8001 redis/redis-stack:7.4.0-v8-x86_64
