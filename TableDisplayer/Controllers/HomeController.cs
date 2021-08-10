using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TableDisplayer.Models;

namespace TableDisplayer.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return RedirectToAction("GetLex", "Sales");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("~/.well-known/pki-validation/5ECE9808BF4D0D668F9659B5D591F467.txt")]
        public VirtualFileResult GetVerificationFile() {
            var path = Path.Combine("~/files", "5ECE9808BF4D0D668F9659B5D591F467.txt");
            return File(path, "text/plain", "5ECE9808BF4D0D668F9659B5D591F467.txt");
        }
    }
}
