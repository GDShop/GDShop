using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GDShop.GDWebservice;
using PagedList.Mvc;
using PagedList;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using GDShop.Models;

namespace GDShop.Controllers
{
    
    public class BaoMatRatCaoController : Controller
    {
        GDWebServiceClient db = new GDWebServiceClient();
        // GET: BaoMatRatCao
        public ActionResult Index(int? page)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            setSession();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var l = db.getListAllProduct();
            return View(l.ToPagedList(pageNumber, pageSize));
        }
        
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string tk = f.Get("txtTKNV").ToString();
            string mk = f.Get("txtMKNV").ToString();
            mk = DoiSangMD5(mk);
            NhanVien nv = db.loginAdmin(tk, mk);
            if (nv == null)
            {
                ViewBag.thongbao = "Tài khoản hoặc mật khẩu không đúng";
                return View();
            }
            else
            {
                Session["NhanVien"] = nv;
                Session["CVNhanVien"] = nv.idchucvu;
                ViewBag.thongbao = "Đăng Nhập Thành Công";
                return RedirectToAction("Index","BaoMatRatCao");
            }
            
        }
        public ActionResult Logout()
        {
            Session["NhanVien"] =null;
            return RedirectToAction("Login", "BaoMatRatCao");
        }
        public static String DoiSangMD5(string mk)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(mk);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }
        public ActionResult Restart()
        {
            setSession();
            return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "", diachi = "", sdt = "", i = 0, id = 0 });
        }
        public void setSession()
        {
            if(Session["NhanVien"] != null)
            {
                Session["Type"] = db.getListT();
                Session["Brand"] = db.getListBrand();
                Session["Quality"] = db.getListQ();
                Session["NhaCungCap"] = db.getListNCC();
            }
        }

        #region Sản Phẩm
        public ActionResult CTSanPham(int id, string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            ViewBag.thongbao = thongbao;
            Product l = db.getProduct(id);
            ViewBag.getB = Session["Brand"];
            ViewBag.getQ = Session["Quality"];
            ViewBag.getT = Session["Type"];
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
        [HttpPost]
        public ActionResult UpdateCTSP(FormCollection f, int id, HttpPostedFileBase file, HttpPostedFileBase file2, HttpPostedFileBase file3)
        {
            
            string tensp = f.Get("txtTenSP").ToString();
            string dongia = f.Get("txtDonGia").ToString();
            int taydai = 0;
            if (f["taydai"] == "on")
                taydai = 1;
            int idloai = int.Parse(f["slLoai"]);
            int idchatlieu = int.Parse(f["slCL"]);
            int idbrand = int.Parse(f["slTH"]);
            string tinhtrang = f.Get("slTT").ToString();
            
            string image1 = UploadImage(file);
            string image2 = UploadImage(file2);
            string image3 = UploadImage(file3);
            if (image1 == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 1 bị trùng hoặc đã có!";
                return RedirectToAction("CTSanPham", "BaoMatRatCao", new { id = id, thongbao = "Ten hinh 1 bi trung" });
            }
            if (image2 == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 2 bị trùng hoặc đã có!";
                return RedirectToAction("CTSanPham", "BaoMatRatCao", new { id = id, thongbao = "Ten hinh 2 bi trung" });
            }
            if (image3 == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 3 bị trùng hoặc đã có!";
                return RedirectToAction("CTSanPham", "BaoMatRatCao", new { id = id, thongbao = "Ten hinh 3 bi trung" });
            }

            Product p = db.getProduct(id);
            if (image1 != null)
                p.image1 = image1;
            if (image2 != null)
                p.image1 = image2;
            if (image3 != null)
                p.image1 = image3;
            p.tensp = tensp;
            p.dongia = int.Parse(dongia);
            p.taydai = taydai;
            p.idloai = idloai;
            p.idchatlieu = idchatlieu;
            p.idbrand = idbrand;
            p.trangthai = tinhtrang;
            int check = db.UpdateProduct(p);
            if(check == 1)
            {
                return RedirectToAction("CTSanPham","BaoMatRatCao", new { id = id, thongbao = "Update success" });
            }
            return RedirectToAction("CTSanPham", "BaoMatRatCao", new { id = id, thongbao = "Fail..!" });
        }
        [HttpGet]
        public ActionResult InsertSP(string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }

            ViewBag.thongbao = thongbao;
            ViewBag.getB = Session["Brand"];
            ViewBag.getQ = Session["Quality"];
            ViewBag.getT = Session["Type"];
            return View();
        }
        [HttpPost]
        public ActionResult InsertSP(FormCollection f, HttpPostedFileBase file, HttpPostedFileBase file2, HttpPostedFileBase file3)
        {
            ViewBag.getB = Session["Brand"];
            ViewBag.getQ = Session["Quality"];
            ViewBag.getT = Session["Type"];
            string tensp = f.Get("txtTenSP").ToString();
            string dongia = f.Get("txtDonGia").ToString();
            int taydai = 0;
            if (f["taydai"] == "on")
                taydai = 1;
            int idloai = int.Parse(f["slLoai"]);
            int idchatlieu = int.Parse(f["slCL"]);
            int idbrand = int.Parse(f["slTH"]);
            string tinhtrang = f.Get("slTT").ToString();
            if (CheckUploadImage(file) == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 1 bị trùng hoặc đã có!";
                return View();
            }
            if (CheckUploadImage(file2) == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 2 bị trùng hoặc đã có!";
                return View();
            }
            if (CheckUploadImage(file3) == "trung")
            {
                ViewBag.thongbao1 = "Tên hình ảnh 3 bị trùng hoặc đã có!";
                return View();
            }
            string image1 = UploadImage(file);
            string image2 = UploadImage(file2);
            string image3 = UploadImage(file3);
            

            Product p = new Product();
            p.tensp = tensp;
            p.dongia = int.Parse(dongia);
            p.taydai = taydai;
            p.idloai = idloai;
            p.image1 = image1;
            p.image2 = image2;
            p.image3 = image3;
            p.idchatlieu = idchatlieu;
            p.idbrand = idbrand;
            p.trangthai = tinhtrang;
            int check = db.InsertProduct(p);
            if (check == 1)
            {
                return RedirectToAction("InsertSP", "BaoMatRatCao", new {thongbao = "Insert success" });
            }
            return RedirectToAction("InsertSP", "BaoMatRatCao", new {thongbao = "Fail..!" });
        }
        public String CheckUploadImage(HttpPostedFileBase file)
        {
            if (file == null)
                return null;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath("~/Images/Product"), fileName);
            if (System.IO.File.Exists(path))
            {

                return "trung";
            }
            return fileName;
        }
        [HttpPost]
        public String UploadImage(HttpPostedFileBase file)
        {
            if (file == null)
                return null;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath("~/Images/Product"), fileName);
            file.SaveAs(path);
            return fileName;
        }
        #endregion

        #region Thông Tin Sản Phẩm

        public ActionResult ThongTin(string ten, string diachi, string sdt, int i, int id)
        {
            if (i == 1)
            {
                ViewBag.idloai = id;
                ViewBag.tenloai = ten;
            }
            if (i == 2)
            {
                ViewBag.idchatlieu = id;
                ViewBag.tenchatlieu = ten;
            }
            if (i == 3)
            {
                ViewBag.idth = id;
                ViewBag.tenth = ten;
                ViewBag.dcth = diachi;
                ViewBag.sdtth = sdt;
            }
            if (i == 4)
            {
                ViewBag.idncc = id;
                ViewBag.tenncc = ten;
                ViewBag.dcncc = diachi;
                ViewBag.sdtncc = sdt;
            }
            if (ten == "sualoai")
                ViewBag.thongbaosualoai = diachi;
            if (ten == "themloai")
                ViewBag.thongbaothemloai = diachi;
            if (ten == "suachatlieu")
                ViewBag.thongbaosuacl = diachi;
            if (ten == "themchatlieu")
                ViewBag.thongbaothemcl = diachi;
            if (ten == "suath")
                ViewBag.thongbaosuath = diachi;
            if (ten == "themth")
                ViewBag.thongbaothemth = diachi;
            if (ten == "suancc")
                ViewBag.thongbaosuancc = diachi;
            if (ten == "themncc")
                ViewBag.thongbaothemncc = diachi;
            ViewBag.Q = Session["Quality"];
            ViewBag.T = Session["Type"];
            ViewBag.B = Session["Brand"];
            ViewBag.N = Session["NhaCungCap"];
            return View();
        }
        
        public ActionResult SuaLoai1(int id)
        {
            var T = db.getListT();
            String a = T.Single(n => n.id == id).tenloai;
            return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = a,diachi="",sdt = "",i =1, id = id});
        }
        [HttpPost]
        public ActionResult SuaLoai(int id,FormCollection f)
        {
            var T = db.getListT();
            GDShop.GDWebservice.Type p = T.Single(n => n.id == id);
            string ten = f.Get("txtTen").ToString();
            p.tenloai = ten;
            int i = db.SuaLoai(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "sualoai", diachi = "Sửa thành công", sdt = "",i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "sualoai", diachi = "Sửa Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        [HttpPost]
        public ActionResult ThemLoai( FormCollection f)
        {
            GDShop.GDWebservice.Type p = new GDWebservice.Type();
            string ten = f.Get("txtTen").ToString();
            p.tenloai = ten;
            int i = db.ThemLoai(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themloai", diachi = "Thêm thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themloai", diachi = "Thêm Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        public ActionResult SuaChatLieu1(int id)
        {
            var T = db.getListQ();
            String a = T.Single(n => n.id == id).tenchatlieu;
            return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = a, diachi = "", sdt = "", i = 2, id = id });
        }
        [HttpPost]
        public ActionResult SuaChatLieu(int id, FormCollection f)
        {
            var T = db.getListQ();
            Quality p = T.Single(n => n.id == id);
            string ten = f.Get("txtTen").ToString();
            p.tenchatlieu = ten;
            int i = db.SuaChatLieu(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suachatlieu", diachi = "Sửa thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suachatlieu", diachi = "Sửa Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        [HttpPost]
        public ActionResult ThemChatLieu(FormCollection f)
        {
            Quality p = new Quality();
            string ten = f.Get("txtTen").ToString();
            p.tenchatlieu = ten;
            int i = db.ThemChatLieu(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themchatlieu", diachi = "Thêm thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themchatlieu", diachi = "Thêm Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        public ActionResult SuaThuongHieu1(int id)
        {
            var T = db.getListBrand();
            Brand bb = T.Single(n => n.id == id);
            string a = bb.ten;
            string b = bb.diachi;
            string c = bb.sdt;
            return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = a, diachi = b, sdt =c , i = 3, id = id });
        }
        [HttpPost]
        public ActionResult SuaThuongHieu(int id, FormCollection f)
        {
            var T = db.getListBrand();
            Brand p = T.Single(n => n.id == id);
            string ten = f.Get("txtTen").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            p.ten = ten;
            p.diachi = diachi;
            p.sdt = sdt;
            int i = db.SuaThuongHieu(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suath", diachi = "Sửa thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suath", diachi = "Sửa Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        [HttpPost]
        public ActionResult ThemThuongHieu(FormCollection f)
        {
            Brand p = new Brand();
            string ten = f.Get("txtTen").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            p.ten = ten;
            p.diachi = diachi;
            p.sdt = sdt;
            int i = db.ThemThuongHieu(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themth", diachi = "Thêm thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themth", diachi = "Thêm Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        public ActionResult SuaNhaCungCap1(int id)
        {
            var T = db.getListNCC();
            NhaCungCap bb = T.Single(n => n.id == id);
            string a = bb.ten;
            string b = bb.diachi;
            string c = bb.sdt;
            return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = a, diachi = b, sdt = c, i = 4, id = id });
        }
        [HttpPost]
        public ActionResult SuaNhaCungCap(int id, FormCollection f)
        {
            var T = db.getListNCC();
            NhaCungCap p = T.Single(n => n.id == id);
            string ten = f.Get("txtTen").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            p.ten = ten;
            p.diachi = diachi;
            p.sdt = sdt;
            int i = db.SuaNCC(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suancc", diachi = "Sửa thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "suancc", diachi = "Sửa Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }

        [HttpPost]
        public ActionResult ThemNhaCungCap(FormCollection f)
        {
            NhaCungCap p = new NhaCungCap();
            string ten = f.Get("txtTen").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            p.ten = ten;
            p.diachi = diachi;
            p.sdt = sdt;
            int i = db.ThemNCC(p);
            if (i == 1)
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themncc", diachi = "Thêm thành công", sdt = "", i = 0, id = 0 });
            }
            else
            {
                return RedirectToAction("ThongTin", "BaoMatRatCao", new { ten = "themncc", diachi = "Thêm Thất Bại", sdt = "", i = 0, id = 0 });
            }
        }
        #endregion

        #region Đơn Đặt Hàng

        public ActionResult DonDatHang(int? page, string thongbao)
        {
            ViewBag.thongbao = thongbao;
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            setSession();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var l = db.getListDDH();
            return View(l.Where(n => n.tinhtrang == 1).OrderBy(n => n.ngaydathang).ToPagedList(pageNumber, pageSize));
        }
        public String LayTenKhachHang(int id)
        {
            return db.getKHTheoID(id).tenkh;
        }
        [HttpGet]
        public ActionResult XuLyDDH(int id)
        {
            ViewBag.ddh = db.getDDHTheoID(id);
            ViewBag.kh = db.getKHTheoID(ViewBag.ddh.idkhachhang);
            ViewBag.tinhtrang = db.getTTHoaDonTheoID(ViewBag.ddh.tinhtrang);
            List<Product> ll = new List<Product>();
            ViewBag.l = db.getListCTDDHTheoID(id);
            foreach (var item in ViewBag.l)
            {
                Product p = db.getProduct(item.idsanpham);
                ll.Add(p);
            }
            ViewBag.listP = ll;
            return View();
        }
        
        public ActionResult XuatHoaDon(int id)
        {
            HoaDonBan h = new HoaDonBan();
            NhanVien nv = (NhanVien)Session["NhanVien"];
            h.idnhanvien = nv.id;
            h.iddondathang = id;
            h.ngayxuat = DateTime.Now;
            h.idtthoadon = 2;
            int i = db.InsertHoaDon(h);
            if (i == 1)
            {
                DonDatHang d = db.getDDHTheoID(id);
                d.tinhtrang = 2;
                db.UpdateDDH(d);
                return RedirectToAction("DonDatHang", "BaoMatRatCao", new { thongbao = "Xuất hóa đơn thành công" });
            }
            else
                return RedirectToAction("DonDatHang","BaoMatRatCao", new { thongbao = "Xuất hóa đơn thất bại"});
        }

        #endregion

        #region Hóa Đơn

        public ActionResult HoaDonBan(int? page)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            setSession();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var l = db.getListHDB();
            ViewBag.listDaoGiao = l.Where(n => n.idtthoadon == 4).OrderBy(n => n.ngayxuat).ToPagedList(pageNumber, pageSize);
            return View(l.Where(n => n.idtthoadon == 2).OrderBy(n => n.ngayxuat).ToPagedList(pageNumber, pageSize));
        }
        public String TimTenKHTheoIDDDH(int id)
        {
            DonDatHang d = db.getDDHTheoID(id);
            Customer kh = db.getKHTheoID(d.idkhachhang);
            return kh.tenkh;
        }
        public String TimTTHDTheoID(int id)
        {
            return db.getTTHoaDonTheoID(id).tentt;
        }
        public int TimTTTheoIDDDH(int id)
        {
            return db.getDDHTheoID(id).tongtien;
        }
        public String TimTenNVTheoID(int id)
        {
            NhanVien nv = db.getNhanVienTheoID(id);
            return nv.tennv;
        }

        public ActionResult XuLyHoaDon(int id)
        {
            var car = db.getListShipCar();
            ViewBag.idhoadon = id;
            return View(car);
        }
        [HttpPost]
        public ActionResult ChuyenHoaDon(int id, FormCollection f)
        {
            int idxe = int.Parse(f["ChonXe"]);
            CTShipCar ct = new CTShipCar();
            ct.idhd = id;
            ct.idshipcar = idxe;
            int i= db.InsertCTShipCar(ct);
            if (i == 1)
            {
                HoaDonBan hd = db.getHDBTheoID(id);
                hd.idtthoadon = 3;
                int ii = db.UpdateHoaDonBan(hd);
                if (ii == 1)
                {
                    DonDatHang ddh = db.getDDHTheoID(hd.iddondathang);
                    ddh.tinhtrang = 3;
                    db.UpdateDDH(ddh);
                }
            }
            if (i == 2)
                ViewBag.thongbaoxe = "Sửa Thành Công";
            return RedirectToAction("HoaDonBan","BaoMatRatCao");
        }
        #endregion

        #region Giao Hàng

        public ActionResult GiaoHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 15;
            var l = db.getListShipCar();
            return View(l.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ChiTietShipCar(int? page, int id, string thongbao)
        {
            ViewBag.thongbao = thongbao;
            ViewBag.idxe = id;
            int pageNumber = (page ?? 1);
            int pageSize = 15;
            var l = db.getListCTShipCar();
            var xe = db.getListShipCar();
            ViewBag.ttxe = xe.Single(q => q.id == id);
            ViewBag.nhanvien = db.getListNhanVien().Where(o => o.idchucvu == 3).ToList();
            var ll = l.Where(n => n.idshipcar == id).ToList();
            var listhd = db.getListHDB();
            List<HoaDonBan> listHD = new List<GDWebservice.HoaDonBan>();
            foreach(var item in ll)
            {
                listHD.Add(listhd.SingleOrDefault(n => n.id == item.idhd));
            }
            ViewBag.hddagiao = listHD.Where(n => n.idtthoadon == 4).ToPagedList(pageNumber, pageSize);
            return View(listHD.Where(n => n.idtthoadon == 3).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult InsertShipCar(string thongbao)
        {
            ViewBag.getT = db.getListNhanVien().Where(n => n.idchucvu == 3).ToList();
            ViewBag.thongbao = thongbao;
            return View(); ;
        }
        [HttpPost]
        public ActionResult InsertShipCar(FormCollection f)
        {
            ViewBag.getT = db.getListNhanVien().Where(n => n.idchucvu == 3).ToList();
            int bxs = int.Parse(f.Get("txtBSX").ToString());
            int idnhanvien = int.Parse(f["slLoai"].ToString());
            var listShipCar = db.getListShipCar();
            ShipCar checksc = listShipCar.SingleOrDefault(n => n.idnhanvien == idnhanvien);
            if(checksc == null)
            {
                ShipCar p = new ShipCar();
                p.biensoxe = bxs;
                p.idnhanvien = idnhanvien;
                int check = db.insertShipCar(p);
                if (check == 1)
                {
                    return RedirectToAction("InsertShipCar", "BaoMatRatCao", new { thongbao = "Insert success" });
                }
                return RedirectToAction("InsertShipCar", "BaoMatRatCao", new { thongbao = "Fail..!" });
            }
            else
                return RedirectToAction("InsertShipCar", "BaoMatRatCao", new { thongbao = "Xe da co chu" });
        }
        [HttpPost]
        public ActionResult UpdateShipCar(int id, FormCollection f)
        {
            ShipCar sc = db.getListShipCar().SingleOrDefault(n => n.id == id);
            int idnhanvien = int.Parse(f["slLoai"].ToString());
            if(sc != null)
            {
                sc.idnhanvien = idnhanvien;
                int check = db.updateShipCarForWeb(sc);
                if (check == 1)
                {
                    return RedirectToAction("ChiTietShipCar", "BaoMatRatCao", new { id = id, thongbao = "Insert success" });
                }
                return RedirectToAction("ChiTietShipCar", "BaoMatRatCao", new { id = id, thongbao = "Fail..!" });
            }
            else
                return RedirectToAction("ChiTietShipCar", "BaoMatRatCao", new { id = id, thongbao = "Fail..!" });
        }
        public String TenNhanVien(int id)
        {
            return db.getNhanVienTheoID(id).tennv;
        }
        #endregion

        #region Quản Lý Khách Hàng

        public ActionResult KhachHang(int? page)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            var listkh = db.getListKH();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(listkh.ToPagedList(pageNumber,pageSize));
        }
        public ActionResult CTKhachHang(int id,string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            ViewBag.thongbao = thongbao;
            Customer l = db.getKHTheoID(id);
            if (l == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(l);
        }
        [HttpPost]
        public ActionResult UpdateCTKH(FormCollection f, int id)
        {
            string tenkh = f.Get("txtTenKH").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            string email = f.Get("txtEmail").ToString();
            string matkhau = f.Get("txtMatKhau").ToString();

            Customer p = db.getKHTheoID(id);
            
            p.tenkh = tenkh;
            p.diachikh = diachi;
            p.dienthoai = sdt;
            p.email = email;
            if (matkhau != "" && matkhau != null)
            {
                p.matkhau = DoiSangMD5(matkhau);
            }
            int check = db.updateCustomer(p);
            if (check == 1)
            {
                return RedirectToAction("CTKhachHang", "BaoMatRatCao", new { id = id, thongbao = "Update success" });
            }
            return RedirectToAction("CTKhachHang", "BaoMatRatCao", new { id = id, thongbao = "Fail..!" });
        }


        #endregion

        #region Quản Lý Nhân Viên

        public ActionResult NhanVien(int? page)
        {
            
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            if(nv.idchucvu != 1 && nv.idchucvu != 5)
            {
                return RedirectToAction("Index", "BaoMatratCao");
            }
            var listkh = db.getListNhanVien();
            var lll = db.getListChucVu();
            //ViewBag.tenchucvu = lll.Single(n => n.id == listkh.idchucvu).tenchucvu;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(listkh.ToPagedList(pageNumber, pageSize));
        }
        public String ChucVuNhanVien(int id)
        {
            var lll = db.getListChucVu();
            return lll.Single(n => n.id == id).tenchucvu;
        }
        public ActionResult CTNhanVien(int id, string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            if (nv.idchucvu != 1 && nv.idchucvu != 5)
            {
                return RedirectToAction("Index", "BaoMatratCao");
            }
            ViewBag.thongbao = thongbao;
            var lll = db.getListChucVu();
            
            ViewBag.getT = lll;
            NhanVien l = db.getNhanVienTheoID(id);

            ViewBag.tenchucvu = lll.Single(n => n.id == l.idchucvu).tenchucvu;
            ViewBag.t = lll.Single(a => a.id == l.idchucvu);
            if (l == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(l);
        }
        [HttpPost]
        public ActionResult UpdateCTNV(FormCollection f, int id)
        {
            string tenkh = f.Get("txtTenKH").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            int email = int.Parse(f["slLoai"].ToString());
            string matkhau = f.Get("txtMatKhau").ToString();

            NhanVien p = db.getNhanVienTheoID(id);

            p.tennv = tenkh;
            p.diachi = diachi;
            p.sdt = sdt;
            p.idchucvu = email;
            if (matkhau != "" && matkhau != null)
            {
                p.matkhau = DoiSangMD5(matkhau);
            }
            int check = db.updateNhanVien(p);
            if (check == 1)
            {
                return RedirectToAction("CTNhanVien", "BaoMatRatCao", new { id = id, thongbao = "Update success" });
            }
            return RedirectToAction("CTNhanVien", "BaoMatRatCao", new { id = id, thongbao = "Fail..!" });
        }
        [HttpGet]
        public ActionResult InsertNhanVien(string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            if (nv.idchucvu != 1 && nv.idchucvu != 5)
            {
                return RedirectToAction("Index", "BaoMatratCao");
            }
            ViewBag.thongbao = thongbao;
            var lll = db.getListChucVu();
            ViewBag.getT = lll;
            return View();
        }
        [HttpPost]
        public ActionResult InsertNhanVien(FormCollection f)
        {
            ViewBag.getT = db.getListChucVu();
            string tennv = f.Get("txtTenNV").ToString();
            string diachi = f.Get("txtDiaChi").ToString();
            string sdt = f.Get("txtSdt").ToString();
            string tk = f.Get("txtTaiKhoan").ToString();
            string matkhau = f.Get("txtMatKhau").ToString();
            int idchucvu = int.Parse(f["slLoai"].ToString());

            NhanVien p = new NhanVien();
            p.tennv = tennv;
            p.diachi = diachi;
            p.sdt = sdt;
            p.taikhoan = tk;
            p.matkhau = DoiSangMD5(matkhau);
            p.idchucvu = idchucvu;
            int check = db.insertNhanVien(p);
            if (check == 1)
            {
                return RedirectToAction("InsertNhanVien", "BaoMatRatCao", new { thongbao = "Insert success" });
            }
            return RedirectToAction("InsertNhanVien", "BaoMatRatCao", new { thongbao = "Fail..!" });
        }
        #endregion

        #region Hóa Đơn Nhập Hàng

        public ActionResult HoaDonNhap(int? page)
        {

            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            setSession();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var l = db.getListHoaDonNhap();
            return View(l.OrderBy(n=>n.ngaynhap).ToPagedList(pageNumber, pageSize));
        }

        public List<DonNhapHang> GetDonNhapHang() //Lấy don dat hang
        {
            List<DonNhapHang> listSC = Session["DonNhapHang"] as List<DonNhapHang>; //ep kieu session, neu chu ton tai = null
            if (listSC == null)
            {
                listSC = new List<DonNhapHang>();
                Session["DonNhapHang"] = listSC;
            }
            return listSC;
        }
        public ActionResult AddDonNhapHang(string Url) //Thêm giỏ hàng
        {
            List<DonNhapHang> listSC = GetDonNhapHang();
            listSC.Add(new DonNhapHang(0,0,0,null));
            
            return Redirect(Url);
        }
        public ActionResult UpdateDonNhapHang(FormCollection f,int MaSP)
        {
            List<DonNhapHang> listSC = GetDonNhapHang();
            try
            {
                int MaSP1 = int.Parse(f["idp"].ToString());
                int SoLuong = int.Parse(f["soluongp"].ToString());
                int DonGia = int.Parse(f["dongiap"].ToString());
                string Size = f["sizep"].ToString();
                DonNhapHang sc = listSC.Find(n => n.MaSP == MaSP);
                if (sc != null)
                {
                    sc.MaSP = MaSP1;
                    sc.DonGia = DonGia;
                    sc.SoLuong = SoLuong;
                    sc.Size = Size;
                }
                return RedirectToAction("DonNhapHang");
            }
            catch
            {
                return RedirectToAction("DonNhapHang");
            }
        }
        public ActionResult DeletecDonNhapHang(int MaSP)
        {
            Product p = db.getProduct(MaSP);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<DonNhapHang> listSC = GetDonNhapHang();
            DonNhapHang sc = listSC.Find(n => n.MaSP == MaSP);
            if (sc != null)
            {
                listSC.RemoveAll(n => n.MaSP == MaSP);
            }
            
            return RedirectToAction("DonNhapHang");
        }
        public ActionResult DonNhapHang()
        {
            ViewBag.NCC = db.getListNCC();
            ViewBag.listProduct = db.getListAllProduct();
            List<DonNhapHang> listSC = GetDonNhapHang();
            return View(listSC);
        }
        public int TongTienHoaDonNhap()
        {
            int a = 0;
            List<DonNhapHang> listSC = GetDonNhapHang();
            foreach(var item in listSC)
            {
                a = a + item.TongTien;
            }
            return a;
        }
        public ActionResult InsertCTDNH(FormCollection f)
        {
            List<DonNhapHang> listSC = GetDonNhapHang();
            NhanVien nv = (NhanVien)Session["NhanVien"];
            int idncc = int.Parse(f["ncc"].ToString());

            GDWebservice.HoaDonNhap hd = new GDWebservice.HoaDonNhap();
            hd.idnhacungcap = idncc;
            hd.ngaynhap = DateTime.Now;
            hd.idnhanvien = nv.id;
            hd.tongtien = TongTienHoaDonNhap();
            int insertHDN = db.InsertHoaDonNhap(hd);
            int idhdn = db.getHDN(hd.ngaynhap, hd.idnhanvien).id;
            if(insertHDN == 1)
            {
                foreach(var item in listSC)
                {
                    CTHoaDonNhap ct = new CTHoaDonNhap();
                    ct.idhoadonnhap = idhdn;
                    ct.idsanpham = item.MaSP;
                    ct.size = item.Size;
                    ct.soluong = item.SoLuong;
                    ct.dongia = item.DonGia;
                    int check = db.InsertCTHoaDonNhap(ct);
                    if(check == 1)
                    {
                        KhoHang kho = new KhoHang();
                        kho.soluong = item.SoLuong;
                        kho.idsanpham = item.MaSP;
                        kho.size = item.Size;
                        db.NhapKho(kho);
                    }
                }
            }
            Session["DonNhapHang"] = null;
            return RedirectToAction("DonNhapHang", "BaoMatRatCao");
        }

        public String TimTenNCCTheoID(int id)
        {
            var listNCC = db.getListNCC();
            return listNCC.SingleOrDefault(n => n.id == id).ten;
        }
        public ActionResult CTHoaDonNhap(int id,string thongbao)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            ViewBag.thongbao = thongbao;
            ViewBag.listNCC = Session["NhaCungCap"];
            var listHDN = db.getListHoaDonNhap();
            HoaDonNhap hdn = listHDN.SingleOrDefault(n => n.id == id);
            var ct = db.getListCTHoaDonNhap();
            ViewBag.CTHD = ct.Where(a => a.idhoadonnhap == id).ToList();
            if (hdn == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hdn);
        }
        #endregion

        #region Kho hàng

        public ActionResult KhoHang(int? page)
        {
            NhanVien nv = (NhanVien)Session["NhanVien"];
            if (nv == null)
            {
                return RedirectToAction("Login", "BaoMatratCao");
            }
            setSession();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var l = db.getListKhoHang();
            return View(l.ToPagedList(pageNumber, pageSize));
        }
        public String TimTenSPTheoID(int id)
        {
            return db.getProduct(id).tensp;
        }
        #endregion

        #region Tìm Kiếm Sản Phẩm

        [HttpPost]
        public ActionResult TimKiem(FormCollection f, int? page)
        {
            string tk = f.Get("txtTimKiem").ToString();
            ViewBag.tukhoa = tk;
            var l = db.timKiemSPTheoTen(tk);
            int pageNumber = (page ?? 1);
            int pageSize = 12;
            if (l.Count() == 0)
            {
                ViewBag.thongbao = "Không tìm thấy sản phẩm nào";
                var ll = new List<Product>();
                return View(ll.ToPagedList(pageNumber, pageSize));
            }
            ViewBag.thongbao = "Đã tìm thấy " + l.Count() + " sản phẩm";
            return View(l.OrderBy(n => n.tensp).ToPagedList(pageNumber, pageSize));
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

        #endregion

        #region Thống Kê
        [HttpGet]
        public ActionResult ThongKe()
        {
            var hda = new List<HoaDonBan>();
            ViewBag.thongke = 0;
            return View(hda);
        }
        [HttpPost]
        public ActionResult ThongKe(FormCollection f)
        {
            
            int thongke = 0;
            int thang = int.Parse(f["slThang"].ToString());
            int nam = int.Parse(f["slNam"].ToString());
            ViewBag.thang = thang;
            ViewBag.nam = nam;
            var hd = db.getListHDB();
            if(thang != 0 && nam != 0)
            {
                var hda = new List<HoaDonBan>();
                hda = hd.Where(n => n.ngayxuat.Month == thang && n.ngayxuat.Year == nam && n.idtthoadon == 4).ToList();
                foreach (var item in hda)
                {
                    thongke = thongke + TongTienTheoIDHoaDon(item.iddondathang);
                }
                ViewBag.thongke = thongke;
                return View(hda);
            }
            else if (nam != 0)
            {
                var hda = new List<HoaDonBan>();
                hda = hd.Where(n => n.ngayxuat.Year == nam && n.idtthoadon == 4).ToList();
                foreach (var item in hda)
                {
                    thongke = thongke + TongTienTheoIDHoaDon(item.iddondathang);
                }
                ViewBag.thongke = thongke;
                return View(hda);
            }
            return View();
        }
        public int TongTienTheoIDHoaDon(int id)
        {
            var ddh = db.getDDHTheoID(id);
            return ddh.tongtien;
        }
        #endregion
    }
}