# Create a new migration
dotnet ef migrations add Name -o .\Infrastructure\CatalogMigrations

# Apply migrations
dotnet ef database update