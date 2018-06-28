using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LongRunningSignalr.Models;
using Microsoft.AspNetCore.SignalR;

namespace LongRunningSignalr.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQueue _queue;
        private readonly IHubContext<JobProgressHub> _hubContext;

        public HomeController(IQueue queue, IHubContext<JobProgressHub> hubContext)
        {
            _queue = queue;
            _hubContext = hubContext;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult StartProgress()
        {
            string jobId = Guid.NewGuid().ToString("N");
            _queue.QueueAsyncTask(() => PerformBackgroundJob(jobId));

            return RedirectToAction("Progress", new {jobId});
        }

        public IActionResult Progress(string jobId)
        {
            ViewBag.JobId = jobId;

            return View();
        }

        private async Task PerformBackgroundJob(string jobId)
        {
            for (int i = 0; i <= 100; i += 5)
            {
                await _hubContext.Clients.Group(jobId).SendAsync("progress", i);

                await Task.Delay(100);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
