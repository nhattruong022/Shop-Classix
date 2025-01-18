using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;

namespace Shop_Classix.Controllers
{
    public class BlogController : Controller
    {
        private readonly DataContext _context;

        public BlogController(DataContext context)
        {
            _context = context;
        }

        // GET: Blog (Danh sách bài viết)
        // GET: Blog (Danh sách bài viết)
        public async Task<IActionResult> Index(string searchKeyword, string sortOrder, int page = 1)
        {
            var query = _context.BlogPosts.AsQueryable();

            // Tìm kiếm bài viết theo từ khóa
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(bp => bp.Title.Contains(searchKeyword) || bp.Content.Contains(searchKeyword));
            }

            // Sắp xếp bài viết theo tiêu chí
            query = sortOrder switch
            {
                "date_desc" => query.OrderByDescending(bp => bp.PublishedDate),
                "title_asc" => query.OrderBy(bp => bp.Title),
                _ => query.OrderBy(bp => bp.PublishedDate),
            };

            // Phân trang
            int pageSize = 5; // Số bài viết mỗi trang
            int totalPosts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền các giá trị vào ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchKeyword"] = searchKeyword;
            ViewData["SortOrder"] = sortOrder;

            return View(posts); // Trả về danh sách bài viết
        }

        // GET: Blog/PostDetail/5 (Trang chi tiết bài viết)
        public async Task<IActionResult> PostDetail(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Create (Tạo bài viết mới)
        public IActionResult Create()
        {
            return View();
        }
    }
}
