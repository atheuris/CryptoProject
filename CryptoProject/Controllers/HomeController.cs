using CryptoProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            var web3 = new Web3("https://mainnet.infura.io/v3/7005a3a8653c4819840c862e01b0ce94");
            var balance = await web3.Eth.GetBalance.SendRequestAsync("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");
            ViewBag.BalanceInWei = balance.Value;
            ViewBag.BalanceInEther = Web3.Convert.FromWei(balance.Value);

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


        


    }
}