using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;


namespace GDShop.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        GDWebServiceClient db = new GDWebServiceClient();
        
        public ActionResult Index()
        {
            ViewBag.listProduct = db.getListProduct();
            return View();
        }
        
    }
}