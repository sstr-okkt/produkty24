using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Models.Entities;
using Produkty24_Web.Models;
using System.Text;
using Produkty24_Web.ViewModels.StockArrivals;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Produkty24_Web.Extensions;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class StockArrivalsController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public StockArrivalsController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<StockArrivalViewModel>();

            using (var response = await httpClient.GetAsync($"api/stockarrivals?page={page}&pageSize={pageSize}")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<StockArrivalViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }

        public async Task<IActionResult> Create()
        {
            await SetAllStockItemsListToViewBagAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] StockArrivalCreateViewModel stockArrival)
        {
            if (!ModelState.IsValid) {
                await SetAllStockItemsListToViewBagAsync();

                return View(stockArrival);
            }

            var serializedStockArrival = JsonConvert.SerializeObject(stockArrival, Formatting.Indented);
            var httpContent = new StringContent(serializedStockArrival, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"api/stockarrivals", httpContent)) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            StockArrivalEditViewModel stockArrival;

            using (var response = await httpClient.GetAsync($"api/stockarrivals/{id}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                stockArrival = JsonConvert.DeserializeObject<StockArrivalEditViewModel>(apiResponse);
            }

            await SetAllStockItemsListToViewBagAsync();

            return View(stockArrival);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, StockArrivalEditViewModel stockArrival)
        {
            if (stockArrival.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                await SetAllStockItemsListToViewBagAsync();

                return View(stockArrival);
            }

            var serializedStockArrival = JsonConvert.SerializeObject(stockArrival, Formatting.Indented);
            var httpContent = new StringContent(serializedStockArrival, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/stockarrivals", httpContent)) {
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

            using (var response = await httpClient.DeleteAsync($"api/stockarrivals/{id}")) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        private async Task SetAllStockItemsListToViewBagAsync()
        {
            IEnumerable<StockItemEntity> stockItems;

            using (var response = await httpClient.GetAsync($"api/stockitems/list")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                stockItems = JsonConvert.DeserializeObject<IEnumerable<StockItemEntity>>(apiResponse);
            }

            ViewBag.StockItems = stockItems;
        }
    }
}
