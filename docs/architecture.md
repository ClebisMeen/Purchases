# Setup 
Install `dotnet tool install --global dotnet-ef`

Validate `dotnet ef`

Execute `dotnet ef database update --project src/Wex.Purchases.Infrastructure --startup-project src/Wex.Purchases.Api`

Fluxo
$env:ConnectionStrings__PurchaseDb="server=localhost;port=3306;database=wex_purchases;user=wex;password=wex123;AllowPublicKeyRetrieval=True;SslMode=None"

dotnet build .\src\Wex.Purchases.Api\Wex.Purchases.Api.csproj

dotnet ef migrations list --project .\src\Wex.Purchases.Infrastructure\Wex.Purchases.Infrastructure.csproj --startup-project .\src\Wex.Purchases.Api\Wex.Purchases.Api.csproj --no-build

dotnet ef database update --project .\src\Wex.Purchases.Infrastructure\Wex.Purchases.Infrastructure.csproj --startup-project .\src\Wex.Purchases.Api\Wex.Purchases.Api.csproj --no-build

docker exec wex-mysql mysql -uroot -proot123 -e "USE wex_purchases; SHOW TABLES; SELECT MigrationId FROM __EFMigrationsHistory;"


dotnet build
dotnet test
docker compose up -d wex-mysql
dotnet ef database update --project .\src\Wex.Purchases.Infrastructure\Wex.Purchases.Infrastructure.csproj --startup-project .\src\Wex.Purchases.Api\Wex.Purchases.Api.csproj


dotnet test tests\Wex.Purchases.UnitTests\Wex.Purchases.UnitTests.csproj
dotnet test tests\Wex.Purchases.IntegrationTests\Wex.Purchases.IntegrationTests.csproj

https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/treasury-reporting-rates-of-exchange#api-quick-guide




docker compose up -d wex-mysql
docker compose up -d redis