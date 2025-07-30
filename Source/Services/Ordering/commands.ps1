# Create migration (Services.Ordering.Infrastructure -> Services.Ordering.API)
dotnet ef migrations add Initial --project .\Infrastructure\ --output-dir ./Migrations/OrderingMigrations --verbose

# Update Database (up to latest migration)
dotnet ef database update --project .\API\ --verbose --context IntegrationEventLogContext
dotnet ef database update --project .\API\ --context IntegrationEventLogContext --connection "Server=localhost;Initial Catalog=Ordering;User Id=admin;Password=1234;TrustServerCertificate=True;Encrypt=false;Trusted_Connection=True;"

# Create migrations for IntegrationEventLogContext
dotnet ef migrations add Initial --project ./API --output-dir ./Migrations/IntegrationEventMigrations --verbose --startup-project ./API --context IntegrationEventLogContext

# Apply IntegrationEventLogContext migrations
dotnet ef database update --project .\API\ --verbose --context IntegrationEventLogContext --connection "Server=localhost;Initial Catalog=Ordering;User Id=admin;Password=1234;TrustServerCertificate=True;Encrypt=false;Trusted_Connection=True;"