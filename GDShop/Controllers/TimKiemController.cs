using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;
using PagedList.Mvc;
using PagedList;

namespace GDShop.Controllers
{
    public class TimKiemController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        // GET: TimKiem
        [HttpPost]
        public ActionResult TimKiem(FormCollection f, int? page)
        {
            string tk = f.Get("txtTimKiem").ToString();
            ViewBag.tukhoa = tk;
            var l = db.timKiemSPTheoTen(tk);
            int pageNumber = (page ?? 1);
            int pageSize = 12;
            if(l.Count() == 0)
            {
                ViewBag.thongbao = "Không tìm thấy sản phẩm nào";
                var ll = new List<Product>();
                return View(ll.ToPagedList(pageNumber, pageSize));
            }
            ViewBag.thongbao = "Đã tìm thấy " + l.Count() + " sản phẩm";
            return View(l.OrderBy(n => n.tensp).ToPagedList(pageNumber,pageSize));
        }
        [HttpGet]
        public ActionResult TimKiem(string tk, int? page)
        {
            ViewBag.tukhoa = tk;
            var l = db.timKiemSPTheoTen(tk);
            int pageNumber = (page ?? 1);
            int pageSize = 12;
            if (l.Count() == 0)
            {
                ViewBag.thongbao = "Không tìm thấy sản phẩm nào";
                return View();
            }
            ViewBag.thongbao = "Đã tìm thấy " + l.Count() + " sản phẩm";
            return View(l.OrderBy(n => n.tensp).ToPagedList(pageNumber, pageSize));
        }
    }
}