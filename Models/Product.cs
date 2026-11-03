using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Models;

public sealed class Product
{
    public int Id { get; set; }

    [Required, StringLength(70, MinimumLength = 2)]
    [Display(Name = "Атауы")]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "100000000")]
    [Display(Name = "Бағасы")]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Санаты")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Қоймада бар")]
    public bool IsAvailable { get; set; }

    [StringLength(260)]
    [Display(Name = "Қысқаша сипаттама")]
    public string Description { get; set; } = string.Empty;

    public int? PhotoIndex { get; set; }

    [StringLength(1000)]
    [Display(Name = "Сурет сілтемесі (HTTPS)")]
    public string? ImageUrl { get; set; }
}
