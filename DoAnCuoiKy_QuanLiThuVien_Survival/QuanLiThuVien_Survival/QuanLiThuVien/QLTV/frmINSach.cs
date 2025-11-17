using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmINSach : Form
    {
        private string TheloaiTruyenfrm;
        private string tenNguoiIN;
        private int id;
        public frmINSach()
        {
            InitializeComponent();
        }
        public frmINSach(int _id, string _theloai)
        {
            InitializeComponent();
            TheloaiTruyenfrm = _theloai;
            id = _id;
            DatabaseDataContext dt = new DatabaseDataContext();
            tenNguoiIN = dt.NguoiDungs.SingleOrDefault(p => p.ID == id).HoTen;
        }
        private void frmSachPrinter_Load(object sender, EventArgs e)
        {
            if (TheloaiTruyenfrm == "Tất cả sách")
            {
                ReportParameter ten = new ReportParameter("NguoiIN", tenNguoiIN);
                ReportParameter loai = new ReportParameter("TheLoai", "TẤT CẢ SÁCH");

                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { ten, loai });
                this.reportViewer1.RefreshReport();

                DatabaseDataContext dt = new DatabaseDataContext();
                var dataSource = (
                    from s in dt.Saches
                    join tg in dt.TacGias on s.MaTG equals tg.MaTG into TacGiaGroup
                    from TacGia in TacGiaGroup.DefaultIfEmpty()
                    join tl in dt.TheLoais on s.MaTheLoai equals tl.MaTheLoai into TheLoaiGroup
                    from TheLoai in TheLoaiGroup.DefaultIfEmpty()
                    join nxb in dt.NhaXuatBans on s.MaNXB equals nxb.MaNXB into NXBGroup
                    from NhaXuatBan in NXBGroup.DefaultIfEmpty()
                    orderby s.ID
                    select new
                    {
                        s.ID, // ID sách
                        TenSach = s.TenSach, // Tên sách
                        MaTG = TacGia != null ? TacGia.TenTG : "N/A", // Tên tác giả
                        MaNXB = NhaXuatBan != null ? NhaXuatBan.TenNXB : "N/A", // Nhà xuất bản
                        MaTheLoai = TheLoai != null ? TheLoai.TenTheLoai : "N/A", // Thể loại
                        s.SoLuong, // Số lượng sách
                        s.MoTa // Mô tả sách
                    }
                ).ToList();

                // Xóa các nguồn dữ liệu cũ
                this.reportViewer1.LocalReport.DataSources.Clear();

                // Thêm nguồn dữ liệu vào báo cáo
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetSach", dataSource));

                // Làm mới báo cáo
                this.reportViewer1.RefreshReport();
            }
            else
            {
                string res = "THỂ LOẠI " + TheloaiTruyenfrm;

                ReportParameter ten = new ReportParameter("NguoiIN", tenNguoiIN);
                ReportParameter loai = new ReportParameter("TheLoai", res.ToUpper());

                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { ten, loai });
                this.reportViewer1.RefreshReport();

                DatabaseDataContext dt = new DatabaseDataContext();
                var dataSource = (
                    from s in dt.Saches
                    join tg in dt.TacGias on s.MaTG equals tg.MaTG into TacGiaGroup
                    from TacGia in TacGiaGroup.DefaultIfEmpty()
                    join tl in dt.TheLoais on s.MaTheLoai equals tl.MaTheLoai into TheLoaiGroup
                    from TheLoai in TheLoaiGroup.DefaultIfEmpty()
                    join nxb in dt.NhaXuatBans on s.MaNXB equals nxb.MaNXB into NXBGroup
                    from NhaXuatBan in NXBGroup.DefaultIfEmpty()
                    where TheLoai.TenTheLoai == TheloaiTruyenfrm
                    orderby s.ID
                    select new
                    {
                        s.ID, // ID sách
                        TenSach = s.TenSach, // Tên sách
                        MaTG = TacGia != null ? TacGia.TenTG : "N/A", // Tên tác giả
                        MaNXB = NhaXuatBan != null ? NhaXuatBan.TenNXB : "N/A", // Nhà xuất bản
                        MaTheLoai = TheLoai != null ? TheLoai.TenTheLoai : "N/A", // Thể loại
                        s.SoLuong, // Số lượng sách
                        s.MoTa // Mô tả sách
                    }
                ).ToList();

                // Xóa các nguồn dữ liệu cũ
                this.reportViewer1.LocalReport.DataSources.Clear();

                // Thêm nguồn dữ liệu vào báo cáo
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetSach", dataSource));

                // Làm mới báo cáo
                this.reportViewer1.RefreshReport();
            }
        }
    }
}
