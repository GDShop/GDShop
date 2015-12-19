using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GDWebService
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GDWebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GDWebService.svc or GDWebService.svc.cs at the Solution Explorer and start debugging.
    public class GDWebService : IGDWebService
    {
        GDDBDataContext db = null;
        public GDWebService()
        {
            db = new GDDBDataContext();
        }

        int getType(string ten)
        {
            int id = db.Types.SingleOrDefault(n => n.tenloai==ten).id;
            return id;
        }

        public List<Brand> getListBrand()
        {
            return db.Brands.ToList();
        }

        public List<Product> getListL1(string ten)
        {
            int i = getType(ten);
            var l = db.Products.Where(n => n.id == i).ToList();
            return l;
        }

        public List<Product> getListProduct()
        {
            var l =  db.Products.Where(n=>n.trangthai != "hide").Take(12).ToList();
            return l;
        }

        public Product getProduct(int id)
        {
            Product p = db.Products.SingleOrDefault(n => n.id == id);
            
            return p;
        }

        public List<Product> getListSPTheoLoai(string tenloai, int i)
        {
            List<Type> listId = db.Types.Where(n => n.tenloai.Contains(tenloai)).ToList();
            var l = (from item in listId
                    from p in db.Products
                    where item.id == p.idloai
                    select p).ToList();
            if (i == 1)
                return l;
            return l.Take(8).ToList();
        }

        public List<Product> getListSPTheoType(string tenloai)
        {
            Type sp= db.Types.SingleOrDefault(n => n.tenloai == tenloai);
            if(sp != null)
            {
                return db.Products.Where(a => a.idloai == sp.id).ToList();
            }
            return null;
        }

        public List<Product> getListSPTheoBrand(string tenloai)
        {
            int id = db.Brands.SingleOrDefault(n => n.ten == tenloai).id;
            var l = db.Products.Where(a => a.idbrand == id).ToList();
            return l;
        }

        public int countType(string ten)
        {
            int id = db.Brands.SingleOrDefault(n => n.ten == ten).id;
            int l = db.Products.Where(a => a.idbrand == id).Count();
            return l;
        }

        public int searchSize(int id, string size)
        {
            List<KhoHang> kh = db.KhoHangs.Where(n => (n.idsanpham == id)).ToList();
            KhoHang k = kh.SingleOrDefault(a => a.size == size);
            if (k!=null)
            {
                return k.soluong;
            }
            else
                return 0;
        }

        public int insertCustomer(Customer kh)
        {
            try
            {
                db.Customers.InsertOnSubmit(kh);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int checkTaiKhoan(string taikhoan)
        {
            Customer kh = db.Customers.SingleOrDefault(n => n.taikhoan == taikhoan);
            if(kh == null)
            {
                return 0;
            }
            return 1;
        }

        public Customer checkDangNhap(string taikhoan, string matkhau)
        {
            Customer kh = db.Customers.SingleOrDefault(n => n.taikhoan == taikhoan && n.matkhau == matkhau);
            if(kh == null)
            {
                return null;
            }
            return kh;
        }

        public int insertDonDatHang(DonDatHang ddh)
        {
            try
            {
                ddh.HoaDonBans.Clear();
                ddh.CTDonDatHangs.Clear();
                db.DonDatHangs.InsertOnSubmit(ddh);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int insertCTDonDatHang(CTDonDatHang ctddh)
        {
            try
            {
                db.CTDonDatHangs.InsertOnSubmit(ctddh);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int getDonDatHang(int idkh, DateTime? ngay)
        {
            DonDatHang dhh = db.DonDatHangs.FirstOrDefault(n => n.idkhachhang == idkh && n.ngaydathang == ngay);
            return dhh.id;
        }

        public int updateCustomer(Customer kh)
        {
            try
            {
                Customer c = db.Customers.Single(n => n.id == kh.id);
                c.tenkh = kh.tenkh;
                c.diachikh = kh.diachikh;
                c.dienthoai = kh.dienthoai;
                c.email = kh.email;
                c.matkhau = kh.matkhau;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<DonDatHang> getListDDHTheoID(int idkh)
        {
            List<DonDatHang> l = db.DonDatHangs.Where(n => n.idkhachhang == idkh).ToList();
            return l;
        }

        public List<CTDonDatHang> getListCTDDHTheoID(int idddh)
        {
            List<CTDonDatHang> l = db.CTDonDatHangs.Where(n => n.iddonhang == idddh).ToList();
            return l;
        }

        public List<Product> timKiemSPTheoTen(string ten)
        {
            var l = db.Products.Where(n => n.tensp.Contains(ten)).ToList();
            return l.Where(a => a.trangthai != "hide").ToList();
        }

        public DonDatHang getDDHTheoID(int id)
        {
            DonDatHang d = db.DonDatHangs.SingleOrDefault(n => n.id == id);
            return d;
        }

        public Customer getKHTheoID(int id)
        {
            return db.Customers.SingleOrDefault(n => n.id == id);
        }

        public TTHoaDon getTTHoaDonTheoID(int id)
        {
            return db.TTHoaDons.SingleOrDefault(n => n.id == id);
        }

        public ShipCar timXeChuaIDDDH(int id)
        {
            HoaDonBan hhd = db.HoaDonBans.SingleOrDefault(n => n.iddondathang == id);
            
            if(hhd != null)
            {
                CTShipCar xe = db.CTShipCars.SingleOrDefault(n => n.idhd == hhd.id);
                if(xe != null)
                    return db.ShipCars.SingleOrDefault(n => n.id == xe.idshipcar);
            }
            return null;
        }

        public NhanVien getNhanVienTheoID(int id)
        {
            return db.NhanViens.SingleOrDefault(n=>n.id == id);
        }

        public List<Product> getListAllProduct()
        {
            return db.Products.ToList();
        }

        public NhanVien loginAdmin(string taikhoan, string matkhau)
        {
            NhanVien nv = db.NhanViens.SingleOrDefault(n => n.taikhoan == taikhoan && n.matkhau == matkhau);
            return nv;
        }

        public Brand getBrandByID(int id)
        {
            return db.Brands.SingleOrDefault(n => n.id == id);
        }

        public Type getTypeByID(int id)
        {
            return db.Types.SingleOrDefault(n => n.id == id);
        }

        public Quality getQByID(int id)
        {
            return db.Qualities.SingleOrDefault(n => n.id == id);
        }

        public List<Quality> getListQ()
        {
            return db.Qualities.ToList();
        }

        public List<Type> getListT()
        {
            return db.Types.ToList();
        }

        public int InsertProduct(Product p)
        {
            try
            {
                db.Products.InsertOnSubmit(p);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }
        public int UpdateProduct(Product p)
        {
            try
            {
                Product pp = db.Products.Single(n => n.id == p.id);
                pp.tensp = p.tensp;
                pp.dongia = p.dongia;
                pp.taydai = p.taydai;
                pp.idbrand = p.idbrand;
                pp.idloai = p.idloai;
                pp.idchatlieu = p.idchatlieu;
                pp.image1 = p.image1;
                pp.image2 = p.image2;
                pp.image3 = p.image3;
                pp.trangthai = p.trangthai;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<NhanVien> getListNhanVien()
        {
            return db.NhanViens.ToList();
        }

        public List<NhaCungCap> getListNCC()
        {
            return db.NhaCungCaps.ToList();
        }

        public int SuaNCC(NhaCungCap n)
        {
            try
            {
                NhaCungCap ncc = db.NhaCungCaps.Single(a => a.id == n.id);
                ncc.ten = n.ten;
                ncc.sdt = n.sdt;
                ncc.diachi = n.diachi;
                ncc.sdt = n.sdt;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int SuaLoai(Type n)
        {
            try
            {
                Type ncc = db.Types.Single(a => a.id == n.id);
                ncc.tenloai = n.tenloai;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int SuaThuongHieu(Brand n)
        {
            try
            {
                Brand ncc = db.Brands.Single(a => a.id == n.id);
                ncc.ten = n.ten;
                ncc.sdt = n.sdt;
                ncc.diachi = n.diachi;
                ncc.sdt = n.sdt;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int SuaChatLieu(Quality n)
        {
            try
            {
                Quality ncc = db.Qualities.Single(a => a.id == n.id);
                ncc.tenchatlieu = n.tenchatlieu;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int ThemNCC(NhaCungCap n)
        {
            try
            {
                db.NhaCungCaps.InsertOnSubmit(n);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int ThemLoai(Type n)
        {
            try
            {
                db.Types.InsertOnSubmit(n);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int ThemThuongHieu(Brand n)
        {
            try
            {
                db.Brands.InsertOnSubmit(n);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int ThemChatLieu(Quality n)
        {
            try
            {
                db.Qualities.InsertOnSubmit(n);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<DonDatHang> getListDDH()
        {
            return db.DonDatHangs.ToList();
        }

        public int UpdateKhoHang(KhoHang kh)
        {
            try
            {
                KhoHang k = db.KhoHangs.SingleOrDefault(n => n.idsanpham == kh.idsanpham && n.size == kh.size);
                k.soluong = k.soluong - kh.soluong;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int InsertHoaDon(HoaDonBan hd)
        {
            try
            {
                db.HoaDonBans.InsertOnSubmit(hd);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int UpdateDDH(DonDatHang ddh)
        {
            try
            {
                DonDatHang d = db.DonDatHangs.SingleOrDefault(n => n.id == ddh.id);
                d.tinhtrang = ddh.tinhtrang;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<HoaDonBan> getListHDB()
        {
            return db.HoaDonBans.ToList();
        }

        public HoaDonBan getHDBTheoID(int id)
        {
            return db.HoaDonBans.SingleOrDefault(n => n.id == id);
        }

        public List<ShipCar> getListShipCar()
        {
            return db.ShipCars.ToList();
        }

        public int InsertCTShipCar(CTShipCar sc)
        {
            CTShipCar ct = db.CTShipCars.SingleOrDefault(n => n.idshipcar == sc.idshipcar && n.idhd == sc.idhd);
            if (ct == null)
            {
                db.CTShipCars.InsertOnSubmit(sc);
                db.SubmitChanges();
                return 1;
            }
            else
            {
                ct.idhd = sc.idhd;
                db.SubmitChanges();
                return 2;
            }
        }

        public int UpdateHoaDonBan(HoaDonBan hd)
        {
            try
            {
                HoaDonBan d = db.HoaDonBans.SingleOrDefault(n => n.id == hd.id);
                d.idtthoadon = hd.idtthoadon;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<CTShipCar> getListCTShipCar()
        {
            return db.CTShipCars.ToList();
        }

        public List<HoaDonBan> getListHodDonTheoIDNV(int id)
        {
            NhanVien nv = db.NhanViens.SingleOrDefault(n => n.id == id);
            ShipCar xe = db.ShipCars.SingleOrDefault(a => a.idnhanvien == nv.id);
            List<CTShipCar> ct = db.CTShipCars.Where(b => b.idshipcar == xe.id).ToList();
            List<HoaDonBan> listHD = new List<HoaDonBan>();
            foreach(var item in ct)
            {
                listHD.Add(db.HoaDonBans.SingleOrDefault( o => o.id == item.idhd));
            }
            return listHD;
        }

        public int UpdateShipCar(int idnhanvien,double log, double lat, string location)
        {
            try
            {
                ShipCar d = db.ShipCars.SingleOrDefault(n => n.idnhanvien == idnhanvien);
                d.latitude = lat;
                d.longitude = log;
                d.location = location;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public NhanVien LoginShipCar(string tk, string mk)
        {
            NhanVien nv = db.NhanViens.SingleOrDefault(n => n.taikhoan == tk && n.matkhau == mk);
            ShipCar xe = db.ShipCars.SingleOrDefault(a => a.idnhanvien == nv.id);
            if (xe != null)
            {
                return nv;
            }
            else
                return null;
        }

        public List<Customer> getListKH()
        {
            return db.Customers.ToList();
        }

        public List<ChucVu> getListChucVu()
        {
            return db.ChucVus.ToList();
        }

        public int updateNhanVien(NhanVien nv)
        {
            try
            {
                NhanVien a = db.NhanViens.SingleOrDefault(n => n.id == nv.id);
                a.tennv = nv.tennv;
                a.diachi = nv.diachi;
                a.sdt = nv.sdt;
                a.idchucvu = nv.idchucvu;
                a.matkhau = nv.matkhau;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int insertNhanVien(NhanVien nv)
        {
            try
            {
                db.NhanViens.InsertOnSubmit(nv);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int insertShipCar(ShipCar sc)
        {
            try
            {
                db.ShipCars.InsertOnSubmit(sc);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int updateShipCarForWeb(ShipCar sc)
        {
            try
            {
                ShipCar s = db.ShipCars.SingleOrDefault(n => n.id == sc.id);
                s.idnhanvien = sc.idnhanvien;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int InsertHoaDonNhap(HoaDonNhap hd)
        {
            try
            {
                db.HoaDonNhaps.InsertOnSubmit(hd);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int InsertCTHoaDonNhap(CTHoaDonNhap ct)
        {
            try
            {
                db.CTHoaDonNhaps.InsertOnSubmit(ct);
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public HoaDonNhap getHDN(DateTime? ngay, int idnhanvien)
        {
            try
            {
                HoaDonNhap hd = db.HoaDonNhaps.SingleOrDefault(n => n.idnhanvien == idnhanvien && n.ngaynhap == ngay);

                return hd;
            }
            catch
            {
                return null;
            }
        }

        public int NhapKho(KhoHang kh)
        {
            try
            {
                KhoHang k = db.KhoHangs.SingleOrDefault(n => n.idsanpham == kh.idsanpham && n.size == kh.size);
                if(k!= null)
                {
                    k.soluong = k.soluong + kh.soluong;
                    db.SubmitChanges();
                }
                else
                {
                    db.KhoHangs.InsertOnSubmit(kh);
                    db.SubmitChanges();
                }
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public int UpdateHoaDonAdnroid(int id)
        {
            try
            {
                HoaDonBan h = db.HoaDonBans.SingleOrDefault(n => n.id == id);
                DonDatHang d = db.DonDatHangs.SingleOrDefault(a => a.id == h.iddondathang);
                h.idtthoadon = 4;
                d.tinhtrang = 4;
                db.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public List<HoaDonNhap> getListHoaDonNhap()
        {
            return db.HoaDonNhaps.ToList();
        }

        public List<CTHoaDonNhap> getListCTHoaDonNhap()
        {
            return db.CTHoaDonNhaps.ToList();
        }

        public List<KhoHang> getListKhoHang()
        {
            return db.KhoHangs.ToList();
        }
    }
}
