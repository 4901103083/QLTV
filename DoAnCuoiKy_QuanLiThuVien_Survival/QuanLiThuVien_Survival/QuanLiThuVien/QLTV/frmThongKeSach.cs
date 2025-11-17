using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmThongKeSach : Form
    {
        private string loai;
        private int id;
        public frmThongKeSach()
        {
            InitializeComponent();
        }
        public frmThongKeSach(int _id, string _loai)
        {
            InitializeComponent();
            id = _id;
            loai = _loai;
        }
        private Timer timer1 = new Timer();
        private void ThongKeSach_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // 1 giây
            timer1.Tick += timer1_Tick_1; // Liên kết sự kiện Tick
            timer1.Start(); // Bắt đầu Timer
            LoadDuLieuSach();
        }
        private void LoadDuLieuSach()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            //var topSach = db.PhieuMuons
            //    .Where(p => p.TrangThai == 1 || p.TrangThai == 2 || p.TrangThai == 3) // Lọc phiếu mượn có trạng thái đang mượn
            //    .GroupBy(p => new { p.Sach.ID, p.Sach.TenSach, p.Sach.MaTG, p.Sach.MoTa })
            //    .Select(g => new
            //    {
            //        g.Key.ID,
            //        g.Key.TenSach,
            //        g.Key.MaTG,
            //        g.Key.MoTa,
            //        SoLuongSachMuon = g.Count()
            //    })
            //    .OrderByDescending(x => x.SoLuongSachMuon)
            //    .Take(4);


            var topSachs = (
                from p in db.PhieuMuons
                join s in db.Saches on p.IDSach equals s.ID into SachGroup
                from Sach in SachGroup.DefaultIfEmpty()
                join tg in db.TacGias on Sach.MaTG equals tg.MaTG into TacGiaGroup
                from TacGia in TacGiaGroup.DefaultIfEmpty()
                where (p.TrangThai == 1 || p.TrangThai == 2 || p.TrangThai == 3) // Lọc phiếu mượn đang mượn
                group new { p, Sach, TacGia, MoTa }
                by new
                {
                    Sach.ID,
                    Sach.TenSach,
                    TacGia.TenTG,
                    Sach.MoTa
                } into g
                orderby g.Count() descending // Sắp xếp theo số lượng mượn giảm dần
                select new
                {
                    ID = g.Key.ID,
                    TenSach = g.Key.TenSach,
                    MaTG = g.Key.TenTG ?? "N/A",        // Xử lý null
                    MoTa = g.Key.MoTa
                }
            ).Take(4).ToList(); // Lấy top 4 sách mượn nhiều nhất

            tkSach.DataSource = topSachs;
            tkSach.Columns[2].HeaderText = "Mã tác giả";
            tkSach.Columns[3].HeaderText = "Mô tả";
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

            lbTrong.Text = DateTime.Now.ToString("dd/MM/yyyy | hh:mm tt");
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInSach_Click(object sender, EventArgs e)
        {
            frmINThongKeSach frmINTK = new frmINThongKeSach(id, loai);
            frmINTK.Show();
        }
    }
}
