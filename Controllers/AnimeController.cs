using AnimePlayerV2.Models;
using AnimePlayerV2.Services;
using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;

namespace AnimeStreamerV2.Controllers
{
    /// <summary>
    /// Controller for managing anime-related operations.
    /// </summary>
    public class AnimeController : Controller
    {
        private AnimeService _animeService { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="environment">The web host environment.</param>
        /// <param name="userManager">The user manager.</param>
        public AnimeController( AnimeDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager )
        {

            _animeService = new AnimeService( context, environment );
            _userManager = userManager;
        }

        /// <summary>
        /// Displays the index page with a list of animes.
        /// </summary>
        /// <returns>The index view with a list of animes.</returns>
        public async Task<IActionResult> Index()
        {
            ViewBag.Countries = EnumExtensions.GetCountryesEnum();
            ViewData["baseUrl"] = $"{Request.Scheme}://{Request.Host}/"/*+{ Request.Host.Port ?? 80}*/;
            ViewData["fun"] = $"{Request.Query["fun"]}";


            string id = Request.RouteValues["id"]!=null ? Request.RouteValues["id"].ToString() : "null";

            List<AnimeModel> animes = await _animeService.GetAnimesFromUserAsync(id);
            if( User.Identity.IsAuthenticated )
            {
                string userId = (await _userManager.GetUserAsync(User)).Id;
                ViewData["userid"] = userId;
                if( !string.IsNullOrEmpty( userId ) )
                {
                    animes = await _animeService.AddTimeSpendToEpisodesAsync( animes, userId );
                }
            }

            return View( animes );
        }



        /// <summary>
        /// Displays details of a specific anime.
        /// </summary>
        /// <param name="id">The ID of the anime.</param>
        /// <returns>The details view for the specified anime.</returns>
        public async Task<IActionResult> Details( int? id )
        {
            if( id == null )
            {
                return NotFound();
            }

            AnimeModel anime = await _animeService.GetAnimeAsync(id);
            if( anime == null )
            {
                return NotFound();
            }
            //anime.Episodes = _context.Episodes.Where(a => a.AnimeId == id).ToList();
            if( User.Identity.IsAuthenticated )
            {
                string userId = (await _userManager.GetUserAsync(User)).Id;
                if( !string.IsNullOrEmpty( userId ) )
                {
                    anime = await _animeService.AddTimeSpendToEpisodeAsnc( anime, userId );
                }
            }

            ViewData["baseUrl"] = $"{Request.Scheme}://{Request.Host}/";
            return View( anime );
        }

        /// <summary>
        /// Displays the create anime form.
        /// </summary>
        /// <returns>The create view.</returns>
        [Authorize( Roles = "Admin,ContentCreator" )]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if( user.Country is null )
            {
                return Redirect( "/Identity/Account/Manage/CreatorSettings" );
            }
            return View();
        }


        /// <summary>
        /// Processes the creation of a new anime.
        /// </summary>
        /// <param name="anime">The anime model to create.</param>
        /// <param name="AnimeIcon">The icon file for the anime.</param>
        /// <returns>Redirects to the Details action if successful, otherwise returns to the Create view.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,ContentCreator" )]
        public async Task<IActionResult> Create( [Bind( "Name,Description" )] AnimeModel anime, IFormFile AnimeIcon )
        {
            anime.CountryOfOrigin = ( await _userManager.GetUserAsync( User ) ).Country;
            anime.CreaterId = ( await _userManager.GetUserAsync( User ) ).Id;
            if( ModelState.IsValid )
            {
                Boolean task= await _animeService.CreateNewAnimeAsync( anime, AnimeIcon );
                if( task )
                {
                    return RedirectToAction( nameof( Details ), new { id = anime.Id } );
                }

            }


            return View( anime );
        }



        /// <summary>
        /// Displays the edit form for a specific anime.
        /// </summary>
        /// <param name="id">The ID of the anime to edit.</param>
        /// <returns>The edit view for the specified anime.</returns>
        [Authorize( Roles = "Admin,ContentCreator" )]
        public async Task<IActionResult> Edit( int id )
        {
            AnimeModel anime =await _animeService.GetAnimeAsync(id,"category");

            if( anime == null )
            {
                return NotFound();
            }

            var viewModel = new AnimeEditViewModel
            {
                Anime = anime,
                AllCategories = await _animeService.GetAllCategoriesAsync(),
                SelectedCategoryIds = anime.Categories.Select(c => c.Id).ToList()
            };

            return View( viewModel );
        }

        /// <summary>
        /// Processes the editing of an existing anime.
        /// </summary>
        /// <param name="id">The ID of the anime to edit.</param>
        /// <param name="viewModel">The view model containing the updated anime data.</param>
        /// <param name="AnimeIcon">The updated icon file for the anime.</param>
        /// <param name="SelectedCategoryIds">The list of selected category IDs.</param>
        /// <returns>Redirects to the Index action if successful, otherwise returns to the Edit view.</returns>
        [HttpPost]
        [Authorize( Roles = "Admin,ContentCreator" )]
        public async Task<IActionResult> Edit( int id, AnimeEditViewModel viewModel, IFormFile? AnimeIcon, List<int> SelectedCategoryIds )
        {
            if( id != viewModel.Anime.Id )
            {
                return NotFound();
            }

            if( AnimeIcon != null )
            {
                await _animeService.AddIcon( viewModel.Anime, AnimeIcon );
            }

            if( ModelState.IsValid )
            {
                try
                {
                    AnimeModel? animeToUpdate = await _animeService.GetAnimeAsync(id,"category");

                    if( animeToUpdate == null )
                    {
                        return NotFound();
                    }

                    _animeService.UpdateAnimeAsync( viewModel, animeToUpdate, SelectedCategoryIds );
                }
                catch( DbUpdateConcurrencyException )
                {

                    return NotFound();

                }
                return RedirectToAction( nameof( Index ) );
            }

            // If we got this far, something failed, redisplay form
            viewModel.AllCategories = await _animeService.GetAllCategoriesAsync();
            return View( viewModel );
        }

        /// <summary>
        /// Displays the delete confirmation page for a specific anime.
        /// </summary>
        /// <param name="id">The ID of the anime to delete.</param>
        /// <returns>The delete confirmation view for the specified anime.</returns>
        [Authorize( Roles = "Admin,ContentCreator" )]
        public async Task<IActionResult> Delete( int id )
        {
            AnimeModel? anime = await _animeService.GetAnimeAsync(id);
            if( anime == null )
            {
                return NotFound();
            }

            return View( anime );
        }

        /// <summary>
        /// Processes the deletion of an anime.
        /// </summary>
        /// <param name="id">The ID of the anime to delete.</param>
        /// <param name="remove">A boolean indicating whether to remove the anime.</param>
        /// <returns>Redirects to the Index action after deletion.</returns>
        [Authorize( Roles = "Admin,ContentCreator" )]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete( int id, bool remove )
        {
            var anime = await _animeService.GetAnimeAsync(id );
            if( anime != null )
            {
                _animeService.RemoveAnimeAsync( anime );
            }

            return RedirectToAction( nameof( Index ) );
        }



    }
}