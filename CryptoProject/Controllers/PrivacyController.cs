using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CryptoProject.Controllers
{
    public class PrivacyController : Controller
    {
        private readonly HttpClient _httpClient;

        public PrivacyController()
        {
            _httpClient = new HttpClient();
        }

        public IActionResult Privacy()
        {
            ViewBag.ContractAddresses = JsonDocument.Parse("{}").RootElement;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Privacy(string walletAddress)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/get_contracts", new { wallet_address = walletAddress });
            var result = await response.Content.ReadAsStringAsync();
            var contractAddresses = JsonConvert.DeserializeObject<dynamic>(result).contract_addresses;
            ViewBag.ContractAddresses = contractAddresses;

            return View();
        }
    }
}
