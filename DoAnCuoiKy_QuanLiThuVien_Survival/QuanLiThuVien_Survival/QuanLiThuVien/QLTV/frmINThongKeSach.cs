using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace QLTV
{
    public partial class frmINThongKeSach : Form
    {
        private string tenNguoiIn;
        private string tenloai;
        private int id;
        public frmINThongKeSach()
        {
            InitializeComponent();
        }
        public frmINThongKeSach(int _id, string _loai)
        {
            InitializeComponent();
            id = _id;
            tenloai = _loai; //_loai;
            DatabaseDataContext dt = new DatabaseDataContext();
            tenNguoiIn = dt.NguoiDungs.SingleOrDefault(p => p.ID == id).HoTen;
        }
        private void frmINThongKe_Load(object sender, EventArgs e)
        {

            // Tạo kết nối dữ liệu
            DatabaseDataContext dt = new DatabaseDataContext();

            // Truy vấn top 4 sách mượn nhiều nhất
            var topSach = (
                from p in dt.PhieuMuons
                join s in dt.Saches on p.IDSach equals s.ID into SachGroup
                from Sach in SachGroup.DefaultIfEmpty()
                join tg in dt.TacGias on Sach.MaTG equals tg.MaTG into TacGiaGroup
                from TacGia in TacGiaGroup.DefaultIfEmpty()
                join tl in dt.TheLoais on Sach.MaTheLoai equals tl.MaTheLoai into TheLoaiGroup
                from TheLoai in TheLoaiGroup.DefaultIfEmpty()
                join nxb in dt.NhaXuatBans on Sach.MaNXB equals nxb.MaNXB into NXBGroup
                from NhaXuatBan in NXBGroup.DefaultIfEmpty()
                where (p.TrangThai == 1 || p.TrangThai == 2 || p.TrangThai == 3) // Lọc phiếu mượn đang mượn
                group new { p, Sach, TacGia, TheLoai, NhaXuatBan }
                by new
                {
                    Sach.ID,
                    Sach.TenSach,
                    TacGia.TenTG,
                    NhaXuatBan.TenNXB,
                    TheLoai.TenTheLoai,
                    Sach.MoTa
                } into g
                orderby g.Count() descending // Sắp xếp theo số lượng mượn giảm dần
                select new
                {
                    ID = g.Key.ID,
                    TenSach = g.Key.TenSach,
                    MaTG = g.Key.TenTG ?? "N/A",        // Xử lý null
                    MaNXB = g.Key.TenNXB ?? "N/A",      // Xử lý null
                    MaTheLoai = g.Key.TenTheLoai ?? "N/A", // Xử lý null
                    SoLuong = g.Count(),               // Số lượng sách được mượn
                    MoTa = g.Key.MoTa
                }
            ).Take(4).ToList(); // Lấy top 4 sách mượn nhiều nhất

            ReportParameter ten = new ReportParameter("NguoiIN", tenNguoiIn);
            ReportParameter loai = new ReportParameter("Loai", tenloai.ToUpper());

            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { ten, loai });
            this.reportViewer1.RefreshReport();


            // Xóa nguồn dữ liệu cũ trong ReportViewer
            this.reportViewer1.LocalReport.DataSources.Clear();

            // Thêm nguồn dữ liệu mới vào báo cáo
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetSach", topSach));

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }
    }
}
