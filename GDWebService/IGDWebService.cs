using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GDWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGDWebService" in both code and config file together.
    [ServiceContract(Namespace = "http://gdshop.com/")]
    public interface IGDWebService
    {
        [OperationContract]
        List<Product> getListProduct();

        [OperationContract]
        Product getProduct(int id);

        [OperationContract]
        List<Brand> getListBrand();

        [OperationContract]
        Brand getBrandByID(int id);

        [OperationContract]
        Type getTypeByID(int id);

        [OperationContract]
        Quality getQByID(int id);

        [OperationContract]
        List<Quality> getListQ();

        [OperationContract]
        List<Type> getListT();

        [OperationContract]
        List<Product> getListL1(string ten);

        [OperationContract]
        List<Product> getListSPTheoLoai(string tenloai, int i);

        [OperationContract]
        List<Product> getListSPTheoType(string tenloai);

        [OperationContract]
        List<Product> getListSPTheoBrand(string tenloai);

        [OperationContract]
        int countType(string ten);

        [OperationContract]
        int searchSize(int id, string size);

        [OperationContract]
        int insertCustomer(Customer kh);

        [OperationContract]
        int checkTaiKhoan(string taikhoan);

        [OperationContract]
        Customer checkDangNhap(string taikhoan, string matkhau);

        [OperationContract]
        int insertDonDatHang(DonDatHang ddh);

        [OperationContract]
        int getDonDatHang(int idkh, DateTime? ngay);

        [OperationContract]
        int insertCTDonDatHang(CTDonDatHang ctddh);

        [OperationContract]
        int updateCustomer(Customer kh);

        [OperationContract]
        List<DonDatHang> getListDDHTheoID (int idkh);

        [OperationContract]
        List<CTDonDatHang> getListCTDDHTheoID(int idddh);

        [OperationContract]
        List<Product> timKiemSPTheoTen(string ten);

        [OperationContract]
        DonDatHang getDDHTheoID(int id);

        [OperationContract]
        Customer getKHTheoID(int id);

        [OperationContract]
        TTHoaDon getTTHoaDonTheoID(int id);

        [OperationContract]
        ShipCar timXeChuaIDDDH(int id);

        [OperationContract]
        NhanVien getNhanVienTheoID(int id);

        [OperationContract]
        List<Product> getListAllProduct();

        [OperationContract]
        NhanVien loginAdmin(string taikhoan, string matkhau);

        [OperationContract]
        int InsertProduct(Product p);

        [OperationContract]
        int UpdateProduct(Product p);

        [OperationContract]
        List<NhanVien> getListNhanVien();

        [OperationContract]
        List<NhaCungCap> getListNCC();

        [OperationContract]
        int SuaNCC(NhaCungCap n);

        [OperationContract]
        int SuaLoai(Type n);

        [OperationContract]
        int SuaThuongHieu(Brand n);

        [OperationContract]
        int SuaChatLieu(Quality n);

        [OperationContract]
        int ThemNCC(NhaCungCap n);

        [OperationContract]
        int ThemLoai(Type n);

        [OperationContract]
        int ThemThuongHieu(Brand n);

        [OperationContract]
        int ThemChatLieu(Quality n);

        [OperationContract]
        List<DonDatHang> getListDDH();

        [OperationContract]
        int UpdateKhoHang(KhoHang kh);

        [OperationContract]
        int InsertHoaDon(HoaDonBan hd);

        [OperationContract]
        int UpdateDDH(DonDatHang ddh);

        [OperationContract]
        List<HoaDonBan> getListHDB();

        [OperationContract]
        HoaDonBan getHDBTheoID(int id);

        [OperationContract]
        List<ShipCar> getListShipCar();

        [OperationContract]
        int InsertCTShipCar(CTShipCar sc);

        [OperationContract]
        int UpdateHoaDonBan(HoaDonBan hd);

        [OperationContract]
        List<CTShipCar> getListCTShipCar();

        [OperationContract]
        List<HoaDonBan> getListHodDonTheoIDNV(int id);

        [OperationContract]
        int UpdateShipCar(int idnhanvien, double log, double lat, string location);

        [OperationContract]
        NhanVien LoginShipCar(string  tk, string mk);

        [OperationContract]
        List<Customer> getListKH();

        [OperationContract]
        List<ChucVu> getListChucVu();

        [OperationContract]
        int updateNhanVien(NhanVien nv);

        [OperationContract]
        int insertNhanVien(NhanVien nv);

        [OperationContract]
        int insertShipCar(ShipCar sc);

        [OperationContract]
        int updateShipCarForWeb(ShipCar sc);

        [OperationContract]
        int InsertHoaDonNhap(HoaDonNhap hd);

        [OperationContract]
        int InsertCTHoaDonNhap(CTHoaDonNhap ct);

        [OperationContract]
        HoaDonNhap getHDN(DateTime? ngay, int idnhanvien);

        [OperationContract]
        int NhapKho(KhoHang kh);

        [OperationContract]
        int UpdateHoaDonAdnroid(int id);

        [OperationContract]
        List<HoaDonNhap> getListHoaDonNhap();

        [OperationContract]
        List<CTHoaDonNhap> getListCTHoaDonNhap();

        [OperationContract]
        List<KhoHang> getListKhoHang();

    }
}
