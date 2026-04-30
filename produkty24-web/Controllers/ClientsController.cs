using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Produkty24_Web.Extensions;
using Produkty24_Web.Models;
using Produkty24_Web.Models.Entities;
using Produkty24_Web.ViewModels.Clients;
using Newtonsoft.Json;
using System.Text;

namespace Produkty24_Web.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly HttpClient httpClient;

        public ClientsController(IDateTimeProvider dateTimeProvider, IHttpClientFactory factory)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.httpClient = factory.CreateClient("apiClient");
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var pageInfo = new PageInfo<ClientViewModel>();

            using (var response = await httpClient.GetAsync($"api/clients?page={page}&pageSize={pageSize}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                pageInfo = JsonConvert.DeserializeObject<PageInfo<ClientViewModel>>(apiResponse);
            }

            return View(pageInfo);
        }

        public async Task<IActionResult> Create()
        {
            await SetAllCountriesListToViewBagAsync();
            await SetAllShippingMethodsListToViewBagAsync();

            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ClientCreateViewModel client)
        {
            if (!ModelState.IsValid)
            {
                await SetAllCountriesListToViewBagAsync();
                await SetAllShippingMethodsListToViewBagAsync();

                return View(client);                
            }

            var serializedClient = JsonConvert.SerializeObject(client, Formatting.Indented);
            var httpContent = new StringContent(serializedClient, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"api/clients", httpContent))
            {
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

            ClientEditViewModel client;

            using (var response = await httpClient.GetAsync($"api/clients/{id}"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                client = JsonConvert.DeserializeObject<ClientEditViewModel>(apiResponse);
            }

            await SetAllCountriesListToViewBagAsync();
            await SetAllShippingMethodsListToViewBagAsync();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, ClientEditViewModel client)
        {
            if (client.Id != id) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                await SetAllCountriesListToViewBagAsync();
                await SetAllShippingMethodsListToViewBagAsync();

                return View(client);                
            }

            var serializedClient = JsonConvert.SerializeObject(client, Formatting.Indented);
            var httpContent = new StringContent(serializedClient, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PutAsync($"api/clients", httpContent))
            {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            using (var response = await httpClient.DeleteAsync($"api/clients/{id}"))
            {
                response.ThrowOnHttpError();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Profile(int id)
        {
            ClientProfileViewModel client;

            using (var response = await httpClient.GetAsync($"api/clients/{id}/profile"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                client = JsonConvert.DeserializeObject<ClientProfileViewModel>(apiResponse);
            }

            return View(client);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        private async Task SetAllCountriesListToViewBagAsync()
        {
            IEnumerable<CountryEntity> countries;

            using (var response = await httpClient.GetAsync($"api/countries/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                countries = JsonConvert.DeserializeObject<IEnumerable<CountryEntity>>(apiResponse);
            }

            ViewBag.Countries = countries;            
        }

        private async Task SetAllShippingMethodsListToViewBagAsync()
        {
            IEnumerable<ShippingMethodEntity> shippingMethods;

            using (var response = await httpClient.GetAsync($"api/shippingmethods/list"))
            {
                response.ThrowOnHttpError();

                var apiResponse = await response.Content.ReadAsStringAsync();
                shippingMethods = JsonConvert.DeserializeObject<IEnumerable<ShippingMethodEntity>>(apiResponse);
            }

            ViewBag.ShippingMethods = shippingMethods;
        }
    }
}