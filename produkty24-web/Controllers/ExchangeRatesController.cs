using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Models;
using System.Text;
using Produkty24_Web.ViewModels.ExchangeRates;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Produkty24_Web.Extensions;
using Produkty24_Web.ViewModels.Currencies;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class ExchangeRatesController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public ExchangeRatesController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<ExchangeRateViewModel>();

            using (var response = await httpClient.GetAsync($"api/exchangerates?page={page}&pageSize={pageSize}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<ExchangeRateViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }

        public async Task<IActionResult> Create()
        {
            await SetAllCurrenciesListToViewBagAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ExchangeRateCreateViewModel exchangeRate)
        {
            if (!ModelState.IsValid)
            {
                await SetAllCurrenciesListToViewBagAsync();

                return View(exchangeRate);
            }

            var serializedExchangeRate = JsonConvert.SerializeObject(exchangeRate, Formatting.Indented);
            var httpContent = new StringContent(serializedExchangeRate, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"api/exchangerates", httpContent))
            {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            ExchangeRateEditViewModel exchangeRate;

            using (var response = await httpClient.GetAsync($"api/exchangerates/{id}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                exchangeRate = JsonConvert.DeserializeObject<ExchangeRateEditViewModel>(apiResponse);
            }

            await SetAllCurrenciesListToViewBagAsync();

            return View(exchangeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, ExchangeRateEditViewModel exchangeRate)
        {
            if (exchangeRate.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                await SetAllCurrenciesListToViewBagAsync();

                return View(exchangeRate);                
            }

            var serializedExchangeRate = JsonConvert.SerializeObject(exchangeRate, Formatting.Indented);
            var httpContent = new StringContent(serializedExchangeRate, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/exchangerates", httpContent))
            {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            using (var response = await httpClient.DeleteAsync($"api/exchangerates/{id}"))
            {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        private async Task SetAllCurrenciesListToViewBagAsync()
        {
            IEnumerable<CurrencyViewModel> currencies;

            using (var response = await httpClient.GetAsync($"api/currencies/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                currencies = JsonConvert.DeserializeObject<IEnumerable<CurrencyViewModel>>(apiResponse);
            }

            ViewBag.Currencies = currencies;
        }
    }
}
