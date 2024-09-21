using AnimePlayerV2.Models.AdminSystem.Languages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using test.Models.AdminSystem;

namespace AnimePlayerV2.Areas.Identity.Pages.Account.Manage
{

    [Authorize(Roles = "Admin,ContentCreator,SubtitleCreator")]
    public class CreatorSettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CreatorSettingsModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Country")]
            public string SelectedCountry { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel
            {
                SelectedCountry = user.Country
            };

            Countries = Enum.GetValues(typeof(CountryEnum))
                .Cast<CountryEnum>()
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.ToString()
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            user.Country = Input.SelectedCountry;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set creator settings.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your creator settings have been updated";
            return RedirectToPage();
        }
    }


}