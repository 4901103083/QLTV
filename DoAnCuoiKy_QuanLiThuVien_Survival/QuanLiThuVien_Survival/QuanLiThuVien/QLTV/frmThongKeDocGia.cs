using System;
using System.Linq;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmThongKeDocGia : Form
    {
        private int id;
        private string tenLoai;

        private Timer timer1 = new Timer();
        public frmThongKeDocGia()
        {
            InitializeComponent();
        }
        public frmThongKeDocGia(int _id, string _tenLoai)
        {
            InitializeComponent();
            id = _id;
            tenLoai = _tenLoai;
        }
        private void ThongKeDocGia_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // 1 giây
            timer1.Tick += timer1_Tick_1; // Liên kết sự kiện Tick
            timer1.Start(); // Bắt đầu Timer
            LoadDuLieuDocGia();
        }

        private void LoadDuLieuDocGia()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var topDocGia = db.PhieuMuons
            .Where(p => p.TrangThai == 1) // Lọc phiếu mượn có trạng thái đang mượn
            .GroupBy(p => new { p.NguoiDung.HoTen, p.NguoiDung.ID, p.NguoiDung.Email })
            .Select(g => new
            {
                HoTen = g.Key.HoTen,
                ID = g.Key.ID,
                Email = g.Key.Email,
                SoLuongSachMuon = g.Count()
            })
            .OrderByDescending(x => x.SoLuongSachMuon)
            .Take(4);
            tkDocGia.DataSource = topDocGia;
            tkDocGia.Columns[0].HeaderText = "Họ và Tên";
            tkDocGia.Columns[1].HeaderText = "Mã độc giả";
            tkDocGia.Columns[2].HeaderText = "Email";
            tkDocGia.Columns[3].HeaderText = "Số sách";
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
            frmINThongKeDocGia frmINTK = new frmINThongKeDocGia(id, tenLoai);
            frmINTK.Show();
        }
    }
}
