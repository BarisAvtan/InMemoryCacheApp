using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCache.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            {
                _memoryCache.Set<string>("time", DateTime.Now.ToString());
            }
            if (_memoryCache.TryGetValue("time",out string timeCache))//inmemoryCache'den time değerini alabilirsen string timeCache degerine ata ve true dön
            {
                _memoryCache.Set<string>("time", DateTime.Now.ToString());
            }
            _memoryCache.Remove("time");//cacheden data siler

            _memoryCache.GetOrCreate<string>("time",entr=>
            {
                //entr.SlidingExpiration //gibi birçok degeri değiştirilebilir
                return DateTime.Now.ToString();
            }); //time varsa onu kullan yoksa oluştur

            return View();
        }
        //https://localhost:7033/product/show
        public IActionResult Show()
        {
        
            ViewBag.time = _memoryCache.Get<string>("time");
            return View();
        }
    }
}
