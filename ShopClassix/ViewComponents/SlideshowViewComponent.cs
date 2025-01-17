using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.ViewComponents
{
    public class SlideshowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Bạn có thể lấy dữ liệu tại đây nếu cần
            return View(); // Trả về view mặc định
        }
    }
}
