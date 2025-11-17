using QLTV;
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

namespace QLThuVien
{
    public partial class frmLichSuMuon : Form
    {
        private string getUsername = "";
        private int id;
        public frmLichSuMuon()
        {
            InitializeComponent();
        }
        public frmLichSuMuon(int _ID)
        {
            InitializeComponent();
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.ID == _ID);
            getUsername = ng.HoTen;
            id = ng.ID;
        }
        private void frmLichSuMuon_Load(object sender, EventArgs e)
        {
            LoadDuLieuSach();
        }

        private void LoadDuLieuSach()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung user = db.NguoiDungs.Where(p => p.ID == id).SingleOrDefault();

            var phieumuon = db.PhieuMuons.Where(p => p.IDBanDoc == user.ID)
                .Join(
                    db.Saches,
                    pm => pm.IDSach,
                    s => s.ID,
                    (pm, s) => new
                    {
                        pm.MaPhieu,
                        s.TenSach,
                        pm.NgayMuon,
                        pm.SoLuong,
                        pm.NgayTra,
                        TrangThai = pm.TrangThai == 0 ? "Chưa Mượn" :
                                    pm.TrangThai == 1 ? "Đang Mượn" :
                                    pm.TrangThai == 2 ? "Quá Hạn" :
                                    pm.TrangThai == 3 ? "Đã Trả" : "Không xác định"
                    }
                ).ToList();

            LichSuMuon.DataSource = phieumuon;
            LichSuMuon.Columns[5].HeaderText = "Trạng Thái";
        }
    }
}
