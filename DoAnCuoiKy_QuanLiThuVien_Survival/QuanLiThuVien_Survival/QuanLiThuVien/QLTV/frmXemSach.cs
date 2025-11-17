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
    public partial class frmXemSach : Form
    {
        private string tenBD = "";
        private int id = 0;
        public frmXemSach()
        {
            InitializeComponent();
        }
        public frmXemSach(int _ID)
        {
            InitializeComponent();
            id = _ID;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDuLieu();
            btnDangKi.Enabled = false;
            lblSoLuongDK.Enabled = false;
            txtSoLuongDK.Enabled = false;
        }
        private void LoadDuLieu()
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            var data = dp.Saches
            .Join(dp.TacGias, sach => sach.MaTG, tacGia => tacGia.MaTG, (sach, tacGia) => new { sach, tacGia })
            .Join(dp.NhaXuatBans, temp => temp.sach.MaNXB, nxb => nxb.MaNXB, (temp, nxb) => new { temp.sach, temp.tacGia, nxb })
            .Join(dp.TheLoais, temp => temp.sach.MaTheLoai, tl => tl.MaTheLoai, (temp, tl) => new
            {
                ID = temp.sach.ID,
                TenSach = temp.sach.TenSach,
                MaTG = temp.tacGia.TenTG,
                MaNXB = temp.nxb.TenNXB,
                MaTheLoai = tl.TenTheLoai,
                SoLuong = temp.sach.SoLuong,
                MoTa = temp.sach.MoTa
            })
            .OrderBy(s => s.ID)
            .ToList();
            dgvSach.DataSource = data;
            foreach (DataGridViewColumn col in dgvSach.Columns)
            {
                if (!new string[] { "ID", "TenSach", "MaTG", "MaNXB", "MaTheLoai", "SoLuong" }
                        .Contains(col.Name))
                {
                    col.Visible = false;
                }
            }
        }
        private void hienThiDuLieuDong(int idrow)
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            DataGridViewRow row = dgvSach.Rows[idrow];
            string madg = dgvSach.Rows[idrow].Cells[0].Value.ToString();
            Sach mt = dp.Saches.Where(p => p.ID.ToString() == madg).SingleOrDefault();
            if (idrow == -1 || dgvSach.Rows.Count == 0) // Kiểm tra nếu không có dòng nào được chọn hoặc dgvMuonTra trống
            {
                // Làm trống các ô nhập liệu
                txtID.Text = "";
                txtTenSach.Text = "";
                txtSoLuong.Text = "";
                txtMaNXB.Text = "";
                txtMoTa.Text = "";
                txtMaTG.Text = "";
                txtMaTheLoai.Text = "";
                return;
            }
            if (mt != null)
            {
                txtID.Text = mt.ID.ToString();
                txtTenSach.Text = mt.TenSach.ToString();
                txtSoLuong.Text = mt.SoLuong.ToString();

                txtMaTG.Text = row.Cells["MaTG"].Value.ToString();
                txtMaNXB.Text = row.Cells["MaNXB"].Value.ToString();               
                txtMaTheLoai.Text = row.Cells["MaTheLoai"].Value.ToString();

                txtMoTa.Text = mt.MoTa.ToString();
            }
            if (mt.SoLuong > 0)
            {
                //btnDangKi.Enabled = true;
                lblSoLuongDK.Enabled = true;
                txtSoLuongDK.Enabled = true;
            }
            else
            {
                //btnDangKi.Enabled = false;
                lblSoLuongDK.Enabled = false;
                txtSoLuongDK.Enabled = false;
            }
        }
        private void dgvSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int idrow = e.RowIndex;
            if (idrow == -1 || dgvSach.Rows.Count == 0 || dgvSach.CurrentRow == null) // Kiểm tra nếu không có dòng nào được chọn hoặc dgvMuonTra trống
            {
                ClearInputFields(); // Làm trống các ô nhập liệu
                return;
            }
            hienThiDuLieuDong(idrow);
        }
        private void ClearInputFields()
        {
            txtID.Text = "";
            txtTenSach.Text = "";
            txtSoLuong.Text = "";
            txtMaNXB.Text = "";
            txtMoTa.Text = "";
            txtMaTG.Text = "";
            txtMaTheLoai.Text = "";
        }

        private void txtSoLuongDK_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtSoLuongDK.Text, out int soLuongDK) &&
                int.TryParse(txtSoLuong.Text, out int soLuong) &&
                soLuongDK > 0 && soLuongDK <= soLuong)
            {
                btnDangKi.Enabled = true;
            }
            else
            {
                btnDangKi.Enabled = false;
            }
        }

        private void btnDangKi_Click(object sender, EventArgs e)
        {
            if (dgvSach.CurrentRow != null)
            {
                int idSach = int.Parse(txtID.Text);
                int soLuongDK = int.Parse(txtSoLuongDK.Text);
                DateTime ngayDangKyMuon = DateTime.Now.Date;

                DatabaseDataContext dp = new DatabaseDataContext();
                var sach = dp.Saches.SingleOrDefault(s => s.ID == idSach);
                var ng = dp.NguoiDungs.SingleOrDefault(p => p.ID == id);

                MessageBox.Show($"Bạn đọc: {ng.HoTen}\nTên Sách: {sach.TenSach}\nSố lượng đăng ký: {soLuongDK}\nNgày đăng ký mượn: {ngayDangKyMuon:dd/MM/yyyy}",
                    "Thông tin đăng ký mượn sách", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (sach != null)
                {
                    if (sach.SoLuong >= soLuongDK)
                    {
                        sach.SoLuong -= soLuongDK;

                        PhieuMuon phieuMuon = new PhieuMuon
                        {
                            IDBanDoc = ng.ID,
                            IDSach = idSach,
                            SoLuong = soLuongDK,
                            NgayDangKyMuon = ngayDangKyMuon,
                            TrangThai = 0
                        };

                        dp.PhieuMuons.InsertOnSubmit(phieuMuon);
                        dp.SubmitChanges();
                    }
                    else
                    {
                        MessageBox.Show("Số lượng sách trong kho không đủ để mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
