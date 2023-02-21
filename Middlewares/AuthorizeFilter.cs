using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace PhoneShop.Middleware;

public class UserAuthorizeAttribute : TypeFilterAttribute
{
    public UserAuthorizeAttribute()
        : base(typeof(UserAuthorizeFilter))
    {
        Arguments = new object[] { };
    }
}

public class UserAuthorizeFilter : IAuthorizationFilter
{
    public UserAuthorizeFilter() { }

    public void OnAuthorization(AuthorizationFilterContext context) { }
}
