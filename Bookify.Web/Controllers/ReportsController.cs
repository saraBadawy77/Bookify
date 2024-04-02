using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Enums;
using Bookify.Web.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class ReportsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHost;
		private readonly IMapper _mapper;
		//private readonly IViewRendererService _viewRendererService;

		private readonly string _logoPath;
		private readonly int _sheetStartRow = 5;

		public ReportsController(ApplicationDbContext context, IMapper mapper,
			IWebHostEnvironment webHost)
		{
			_context = context;
			_mapper = mapper;
			_webHost = webHost;
			

			
		}
		public IActionResult Index()
        {
            return View();
        }
		public IActionResult Books(IList<int> selectedAuthors, IList<int> selectedCategories,
			int? pageNumber)
		{
			var authors = _context.Authors.OrderBy(a => a.Name).ToList();
			var categories = _context.Categories.OrderBy(a => a.Name).ToList();

			IQueryable<Book> books = _context.Books
						.Include(b => b.Author)
						.Include(b => b.Categories)
						.ThenInclude(c => c.Category)
						.Where(b => (!selectedAuthors.Any() || selectedAuthors.Contains(b.AuthorId))
						&& (!selectedCategories.Any() || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId))));

			//if (selectedAuthors.Any())
			//    books = books.Where(b => selectedAuthors.Contains(b.AuthorId));

			//if (selectedCategories.Any())
			//    books = books.Where(b => b.Categories.Any(c => selectedCategories.Contains(c.CategoryId)));

			var viewModel = new BooksReportViewModel
			{
				Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
				Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories)
			};

			if (pageNumber is not null)
				viewModel.Books = PaginatedList<Book>.Create(books, pageNumber ?? 0, (int)ReportsConfigurations.PageSize);

			return View(viewModel);
		}

	}
}
