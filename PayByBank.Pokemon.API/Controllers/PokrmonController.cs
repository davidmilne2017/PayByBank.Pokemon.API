using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.API.Controllers
{
    public class PokrmonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
