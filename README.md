# IdentityProvider

### Setup

#### Setup - Database Initialization 

```
cd src 
dotnet ef database update -c ApplicationDbContext -s IdentityProvider
dotnet ef database update -c PersistedGrantDbContext -s IdentityProvider
dotnet ef database update -c ConfigurationDbContext -s IdentityProvider
```
