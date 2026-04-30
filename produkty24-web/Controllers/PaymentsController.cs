using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Models.Entities;
using Produkty24_Web.Models;
using System.Text;
using Produkty24_Web.ViewModels.Payments;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Produkty24_Web.Extensions;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public PaymentsController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<PaymentViewModel>();

            using (var response = await httpClient.GetAsync($"api/payments?page={page}&pageSize={pageSize}")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<PaymentViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }

        public async Task<IActionResult> CreateFast([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            OrderEntity order;

            using (var response = await httpClient.GetAsync($"api/orders/{id}")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                order = JsonConvert.DeserializeObject<OrderEntity>(apiResponse);
            }

            var newPayment = new PaymentCreateViewModel();

            newPayment.ClientId = order.ClientId;
            newPayment.OrderId = order.Id;

            return View(newPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFast([FromForm] PaymentCreateViewModel payment)
        {
            if (!ModelState.IsValid) {
                return View(payment);
            }

            var serializedPayment = JsonConvert.SerializeObject(payment, Formatting.Indented);
            var httpContent = new StringContent(serializedPayment, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"api/payments", httpContent)) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Edit", "Orders", new { id = payment.OrderId });
        }

        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null || id < 1) {
                return NotFound();
            }

            PaymentEditViewModel payment;

            using (var response = await httpClient.GetAsync($"api/payments/{id}")) {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                payment = JsonConvert.DeserializeObject<PaymentEditViewModel>(apiResponse);
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, PaymentEditViewModel payment)
        {
            if (payment.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return View(payment);                
            }

            var serializedPayment = JsonConvert.SerializeObject(payment, Formatting.Indented);
            var httpContent = new StringContent(serializedPayment, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/payments", httpContent)) {
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

            using (var response = await httpClient.DeleteAsync($"api/payments/{id}")) {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }
    }
}
