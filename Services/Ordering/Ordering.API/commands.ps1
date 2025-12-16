# Create a new migration (OrderingContext)
# NOTE: I've moved the OrderingContextDesignFactory from Ordering.Infrastructure to 
# Ordering.API because the ModelSnapshot was always being created in the latter latter
# one no matter how I execute the `dotnet ef migrations add` command. One option was to
# execute it from the Ordering.Infrastructure directory and specify the --output-dir as
# the `..\Ordering.API\Infrastructure\OrderingMigrations` dir of Ordering.API. The other 
# one was to execute it from the Ordering.API directory and specify the --output-dir as
# `\Infrastructure\OrderingMigrations` and the --project as `..\Ordering.Infrastructure\
# Ordering.Infrastructure.csproj`. Neither of those caused the ModelSnapshot to be created
# alongside the migrations.
dotnet ef migrations add Initial --output-dir .\Infrastructure\OrderingMigrations --context OrderingContext

# Apply migrations (OrderingContext)
dotnet ef database update --context OrderingContext

## Create a new migration (IntegrationEventLogDbContext)
dotnet ef migrations add Initial --output-dir .\Infrastructure\IntegrationEventMigrations --context IntegrationEventLogContext

# Apply migrations (IntegrationEventLogDbContext)
dotnet ef database update --context IntegrationEventLogContext