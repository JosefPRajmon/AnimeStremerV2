using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnimeStreamerV2.ViewComponents
{
    public class AnimeRowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<AnimeModel> AnimeList, String RowName, string BaseUrl)
        {
            ViewData["RowName"] = RowName;
            ViewData["BaseUrl"] = BaseUrl;
            /* List<AnimeModel> a = AnimeList.ToList();
             for (int i = 0; i<20; i++)
             {
                 foreach (var item in AnimeList)
                 {
                     a.Add(item);
                 }
             }
             AnimeList = a;*/
            return View(AnimeList);
        }
    }
}
