using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;

namespace GDShop.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        GDWebServiceClient db = new GDWebServiceClient();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky(string thongbao)
        {
            ViewBag.checkDangNhap = thongbao;
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(Customer kh)
        {
            
            int i = db.checkTaiKhoan(kh.taikhoan);
            kh.matkhau = BaoMatRatCaoController.DoiSangMD5(kh.matkhau);
            if (i == 1)
            {
                ViewBag.checkDK = "Tài khoản đã có người sử dụng";
                return View();
            }
            else
            {
                ViewBag.checkDK = "";
                ViewBag.checkInsert = db.insertCustomer(kh);
                ViewBag.checkDK = "Đăng ký thành công";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection f)
        {
            string tk = f.Get("txtTaiKhoan").ToString();
            string mk = f.Get("txtMatKhau").ToString();
            mk = BaoMatRatCaoController.DoiSangMD5(mk);
            Customer kh = db.checkDangNhap(tk,mk);
            if (kh != null)
            {
                Session["TaiKhoan"] = kh;
                ViewBag.checkDangNhap = "Đăng nhập thành công";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Dangky", "Customer", new { thongbao = "Đăng nhập thất bại: Tài khoản hoặc mật khẩu không đúng" });
            }
                
            
        }
        public ActionResult DangNhapPartial()
        {
            Customer kh = (Customer)Session["TaiKhoan"];
            if (kh != null)
            {
                ViewBag.tk = "Xin Chào! "+kh.tenkh;
            }
            else
                ViewBag.tk = "Tài Khoản";
            return PartialView();
            
        }
        public ActionResult DangNhapTKPartial()
        {
            Customer kh = (Customer)Session["TaiKhoan"];
            if (kh == null)
            {
                return RedirectToAction("Dangky", "Customer");
            }
            return RedirectToAction("QLTaiKhoan", "Customer");
        }
        public ActionResult ThoatTaiKhoan()
        {
            if (Session["TaiKhoan"] != null)
                Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult QLTaiKhoan(string thongbao)
        {
            ViewBag.taikhoan = (Customer)Session["TaiKhoan"];
            ViewBag.thongbao1 = thongbao;
            ViewBag.listDDH = db.getListDDHTheoID(ViewBag.taikhoan.id);
            return View();
        }
        [HttpPost]
        public ActionResult QLTaiKhoan(FormCollection f)
        {
            
            ViewBag.taikhoan = (Customer)Session["TaiKhoan"];
            Customer kh = (Customer)Session["TaiKhoan"];
            ViewBag.listDDH = db.getListDDHTheoID(ViewBag.taikhoan.id);
            kh.tenkh = f.Get("txtHoTen").ToString();
            kh.email = f.Get("txtEmail").ToString();
            kh.dienthoai = f.Get("txtSDT").ToString();
            kh.diachikh = f.Get("txtDiaChi").ToString();
            int updateCustomer = db.updateCustomer(kh);
            if(updateCustomer == 1)
            {
                ViewBag.thongbao = "Cập nhật thành công";
                return View();
            }
            else
                ViewBag.thongbao = "Cập nhật thất bại";
            return View();
        }
        [HttpPost]
        public ActionResult DoiMatKhau(FormCollection f)
        {
            Customer kh = (Customer)Session["TaiKhoan"];
            kh.matkhau = f.Get("txtMatKhau").ToString();
            int updateCustomer = db.updateCustomer(kh);
            if (updateCustomer == 1)
            {
                return RedirectToAction("QLTaiKhoan", "Customer", new { thongbao = "Đổi mật khẩu thành công" });
            }
            return RedirectToAction("QLTaiKhoan", "Customer", new { thongbao = "Đổi mật khẩu thất bại" });
        }

    }
}