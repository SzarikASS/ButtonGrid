using ButtonGrid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ButtonGrid.Controllers
{
    public class ButtonController : Controller
    {
        static List<ButtonModel> buttons = new List<ButtonModel>();
        Random random = new Random();
        const int GridSize = 25;

        public IActionResult Index()
        {
            if (buttons.Count < GridSize)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    buttons.Add(new ButtonModel { Id = x, ButtonState = random.Next(4) });
                }
            }
            return View("Index", buttons);
        }

        public IActionResult HandleButtonClick(string buttonNumber)
        {
            int x = int.Parse(buttonNumber);
            buttons.ElementAt(x).ButtonState = (buttons.ElementAt(x).ButtonState + 1) % 4;

            return View("Index", buttons);
        }

        public IActionResult ShowOneButton(int buttonNumber)
        {

            buttons.ElementAt(buttonNumber).ButtonState = (buttons.ElementAt(buttonNumber).ButtonState + 1) % 4;

            string buttonString = RenderRazorViewToString(this, "ShowOneButton", buttons.ElementAt(buttonNumber));

            bool ifWin = true;

            for(int i = 0; i<buttons.Count;i++)
            {
                if (buttons.ElementAt(i).ButtonState != buttons.ElementAt(0).ButtonState)
                {
                    ifWin = false;
                }
            }

            string messageString="";

            if(ifWin == true)
            {
               messageString = "<p>Good work!</p>";
            }
            else
            {
                messageString = "<p>Not all buttons are the same</p>";
            }



            var package = new { part1 = buttonString, part2 = messageString};

            return Json(package);

            return PartialView(buttons.ElementAt(buttonNumber));
        }



        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
