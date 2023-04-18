using CryptoProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Nethereum.Web3;
using static System.Net.WebRequestMethods;

namespace CryptoProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static string infuraUrl = "https://mainnet.infura.io/v3/7005a3a8653c4819840c862e01b0ce94";
        private static Web3 web3 = new Web3(infuraUrl);

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        [HttpGet]
        public async Task<ActionResult> GetBalanceAsync(string address)
        {
            try
            {
                var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
                var etherAmount = Web3.Convert.FromWei(balance.Value);
                return Json(new { success = true, balance = etherAmount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}