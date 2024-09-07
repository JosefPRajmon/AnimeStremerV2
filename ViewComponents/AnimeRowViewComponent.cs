using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnimeStreamerV2.ViewComponents
{
    public class AnimeRowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<AnimeModel> AnimeList, String RowName)
        {
            ViewData["RowName"] = RowName;
            return View(AnimeList);
        }
    }
}
