dotnet new webapi --no-https --use-minimal-apis -o backend --dry-run
dotnet new webapi --no-https --use-minimal-apis -o backend
cd backend
dotnet build
dotnet watch
https://obscure-invention-wrp6xx9wj6cgqgj-5169.app.github.dev/swagger/index.html
dotnet add package CsvHelper
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.EntityFrameworkCore.SQLite.Design
mkdir Data Models wwwroot

dotnet tool install --global dotnet-ef --version 8.*

dotnet ef migrations add m1 -o Data/Migrations
dotnet ef database update

dotnet ef migrations add m2_library -o Data/Migrations
dotnet ef database update