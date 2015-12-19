using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDShop.Models
{
    public class DonNhapHang
    {
        public int MaSP { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string Size { get; set; }
        public int TongTien { get { return SoLuong * DonGia; } }

        public DonNhapHang(int MaSP, int SoLuong, int DonGia, string Size) {
            this.MaSP = MaSP;
            this.SoLuong = SoLuong;
            this.DonGia = DonGia;
            this.Size = Size;
        }
    }
}