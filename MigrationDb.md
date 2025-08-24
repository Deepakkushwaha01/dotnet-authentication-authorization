Phir:

dotnet ef migrations add InitialIdentity --context AuthDbContext
dotnet ef database update --context AuthDbContext

dotnet ef migrations add AddRefreshToken --context AppDbContext
dotnet ef database update --context AppDbContext