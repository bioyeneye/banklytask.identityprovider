using Microsoft.Extensions.DependencyInjection;

namespace IdentityProvider.BusinessDomain.Services.Authentication
{
    public static class CustomIdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddCustomUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
            return builder;
        }
    }
}