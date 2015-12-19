using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;

namespace GDShop.Controllers
{
    public class DonDatHangController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        // GET: DonDatHang
        public ActionResult ListCTDDH(int id)
        {
            ViewBag.ddh = db.getDDHTheoID(id);
            ViewBag.kh = db.getKHTheoID(ViewBag.ddh.idkhachhang);
            ViewBag.tinhtrang = db.getTTHoaDonTheoID(ViewBag.ddh.tinhtrang);
            ViewBag.giaohang = db.timXeChuaIDDDH(ViewBag.ddh.id);
            if (ViewBag.giaohang != null)
                ViewBag.nhanvien = db.getNhanVienTheoID(ViewBag.giaohang.idnhanvien);
            else
            {
                ViewBag.nhanvien = new NhanVien();
                ViewBag.giaohang = new ShipCar();
            }
                
            List<Product> ll = new List<Product>();
            ViewBag.l = db.getListCTDDHTheoID(id);
            foreach(var item in ViewBag.l)
            {
                Product p = db.getProduct(item.idsanpham);
                ll.Add(p);
            }
            ViewBag.listP = ll;
            return View();
        }
        public String getProductByID(int id)
        {
            Product l = db.getProduct(id);
            if (l == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return l.image1;
        }

    }
}