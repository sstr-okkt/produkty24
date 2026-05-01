using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Models.Entities;
using Produkty24_Web.Models;
using System.Text;
using Produkty24_Web.ViewModels.Orders;
using Produkty24_Web.ViewModels.Clients;
using Newtonsoft.Json;
using Produkty24_Web.Extensions;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public OrdersController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<OrderViewModel>();

            using (var response = await httpClient.GetAsync($"api/orders?page={page}&pageSize={pageSize}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<OrderViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }        

        public async Task<IActionResult> Create()
        {
            await SetAllClientsListToViewBagAsync();
            await SetOrderStatusesListToViewBagAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] OrderCreateViewModel order)
        {
            if (!ModelState.IsValid) {
                await SetAllClientsListToViewBagAsync();
                await SetOrderStatusesListToViewBagAsync();

                return View(order);
            }

            var serializedClient = JsonConvert.SerializeObject(order, Formatting.Indented);
            var httpContent = new StringContent(serializedClient, Encoding.UTF8, "application/json");

            var createdOrder = new OrderViewModel();

            using (var response = await httpClient.PostAsync($"api/orders", httpContent))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                createdOrder = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
            }

            return RedirectToAction("Edit", new { id = createdOrder.Id });
        }

        public async Task<IActionResult> CreateFast([FromRoute] int id)
        {
            var order = new OrderCreateViewModel() { ClientId = id };

            var serializedOrder = JsonConvert.SerializeObject(order, Formatting.Indented);
            var httpContent = new StringContent(serializedOrder, Encoding.UTF8, "application/json");

            OrderViewModel createdOrder;

            using (var response = await httpClient.PostAsync($"api/orders", httpContent))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                createdOrder = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
            }

            TempData["OrderId"] = createdOrder.Id;

            return RedirectToAction("Edit", new { id = createdOrder.Id });
        }

        public async Task<IActionResult> Edit([FromRoute] int? id, [FromQuery(Name = "orderId")] int? orderId)
        {
            if ((id == null || id < 1) && orderId != null && orderId > 0)
            {
                id = orderId;
            }

            if (id == null || id < 1) {
                if (TempData["OrderId"] is int tempOrderId && tempOrderId > 0)
                {
                    id = tempOrderId;
                }
                else
                {
                    return NotFound();
                }
            }

            OrderEditViewModel order;

            using (var response = await httpClient.GetAsync($"api/orders/{id}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                order = JsonConvert.DeserializeObject<OrderEditViewModel>(apiResponse);
            }

            await SetOrderStatusesListToViewBagAsync();           
            
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, OrderEditViewModel order)
        {
            if (order.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                await SetOrderStatusesListToViewBagAsync();

                return View(order);                
            }

            var serializedOrder = JsonConvert.SerializeObject(order, Formatting.Indented);
            var httpContent = new StringContent(serializedOrder, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/orders", httpContent)) {
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

            using (var response = await httpClient.DeleteAsync($"api/orders/{id}")) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        private async Task SetAllClientsListToViewBagAsync()
        {
            IEnumerable<ClientViewModel> clients;

            using (var response = await httpClient.GetAsync($"api/clients/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                clients = JsonConvert.DeserializeObject<IEnumerable<ClientViewModel>>(apiResponse);
            }

            ViewBag.Clients = clients;
        }

        private async Task SetOrderStatusesListToViewBagAsync()
        {
            IEnumerable<OrderStatusEntity> orderStatuses;

            using (var response = await httpClient.GetAsync($"api/orderstatuses/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                orderStatuses = JsonConvert.DeserializeObject<IEnumerable<OrderStatusEntity>>(apiResponse);
            }

            ViewBag.Statuses = orderStatuses;
        }
    }
}
