using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using ProductCatalog.Models;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers;

public sealed class ProductsController(IProductCatalog catalog, IWebHostEnvironment environment) : Controller
{
    private static readonly string[] Categories = ["Смартфондар", "Ноутбуктер", "Аудио", "Аксессуарлар"];

    [HttpGet]
    public IActionResult Index(string? q, string? category, string? sort = "featured")
    {
        var allProducts = catalog.GetAll();
        IEnumerable<Product> result = allProducts;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            result = result.Where(product => product.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || product.Category.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category) && Categories.Contains(category))
        {
            result = result.Where(product => product.Category == category);
        }

        result = sort switch
        {
            "price-asc" => result.OrderBy(product => product.Price),
            "price-desc" => result.OrderByDescending(product => product.Price),
            "name" => result.OrderBy(product => product.Name),
            _ => result.OrderBy(product => product.Id)
        };

        ViewData["Title"] = "Өнімдер жинағы";
        ViewBag.CatalogIntro = "Күнделікті техникаға арналған шағын таңдау.";
        ViewBag.CollectionSize = allProducts.Count;
        ViewBag.AvailableCount = allProducts.Count(product => product.IsAvailable);
        ViewBag.CategoryList = Categories;
        ViewBag.SearchTerm = q ?? string.Empty;
        ViewBag.SelectedCategory = category ?? string.Empty;
        ViewBag.SelectedSort = sort ?? "featured";

        ViewBag.Notice = TempData["Notice"];
        return View(result.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Өнім қосу";
        PopulateFormData();
        return View(new Product { IsAvailable = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? photo)
    {
        if (!Categories.Contains(product.Category))
            ModelState.AddModelError(nameof(product.Category), "Тізімнен санат таңдаңыз.");

        product.ImageUrl = product.ImageUrl?.Trim();
        if (!string.IsNullOrWhiteSpace(product.ImageUrl) && !IsAllowedImageUrl(product.ImageUrl))
            ModelState.AddModelError(nameof(product.ImageUrl), "HTTPS сілтемесін немесе /images/ ішіндегі жергілікті суретті қолданыңыз.");

        if (photo is { Length: > 0 })
        {
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
                ModelState.AddModelError("photo", "JPG, PNG немесе WEBP фотосын таңдаңыз.");
            else if (photo.Length > 5 * 1024 * 1024)
                ModelState.AddModelError("photo", "Фотосурет өлшемі 5 MB-тан аспауы керек.");
            else if (ModelState.IsValid)
            {
                var uploadFolder = Path.Combine(environment.WebRootPath, "images", "uploads");
                Directory.CreateDirectory(uploadFolder);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await photo.CopyToAsync(stream);
                product.ImageUrl = $"/images/uploads/{fileName}";
            }
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Өнім қосу";
            PopulateFormData();
            return View(product);
        }

        catalog.Add(product);
        TempData["Notice"] = "Жаңа өнім жинаққа қосылды.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var product = catalog.GetById(id);
        if (product is null) return NotFound();
        ViewData["Title"] = product.Name;
        return View(product);
    }

    private void PopulateFormData()
    {
        ViewBag.Categories = new SelectList(Categories);
    }

    private static bool IsAllowedImageUrl(string value)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return uri.Scheme == Uri.UriSchemeHttps;

        return value.StartsWith("/images/", StringComparison.OrdinalIgnoreCase)
            && !value.Contains("..", StringComparison.Ordinal);
    }
}
