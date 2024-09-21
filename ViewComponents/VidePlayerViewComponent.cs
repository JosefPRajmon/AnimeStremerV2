using Microsoft.AspNetCore.Mvc;

namespace AnimeStreamerV2.ViewComponents
{
    /// <summary>
    /// Represents a view component for rendering a video player.
    /// </summary>
    public class VideoPlayerViewComponent : ViewComponent
    {
        /// <summary>
        /// Invokes the view component to render the video player.
        /// </summary>
        /// <returns>The result of the view component.</returns>
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
