using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Models.Entities;
using Produkty24_Web.Models;
using System.Text;
using Produkty24_Web.ViewModels.OrdersItems;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Produkty24_Web.Extensions;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class OrdersItemsController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public OrdersItemsController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<OrderItemViewModel>();

            using (var response = await httpClient.GetAsync($"api/ordersitems?page={page}&pageSize={pageSize}")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<OrderItemViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }

        public async Task<IActionResult> Create([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            var orderItem = new OrderItemCreateViewModel() { OrderId = id };
            await SetAllStockItemsListToViewBagAsync();

            return View(orderItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] OrderItemCreateViewModel orderItem)
        {
            if (!ModelState.IsValid) {
                await SetAllStockItemsListToViewBagAsync();

                return View(orderItem);
            }

            var serializedOrderItem = JsonConvert.SerializeObject(orderItem, Formatting.Indented);
            var httpContent = new StringContent(serializedOrderItem, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"api/ordersitems", httpContent)) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Edit", "Orders", new { id = orderItem.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            OrderItemEditViewModel orderItem;

            using (var response = await httpClient.GetAsync($"api/ordersitems/{id}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                orderItem = JsonConvert.DeserializeObject<OrderItemEditViewModel>(apiResponse);
            }

            await SetAllStockItemsListToViewBagAsync();                    

            return View(orderItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, OrderItemEditViewModel orderItem)
        {
            if (orderItem.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                await SetAllStockItemsListToViewBagAsync();

                return View(orderItem);                
            }

            var serializedOrderItem = JsonConvert.SerializeObject(orderItem, Formatting.Indented);
            var httpContent = new StringContent(serializedOrderItem, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/ordersitems", httpContent)) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Edit", "Orders", new { id = orderItem.OrderId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            using (var response = await httpClient.DeleteAsync($"api/ordersitems/{id}")) {
                response.ThrowOnHttpError();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }     

        private async Task SetAllStockItemsListToViewBagAsync()
        {
            IEnumerable<StockItemEntity> stockItems;

            using (var response = await httpClient.GetAsync($"api/stockitems/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                stockItems = JsonConvert.DeserializeObject<IEnumerable<StockItemEntity>>(apiResponse);
            }

            ViewBag.StockItems = stockItems;
        }
    }
}
