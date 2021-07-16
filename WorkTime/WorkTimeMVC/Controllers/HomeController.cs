using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using WorkTimeLogic;
using WorkTimeMVC.Models;

namespace WorkTimeMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobService _jobService;
        private readonly IReportsService _reportsService;

        public HomeController(ILogger<HomeController> logger, IJobService jobService, IReportsService reportsService)
        {
            _logger = logger;
            _jobService = jobService;
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index()
        {
            HomeModel model = new()
            {
                Jobs = await _jobService.GetJobs(),
                Reports = await _reportsService.GetReports()
            };
            return View(model);
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
