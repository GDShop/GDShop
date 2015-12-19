using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;

namespace GDShop.Controllers
{
    public class ProductController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        // GET: Product
        public ViewResult DetailProduct(int id)
        {
            Product l = db.getProduct(id);
            ViewBag.brand = db.getBrandByID((int)l.idbrand);
            ViewBag.type = db.getTypeByID((int)l.idloai);
            ViewBag.q = db.getQByID((int)l.idchatlieu);
            if (l == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(l);
        }
        
        public PartialViewResult showSPTheoLoaiPartial()
        {
            ViewBag.tabCLB = db.getListSPTheoLoai("Câu Lạc Bộ",0);
            ViewBag.tabDT = db.getListSPTheoLoai("Đội Tuyển",0);
            ViewBag.tabT90 = db.getListSPTheoLoai("T90",0);
            ViewBag.tabTM = db.getListSPTheoLoai("Thủ Môn",0);
            ViewBag.tabCN = db.getListSPTheoLoai("Cho Nữ",0);
            return PartialView();
        }

        public int TimSLSPTheoIDVaSize(int id, string size)
        {
            return db.searchSize(id,size);
        }
        public ActionResult RecommenedProductPartial()
        {
            return View();
        }
    }
}