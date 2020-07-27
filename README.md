# IdentityProvider

### Setup

#### Setup - Build project and restore packages 

```
cd src 
dotnet build 
```

#### Setup - Database Initialization 
##### Change the ApplicationDbContext connection string in appsettings.json to match your msqlserver connection

```
cd src 
dotnet ef database update -c ApplicationDbContext -s IdentityProvider
dotnet ef database update -c PersistedGrantDbContext -s IdentityProvider
dotnet ef database update -c ConfigurationDbContext -s IdentityProvider
```

#### Setup - Database Seed 
##### Change the Config.cs detail in the IdentityProvider project to seed the database
