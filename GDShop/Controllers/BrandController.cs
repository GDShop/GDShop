using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;

namespace GDShop.Controllers
{
    public class BrandController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        // GET: Brand
        public PartialViewResult ThuonghieuPartial()
        {
            ViewBag.listBrand = db.getListBrand();
            
            return PartialView();
        }

        public ActionResult BrandProduct(string tenloai)
        {
            ViewBag.tenloai = tenloai;
            ViewBag.BrandProduct = db.getListSPTheoBrand(tenloai);
            return View();
        }

        public int CountProduct(string ten)
        {
            int a = db.countType(ten);
            return a;
        }
    }
}