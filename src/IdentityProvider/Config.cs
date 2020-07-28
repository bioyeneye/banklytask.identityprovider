using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityProvider.DataAccess.Entities.Enums;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityProvider
{
    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "consoleclient",
                    ClientName = "Client application using client credentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
                    AllowedScopes = new List<string> {"complaint_service.api.read", "complaint_service.api.write"}
                },
                new Client
                {
                    ClientId = "resourceownerclient",
 
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 120, //86400,
                    IdentityTokenLifetime = 120, //86400,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    SlidingRefreshTokenLifetime = 30,
                    AllowOfflineAccess = true,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AlwaysSendClientClaims = true,
                    Enabled = true,
                    ClientSecrets=  new List<Secret> { new Secret("resourceownerclient".Sha256()) },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "complaint_service.api.read", "complaint_service.api.write"
                    },
                    
                },
                new Client
                {
                    ClientId = "demo_api_swagger",
                    ClientName = "Swagger UI for demo_api",
                    ClientSecrets = {new Secret("secret".Sha256())}, // change me!

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = {"https://localhost:5001/swagger/oauth2-redirect.html"},
                    AllowedCorsOrigins = {"https://localhost:5001"},
                    AllowedScopes = {"complaint_service.api.read", "complaint_service.api.write"}
                }
            };
        }
    }

    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = new List<string> {"roles"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource
                {
                    Name = "complaint_service.api",
                    DisplayName = "Product Complaint Service",
                    Description = "Allow the application to access Complaint Service",
                    Scopes = new List<string> {"complaint_service.api.read", "complaint_service.api.write"},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())}, // change me!
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("complaint_service.api.read", "Read Access to Product Complaint Service"),
                new ApiScope("complaint_service.api.write", "Write Access to Product Complaint Service")
            };
        }
    }

    internal class Users
    {
        public static List<AppUser> Get()
        {
            return new List<AppUser>
            {
                new AppUser
                {
                    SubjectId = "9DE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "admin@identity.com",
                    Password = "Password1@",
                    IsActive = true,
                    FirstName = "Admin",
                    LastName = "User",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "admin@identity.com"),
                        new Claim(JwtClaimTypes.Role, Enum.GetName(typeof(RolesConstants.Enum), RolesConstants.Enum.Administrator))
                    },
                    RoleName  = Enum.GetName(typeof(RolesConstants.Enum), RolesConstants.Enum.Administrator),
                },
                new AppUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "folakemi@identity.com",
                    Password = "Password1@",
                    IsActive = true,
                    FirstName = "Folakemi",
                    LastName = "Simisola",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "folakemi@identity.com"),
                        new Claim(JwtClaimTypes.Role, Enum.GetName(typeof(RolesConstants.Enum), RolesConstants.Enum.User))
                    },
                    RoleName  = Enum.GetName(typeof(RolesConstants.Enum), RolesConstants.Enum.User),
                }
            };
        }
    }
    
    internal class AppUser : TestUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
    }
}