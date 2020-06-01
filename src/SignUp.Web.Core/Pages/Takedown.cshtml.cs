using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignUp.Web.Core.Pages
{
    public class TakedownModel : PageModel
    {
        public static bool IsOffline { get; private set; }

        public void OnGet()
        {
            IsOffline = true;
        }
    }
}