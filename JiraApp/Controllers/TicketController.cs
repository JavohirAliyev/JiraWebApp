using JiraApp.Models;
using JiraApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;

namespace JiraApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly JiraService _jiraService;

        public TicketController(JiraService jiraService)
        {
            _jiraService = jiraService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateTicket()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(string summary, string priority)
        {
            if (string.IsNullOrEmpty(summary) || string.IsNullOrEmpty(priority))
            {
                ModelState.AddModelError("Empty String", "Summary and Priority have to be filled");
                return View();
            }

            var userEmail = User.Identity.Name;
            var collectionName = "Collection Name";
            var pageLink = Request.Path;

            var result = await _jiraService.CreateTicket(summary, priority, userEmail, collectionName, pageLink);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(result);
            }

            return RedirectToAction("ViewTickets", "Ticket");
        }

        public async Task<IActionResult> ViewTickets()
        {
            var projectKey = "KAN";
            var tickets = await _jiraService.GetTickets(projectKey);

            if (!tickets.Success)
            {
                ViewBag.Errors = tickets.Errors;
                return View(new List<Ticket>());
            }

            return View(tickets.Data);
        }
    }
}
