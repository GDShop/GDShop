using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GDShop.GDWebservice;

namespace GDShop.Models
{
    public class ShoppingCart
    {
        GDWebServiceClient db = new GDWebServiceClient();
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string Image { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string Size { get; set; }
        public double TongTien
        {
            get { return SoLuong * DonGia; }
        }
        public ShoppingCart(int MaSP)
        {
            this.MaSP = MaSP;
            Product p = db.getProduct(this.MaSP);
            TenSP = p.tensp;
            Image = p.image1;
            SoLuong = 1;
            DonGia = p.dongia;
        }
    }
}