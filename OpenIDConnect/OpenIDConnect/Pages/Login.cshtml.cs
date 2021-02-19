using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;


namespace OpenIDConnect.Pages
{
    public class LoginModel : PageModel
    {
        public async Task OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync("oidc",
                                    new AuthenticationProperties
                                   {
                                        RedirectUri = redirectUri,
                                        IsPersistent = true,
                                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(15) 
                                   });


        }
      
    }
}
