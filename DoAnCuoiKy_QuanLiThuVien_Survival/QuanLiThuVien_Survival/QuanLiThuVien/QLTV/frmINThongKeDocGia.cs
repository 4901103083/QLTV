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
    public partial class frmINThongKeDocGia : Form
    {
        private string tenNguoiIn;
        private string tenloai;
        private int id;
        public frmINThongKeDocGia()
        {
            InitializeComponent();
        }
        public frmINThongKeDocGia(int _id, string _loai)
        {
            InitializeComponent();
            id = _id;
            tenloai = _loai;
        }
        private void frmINThongKeDocGia_Load(object sender, EventArgs e)
        {
            DatabaseDataContext dt = new DatabaseDataContext();
            tenNguoiIn = dt.NguoiDungs.Where(p => p.ID == id).Select(t => t.HoTen).FirstOrDefault();

            DatabaseDataContext db = new DatabaseDataContext();
            var topDocGia = db.PhieuMuons
            .Where(p => p.TrangThai == 1) // Lọc phiếu mượn có trạng thái đang mượn
            .GroupBy(p => new { p.NguoiDung.HoTen, p.NguoiDung.ID, p.NguoiDung.Email })
            .Select(g => new
            {
                ID = g.Key.ID,
                HoTen = g.Key.HoTen,
                Email = g.Key.Email,
                TenDangNhap = g.Count().ToString()
            })
                //.OrderByDescending(x => x.SoLuong)
            .Take(4);
            //DataTable table = new DataTable();
            //table.Columns.Add("ID", typeof(int));
            //table.Columns.Add("HoTen", typeof(string));
            //table.Columns.Add("Email", typeof(string));
            //table.Columns.Add("SoLuong", typeof(int)); // Thêm cột SoLuong

            //foreach (var item in topDocGia)
            //{
            //    table.Rows.Add(item.ID, item.HoTen, item.Email, item.SoLuong);
            //}


            ReportParameter tenNI = new ReportParameter("NguoiIN", tenNguoiIn);
            ReportParameter Loai = new ReportParameter("TheLoai", tenloai.ToUpper());


            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { tenNI, Loai });

            ReportDataSource dataSource = new ReportDataSource("DataSetDG", topDocGia);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(dataSource);
            reportViewer1.RefreshReport();
        }
    }
}