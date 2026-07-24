using LaptopCart.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace LaptopCart.Controllers
{
    public class SupportController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SupportController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult CreateTicket()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketViewModel model)
        {
            //Validating the model
            if (!ModelState.IsValid)
                return View(model);
            //Getting the UserID and assigning it to CreateTicketViewModel.UserID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;

         //Here we have all data now in the model from the form(CreateTicket View-UI)
        //Subject : From UI
        // Description : From UI
        // UserId: in the above code we are getting the user and assigning it to model.UserID
        // Status : "Open"; Hardcoded By default
        //Id : DB create it automaticatlly starting with 0 by default

        //to pass the model data to the web api project, we need to use HttpClient to send a POST request to the API endpoint
        var client = _httpClientFactory.CreateClient();// Register this HttpClient in Program.cs file using DI
            var token = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                //Passing the model to the api project in JSON format using Post method and storing the response in a variable
                var response = await client.PostAsJsonAsync("https://localhost:7142/api/Tickets/Create", model);
                if (response.IsSuccessStatusCode)
                {
                    //if the status code is 200 then we are redirect to TicketList View and displaying the list of created tickets
                    return RedirectToAction("TicketList");
                }
                ModelState.AddModelError("", "Something went wrong");
            }
            catch (Exception ex)
            {


            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreateTicketViewModel model)
        {
          
            //to pass the model data to the web api project, we need to use HttpClient to send a POST request to the API endpoint
            var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("https://localhost:1760/api/Tickets/Create");
            try
            {
                var response = await client.PutAsJsonAsync($"https://localhost:7142/api/Tickets/{model.Id}", model);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Update Failed");
                   return View(model);
                }
               
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("TicketList");
        }


        public async Task<IActionResult> TicketList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await client.GetAsync($"https://localhost:7142/api/Tickets");
                if (response.IsSuccessStatusCode)
                {
                    var tickets = await response.Content.ReadFromJsonAsync<List<CreateTicketViewModel>>();
                    return View(tickets);
                }
                ModelState.AddModelError("", "Unable to fetch tickets");
            }
            catch (Exception ex)
            {
            }
            return View(new List<CreateTicketViewModel>());
        }
        public async Task<IActionResult> Edit(string Id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync($"https://localhost:7142/api/Tickets/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var ticket = await response.Content.ReadFromJsonAsync<CreateTicketViewModel>();
                    return View(ticket);
                }
                ModelState.AddModelError("", "Something went wrong");
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public async Task<IActionResult> Delete(string Id)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"https://localhost:7142/api/Tickets/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("TicketList");
                }
                ModelState.AddModelError("", "Something went wrong");
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("TicketList");
        }

    }
}
