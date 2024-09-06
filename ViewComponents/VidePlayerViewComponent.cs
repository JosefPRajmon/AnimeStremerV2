using Microsoft.AspNetCore.Mvc;

namespace AnimeStreamerV2.ViewComponents
{
    public class VideoPlayerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
