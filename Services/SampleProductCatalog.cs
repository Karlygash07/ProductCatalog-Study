using ProductCatalog.Models;

namespace ProductCatalog.Services;

public sealed class SampleProductCatalog : IProductCatalog
{
    private readonly object _gate = new();
    private readonly List<Product> _items =
    [
        new() { Id=1, PhotoIndex=1, Name="Aero 14 ноутбугі", Price=589000, Category="Ноутбуктер", IsAvailable=true, Description="Күнделікті жұмысқа арналған жеңіл алюминий ноутбук." },
        new() { Id=2, PhotoIndex=2, Name="Cobalt One", Price=429000, Category="Смартфондар", IsAvailable=true, Description="Жарқын экран мен жинақы камера жүйесі бар смартфон." },
        new() { Id=3, PhotoIndex=3, Name="Cloud Buds", Price=64900, Category="Аудио", IsAvailable=false, Description="Қалтаға оңай сыятын сымсыз құлаққап және қуаттау қапшығы." },
        new() { Id=4, PhotoIndex=4, Name="Tempo Watch", Price=119000, Category="Аксессуарлар", IsAvailable=true, Description="Қадам, белсенділік және күнделікті хабарламаларды бақылайды." },
        new() { Id=5, PhotoIndex=5, Name="Room Speaker", Price=79900, Category="Аудио", IsAvailable=true, Description="Үйге арналған жылы дыбысы бар ықшам Bluetooth динамигі." },
        new() { Id=6, PhotoIndex=6, Name="Keyline 75", Price=54900, Category="Аксессуарлар", IsAvailable=true, Description="Жұмыс үстеліне арналған жинақы механикалық пернетақта." },
        new() { Id=7, PhotoIndex=7, Name="Pebble Mouse", Price=22900, Category="Аксессуарлар", IsAvailable=true, Description="Жеңіл пішіні мен дәл сенсоры бар сымсыз тінтуір." },
        new() { Id=8, PhotoIndex=8, Name="Frame 27", Price=219000, Category="Аксессуарлар", IsAvailable=false, Description="Үй кеңсесіне арналған жұқа жиекті кең экранды монитор." },
        new() { Id=9, PhotoIndex=9, Name="Focus M2", Price=349000, Category="Аксессуарлар", IsAvailable=true, Description="Күнделікті фото мен видеоға арналған ықшам камера." },
        new() { Id=10, PhotoIndex=10, Name="Paper Tab", Price=149000, Category="Аксессуарлар", IsAvailable=true, Description="Оқуға және жазбаға арналған жеңіл электронды планшет." },
        new() { Id=11, PhotoIndex=11, Name="Studio Arc", Price=139000, Category="Аудио", IsAvailable=true, Description="Ұзақ тыңдауға арналған жұмсақ қаптамалы студиялық құлаққап." },
        new() { Id=12, PhotoIndex=12, Name="Beam Mini", Price=179000, Category="Аксессуарлар", IsAvailable=false, Description="Шағын кеңістікке лайық жеңіл портативті проектор." }
    ];
    private int _nextId = 13;

    public IReadOnlyList<Product> GetAll()
    {
        lock (_gate) return _items.Select(Clone).ToArray();
    }

    public Product? GetById(int id)
    {
        lock (_gate)
        {
            var product = _items.FirstOrDefault(item => item.Id == id);
            return product is null ? null : Clone(product);
        }
    }

    public Product Add(Product product)
    {
        lock (_gate)
        {
            product.Id = _nextId++;
            var saved = Clone(product);
            _items.Add(saved);
            return Clone(saved);
        }
    }

    private static Product Clone(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Category = product.Category,
        IsAvailable = product.IsAvailable,
        Description = product.Description,
        PhotoIndex = product.PhotoIndex,
        ImageUrl = product.ImageUrl
    };
}
