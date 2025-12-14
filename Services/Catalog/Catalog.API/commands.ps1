# Create a new migration (CatalogContext)
dotnet ef migrations add Initial --output-dir .\Infrastructure\CatalogMigrations --context CatalogContext

# Apply migrations (CatalogContext)
dotnet ef database update --context CatalogContext

## Create a new migration (IntegrationEventLogDbContext)
dotnet ef migrations add Initial --output-dir .\Infrastructure\IntegrationEventMigrations --context IntegrationEventLogContext

# Apply migrations (IntegrationEventLogDbContext)
dotnet ef database update --context IntegrationEventLogContext

# Install HTTPGenerator tool
dotnet tool install --global httpgenerator

# Generate .http file
httpgenerator http://localhost:5003/swagger/v1/swagger.json --output-type OneFile