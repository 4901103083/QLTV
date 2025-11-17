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
    public partial class frmINPhieuChon : Form
    {
        private int id;
        private string tenNguoiIn;
        private string maPhieu;
        public frmINPhieuChon()
        {
            InitializeComponent();
        }
        public frmINPhieuChon(int _id, string _maPhieu)
        {
            InitializeComponent();
            id = _id;
            maPhieu = _maPhieu;
            DatabaseDataContext dt = new DatabaseDataContext();
            tenNguoiIn = dt.NguoiDungs.SingleOrDefault(p => p.ID == id).HoTen;
        }
        private void frmInPhieuChon_Load(object sender, EventArgs e)
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            var dataSource = dp.PhieuMuons
            .Where(p => p.MaPhieu.ToString() == maPhieu) // Lọc theo MaPhieu
            .Select(p => new
            {
                p.MaPhieu,
                IDBanDoc = dp.NguoiDungs
                    .Where(n => n.ID == p.IDBanDoc)
                    .Select(n => n.HoTen)
                    .FirstOrDefault() ?? "N/A",
                IDSach = dp.Saches
                    .Where(s => s.ID == p.IDSach)
                    .Select(s => s.TenSach)
                    .FirstOrDefault() ?? "N/A",
                p.SoLuong,
                p.NgayDangKyMuon,
                p.NgayMuon,
                p.HanTra,
                TrangThai = p.TrangThai == 0 ? "Chưa Mượn" :
                            p.TrangThai == 1 ? "Đang Mượn" :
                            p.TrangThai == 2 ? "Quá Hạn" :
                            p.TrangThai == 3 ? "Đã Trả" : "Không xác định"
            }).ToList();
            ReportParameter tenNI = new ReportParameter("NguoiIN", tenNguoiIn);
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { tenNI });
            this.reportViewer1.RefreshReport();
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(
                new Microsoft.Reporting.WinForms.ReportDataSource("DataSetPhieuMuon", dataSource)
            );
            this.reportViewer1.RefreshReport();
        }
    }
}
