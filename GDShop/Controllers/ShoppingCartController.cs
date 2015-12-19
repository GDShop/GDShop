using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.Models;
using GDShop.GDWebservice;
using GDShop.Controllers;

namespace GDShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        #region ShoppingCart
        public List<ShoppingCart> GetShoppingCart() //Lấy giỏ hàng
        {
            List<ShoppingCart> listSC = Session["ShoppingCart"] as List<ShoppingCart>; //ep kieu session, neu chu ton tai = null
            if(listSC == null)
            {
                listSC = new List<ShoppingCart>();
                Session["ShoppingCart"] = listSC;
            }
            return listSC;
        }
        public ActionResult AddShoppingCart(int MaSP, string Url) //Thêm giỏ hàng
        {
            Product p = db.getProduct(MaSP);
            if(p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<ShoppingCart> listSC = GetShoppingCart();
            ShoppingCart sc = listSC.Find(n => n.MaSP == MaSP);
            if(sc == null)
            {
                sc = new ShoppingCart(MaSP);
                listSC.Add(sc);
                return Redirect(Url);
            }
            else
            {
                sc.SoLuong++;
                return Redirect(Url);
            }
        }
        public ActionResult UpdateShoppingCart(int MaSP, FormCollection f)
        {
            ProductController pc = new ProductController();
            Product p = db.getProduct(MaSP);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<ShoppingCart> listSC = GetShoppingCart();
            ShoppingCart sc = listSC.Find(n => n.MaSP == MaSP);
            if(sc != null)
            {
                sc.Size = f["size"].ToString();
                int z = pc.TimSLSPTheoIDVaSize(MaSP, sc.Size);
                if (z > 0 && z > sc.SoLuong)
                    sc.SoLuong = int.Parse(f["txtSoluong-"+ MaSP].ToString());
                if (z == 0)
                    sc.SoLuong = 0;
            }
            return RedirectToAction("ShoppingCart");
        }
        public ActionResult DeletecShoppingCart(int MaSP)
        {

            Product p = db.getProduct(MaSP);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<ShoppingCart> listSC = GetShoppingCart();
            ShoppingCart sc = listSC.Find(n => n.MaSP == MaSP);
            if(sc != null)
            {
                listSC.RemoveAll(n => n.MaSP == MaSP);
            }
            if(listSC.Count()==0)
                return RedirectToAction("Index","Home");
            return RedirectToAction("ShoppingCart");
        }
        public ActionResult ShoppingCart()
        {
            if(Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index","Home");
            }
            List<ShoppingCart> listSC = GetShoppingCart();
            return View(listSC);
        }
        public int TongSoLuong()
        {
            int tongsl = 0;
            List<ShoppingCart> listSC = Session["ShoppingCart"] as List<ShoppingCart>;

            if(listSC != null)
            {
                tongsl = listSC.Sum(n => n.SoLuong);
            }
            return tongsl;
        }
        public double TongTien()
        {
            double tongtien = 0;
            List<ShoppingCart> listSC = Session["ShoppingCart"] as List<ShoppingCart>;
            if (listSC != null)
            {
                tongtien = listSC.Sum(n => n.TongTien);
            }
            return tongtien;
        }
        public int SoSanPham()
        {
            List<ShoppingCart> listSC = Session["ShoppingCart"] as List<ShoppingCart>;
            if (listSC != null)
            {
                return listSC.Count();
            }
            return 0;
        }
        #endregion
        #region Đặt hàng
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            if(Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangky","Customer");
            }
            if(Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.listSC = GetShoppingCart();
            DonDatHang ddh = new DonDatHang();
            Customer kh = (Customer)Session["TaiKhoan"];
            List<ShoppingCart> listSC = GetShoppingCart();
            ddh.tinhtrang = 1;
            ddh.idkhachhang = kh.id;
            ddh.ngaydathang = DateTime.Now;
            ddh.tongtien = (int)TongTien();
            int check = db.insertDonDatHang(ddh);
            int id = db.getDonDatHang(ddh.idkhachhang, ddh.ngaydathang);
            if(check == 1)
            {
                foreach (var item in listSC)
                {
                    CTDonDatHang ct = new CTDonDatHang();
                    ct.iddonhang = id;
                    ct.idsanpham = item.MaSP;
                    ct.soluong = item.SoLuong;
                    ct.size = item.Size;
                    int check1 = db.insertCTDonDatHang(ct);
                    if( check1 == 1)
                    {
                        KhoHang kho = new KhoHang();
                        kho.soluong = item.SoLuong;
                        kho.idsanpham = item.MaSP;
                        kho.size = item.Size;
                        db.UpdateKhoHang(kho);
                    }
                }
            }
            kh.tenkh = f.Get("txtHoTen").ToString();
            kh.diachikh = f.Get("txtDiaChi").ToString();
            kh.email = f.Get("txtEmail").ToString();
            kh.dienthoai = f.Get("txtSdt").ToString();
            int updateCustomer = db.updateCustomer(kh);
            if (updateCustomer != 1)
            {
                return RedirectToAction("ThongTinDonHang", "ShoppingCart");
            }
            Session["ShoppingCart"] = null;
            return RedirectToAction("Index","Home");
        }
        #endregion
        public ActionResult ThongTinDatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangky", "Customer");
            }
            Customer kh = (Customer)Session["TaiKhoan"];
            var l = GetShoppingCart();
            l.RemoveAll(n => n.Size == null);
            l.RemoveAll(n => n.Size == "");
            l.RemoveAll(n => n.SoLuong == 0);
            ViewBag.listSC = l;
            return View(kh);
        }
    }
}