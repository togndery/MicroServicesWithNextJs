using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Register
{   
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Index(UserManager<ApplicationUser> userManager)
         {
            _userManager = userManager;
         }

         [BindProperty]
         public RegisterViewModel InputModel {get;set;}

         [BindProperty]
         public bool RegisterSuccess {get;set;}
        public IActionResult OnGet(string returnUrl)
        {
            InputModel = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };

            return Page();
        }


        public async Task<IActionResult> OnPost(RegisterViewModel model)
        {
           if(InputModel.Button !="register")
           {
             return Redirect("~/");
           }
           if(ModelState.IsValid)
           {  
              var user = new ApplicationUser
              {
                UserName = InputModel.Username,
                Email = InputModel.Email,
                EmailConfirmed = true,
                             
              } ;

              var result = await _userManager.CreateAsync(user,InputModel.Password);

              if(result.Succeeded)
              {
                await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(JwtClaimTypes.Name, InputModel.FullName)
                });
              }

              RegisterSuccess = true;
           }

           return Page();

        }
         
    }
}
