using InMemoryCache.Models;
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
            //_memoryCache.Remove("time");//cacheden data siler

            //_memoryCache.GetOrCreate<string>("time",entr=>
            //{
            //    //entr.SlidingExpiration //gibi birçok degeri değiştirilebilir
            //    return DateTime.Now.ToString();
            //}); //time varsa onu kullan yoksa oluştur

            return View();
        }
        //https://localhost:7033/product/show
        public IActionResult Show()
        {
        
            ViewBag.time = _memoryCache.Get<string>("time");
            return View();
        }

        public IActionResult Show2()
        {

            if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);//AbsoluteExpiration 10 sn cache'de tutar
                options.SlidingExpiration = TimeSpan.FromSeconds(10);//SlidingExpiration 10 snde bu degere erişebilirse bu datanın ömrü 10 sn artar,10 sn de erişemezse memoryden silinir

                _memoryCache.Set<string>("time", DateTime.Now.ToString(),options);
            }
            return View();
        }

        //cache priority 
        public IActionResult Show3()
        {

            if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);//AbsoluteExpiration 10 sn cache'de tutar
                options.SlidingExpiration = TimeSpan.FromSeconds(10);//SlidingExpiration 10 snde bu degere erişebilirse bu datanın ömrü 10 sn artar,10 sn de erişemezse memoryden silinir
                options.Priority = CacheItemPriority.High;//Priority önceliği belirtirlir
                options.Priority = CacheItemPriority.NeverRemove;//asla silme fakat cache dolarsa ve hiçbirini silemezse exception fırlatılır.
                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);
            }
            return View();
        }

        //bir datanın hangi sebebden dolayı memoryden düştüğünü RegisterPostEvictionCallback method ile görebiliriz
        public IActionResult Show4()
        {

            if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);//AbsoluteExpiration 10 sn cache'de tutar
                options.SlidingExpiration = TimeSpan.FromSeconds(10);//SlidingExpiration 10 snde bu degere erişebilirse bu datanın ömrü 10 sn artar,10 sn de erişemezse memoryden silinir
                options.Priority = CacheItemPriority.High;
                options.Priority = CacheItemPriority.NeverRemove;

                options.RegisterPostEvictionCallback((key, value, reason, state) =>// cache'nin neden silindiğini bu şekikde anlayabiliriz.
                {
                    _memoryCache.Set("callback", $"{key}->{value}=> sebeb : {reason}");
                });//delegeler metodu işaret eder yani (key,value,reason,state) bu 4 tane parametre alan bir metodu işaret eder.

                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);
            }
            return View();
        }

        //complex types caching

        public IActionResult Show5()
        {
            Product p = new Product {Id = 1 , Name ="Pencil", Price =250 };

            _memoryCache.Set<Product>("product01", p);//product nesnesinin cachelenmesi
            _memoryCache.Set<double>("money", 105.12);
            ViewBag.product = _memoryCache.Get<Product>("product01");
            //htmlde 
            //Product p =ViewBag.product is Product //is product'a dönüştürebilirse true dönüştüremezse false döner
            //Product p =ViewBag.product as Product //as product'a dönüştürür
            //<h3>@p.Id => @p.Name</h3> şekilinde erişebilirsiniz.
            return View();
        }
    }
}
