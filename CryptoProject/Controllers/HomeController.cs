using CryptoProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.Web3;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

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

        [HttpPost]
        public async Task<IActionResult> GetBalance(string address)
        {
            var web3 = new Web3("https://mainnet.infura.io/v3/7005a3a8653c4819840c862e01b0ce94");
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            ViewBag.BalanceInWei = balance.Value;
            ViewBag.BalanceInEther = Web3.Convert.FromWei(balance.Value);
            ViewBag.Address = address;

            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> GetFunctions(string contractAddress)
        {
            string contractAbi = await GetContractABI(contractAddress);
            var contract = new Contract(null, contractAbi, contractAddress);
            var functions = contract.ContractBuilder.ContractABI.Functions;

            List<string> functionSignatures = new List<string>();
            foreach (var function in functions)
            {
                var inputParams = string.Join(", ", function.InputParameters.Select(p => $"{p.Type} {p.Name}"));
                var outputParams = string.Join(", ", function.OutputParameters.Select(p => $"{p.Type} {p.Name}"));
                var functionSignature = $"{function.Name}({inputParams})";
                functionSignatures.Add(functionSignature);
            }

            ViewBag.Functions = functionSignatures;
            ViewBag.ContractAddress = contractAddress;
            return View("Index");
        }
            private async Task<string> GetContractABI(string contractAddress)
        {
            // Replace with your Etherscan API key
            string etherscanApiKey = "7UI59WUIDUBUDJRQDNUG45WQ5CY4R8SWX2";

            // Create an HttpClient to make the request
            HttpClient httpClient = new HttpClient();

            // Build the Etherscan API request URL
            string requestUrl = $"https://api.etherscan.io/api?module=contract&action=getabi&address={contractAddress}&apikey={etherscanApiKey}";

            // Make the request
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

            // Ensure a successful response
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response to a dynamic object
            JsonDocument jsonResponse = JsonDocument.Parse(responseContent);
            string contractAbi = jsonResponse.RootElement.GetProperty("result").GetString();
            // Return the ABI string
            return contractAbi;
        }
        public async Task<IActionResult> GetFunctionOutput(string contractAddress, string functionName)
        {
            string contractAbi = await GetContractABI(contractAddress);
            var contract = web3.Eth.GetContract(contractAbi, contractAddress);

            // Get the function
            var function = contract.GetFunction(functionName);

            // Get the function ABI
            var functionABI = contract.ContractBuilder.ContractABI.Functions.FirstOrDefault(f => f.Name == functionName);
            if (functionABI == null)
            {
                // Handle the case when the function is not found in the ABI
                return View("Index");
            }

            // Check the output type
            var outputParameter = functionABI.OutputParameters[0];
            if (outputParameter.ABIType.Name.StartsWith("int"))
            {
                var result = await function.CallAsync<int>();
                ViewBag.FunctionOutput = result.ToString(); // Convert the integer output to a string
            }
            else if (outputParameter.ABIType.Name == "string")
            {
                var result = await function.CallAsync<string>();
                ViewBag.FunctionOutput = result;
            }
            else
            {
                // Handle the case when the output type is not supported
                ViewBag.FunctionOutput = "unsupported";
            }

            return View("Index");
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