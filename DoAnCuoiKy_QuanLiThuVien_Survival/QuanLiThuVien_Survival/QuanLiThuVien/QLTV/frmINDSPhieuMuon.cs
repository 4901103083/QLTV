using Microsoft.Reporting.WinForms;
using QLTV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thư_viện_mượn_trả_mới
{
    public partial class frmINDSPhieuMuon : Form
    {
        private string tenNguoiIn = "";
        private string strloaiPhieu = "";
        private int loaiPhieu;
        public frmINDSPhieuMuon()
        {
            InitializeComponent();
        }
        public frmINDSPhieuMuon(string _tenNI, int _loaiPhieu)
        {
            InitializeComponent();
            tenNguoiIn = _tenNI;
            loaiPhieu = _loaiPhieu;
            if (loaiPhieu == 1)
            {
                strloaiPhieu = "ĐANG MƯỢN";
            }
            else if (loaiPhieu == 3)
            {
                strloaiPhieu = "QÚA HẠN";
            }
            else
            {
                strloaiPhieu = "ĐÃ TRẢ";
            }
        }
        private void inDSMuon_Load(object sender, EventArgs e)
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            var dataSource = dp.PhieuMuons
            .Where(p => p.TrangThai == loaiPhieu)
            .OrderBy(p => p.MaPhieu)
            .Select(p => new
            {
                p.MaPhieu,
                IDBanDoc = dp.NguoiDungs
                    .Where(n => n.ID == p.IDBanDoc)
                    .Select(n => n.HoTen)
                    .FirstOrDefault() ?? "N/A",
                IDSach = dp.Saches
                    .Where(s => s.ID == p.IDSach)       // Lọc sách có ID khớp
                    .Select(s => s.TenSach)             // Chọn tên sách (TenSach)
                    .FirstOrDefault() ?? "N/A",
                p.SoLuong,
                p.NgayDangKyMuon,
                p.NgayMuon,
                p.HanTra,
                TrangThai = p.TrangThai == 1 ? "Đang mượn" : 
                            p.TrangThai == 3 ? "Quá Hạn" :
                            p.TrangThai == 2 ? "Đã trả" : "Khác"
            }).ToList();

            ReportParameter ten = new ReportParameter("NguoiIN", tenNguoiIn);
            ReportParameter loai = new ReportParameter("LoaiPhieu", strloaiPhieu);

            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { ten, loai });
            this.reportViewer1.RefreshReport();
            // Thêm dữ liệu vào Report Viewer
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(
                new Microsoft.Reporting.WinForms.ReportDataSource("DataSetMuon", dataSource)
            );


            this.reportViewer1.RefreshReport();
        }
    }
}
