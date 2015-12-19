using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;
using PagedList;
using PagedList.Mvc;

namespace GDShop.Controllers
{
    public class TypeController : Controller
    {
        // GET: Type
        GDWebServiceClient db = new GDWebServiceClient();
        public ActionResult LoaiProduct(string tenloai, int? page)
        {
            int pageSize = 12;
            int pageNumber = ((int?)page ?? 1);
            ViewBag.tenloai = tenloai;
            var a = db.getListSPTheoType(tenloai).ToPagedList(pageNumber,pageSize);
            return View(a);
        }
        public ActionResult LoaiProduct1(string tenloai, int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.tenloai = tenloai;
            var a = db.getListSPTheoLoai(tenloai,1).ToPagedList(pageNumber, pageSize);
            return View(a);
        }
    }
}