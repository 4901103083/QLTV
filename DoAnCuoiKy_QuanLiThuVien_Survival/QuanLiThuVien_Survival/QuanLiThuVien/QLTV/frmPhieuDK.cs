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
    public partial class frmPhieuDK : Form
    {
        public frmPhieuDK()
        {
            InitializeComponent();
        }

        private void PhieuDK_Load(object sender, EventArgs e)
        {
            LoadDuLieu();
            lblThongTin.Enabled = false;
            lblNgayMuon.Enabled = false;
            lblHanTra.Enabled = false;
            btnHuy.Enabled = false;
            dtpNgayMuon.Enabled = false;
            dtpHanTra.Enabled = false;
            btnXacNhan.Enabled = false;
        }
        private void LoadDuLieu()
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            var allData = dp.PhieuMuons.Where(p => p.TrangThai == 0).OrderBy(p => p.NgayDangKyMuon).ToList();
            // Kiểm tra và xử lý phiếu mượn đã quá 3 ngày
            foreach (var phieu in allData)
            {
                if (phieu.NgayDangKyMuon?.AddDays(3) < DateTime.Today)
                {
                    var sach = dp.Saches.SingleOrDefault(s => s.ID == phieu.IDSach);
                    if (sach != null)
                    {
                        sach.SoLuong += phieu.SoLuong;
                    }
                    dp.PhieuMuons.DeleteOnSubmit(phieu);
                }
            }
            dp.SubmitChanges();

            var Data = dp.PhieuMuons
            .Join(dp.NguoiDungs,
                    pm => pm.IDBanDoc,
                    nd => nd.ID,
                    (pm, nd) => new { pm, nd })
            .Join(dp.Saches,
                    temp => temp.pm.IDSach,
                    sach => sach.ID,
                    (temp, sach) => new
                    {
                        MaPhieu = temp.pm.MaPhieu,
                        IDBanDoc = temp.nd.HoTen, // Lấy tên bạn đọc
                        IDSach = sach.TenSach,     // Lấy tên sách
                        SoLuong = temp.pm.SoLuong,
                        NgayDangKyMuon = temp.pm.NgayDangKyMuon,
                        NgayMuon = temp.pm.NgayMuon,
                        HanTra = temp.pm.HanTra,
                        NgayTra = temp.pm.NgayTra,
                        TrangThai = temp.pm.TrangThai == 0 ? "Chưa Mượn" :
                                    temp.pm.TrangThai == 1 ? "Đang Mượn" :
                                    temp.pm.TrangThai == 2 ? "Quá Hạn" :
                                    temp.pm.TrangThai == 3 ? "Đã Trả" : "Không xác định"
                    })
            .Where(p => p.TrangThai == "Chưa Mượn")
            .OrderBy(p => p.NgayDangKyMuon)
            .ToList();

            dgvPhieu.DataSource = Data;

            foreach (DataGridViewColumn col in dgvPhieu.Columns)
            {
                if (!new string[] { "MaPhieu", "IDBanDoc", "IDSach", "SoLuong", "NgayDangKyMuon", "TrangThai" }
                        .Contains(col.Name))
                {
                    col.Visible = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtIDBanDoc.Text) && !string.IsNullOrWhiteSpace(txtIDSach.Text))
            {
                btnDK.Enabled = true;
            }
            else
            {
                btnDK.Enabled = false;
            }

            // Chọn dòng đầu tiên nếu có dữ liệu
            if (dgvPhieu.Rows.Count > 0)
            {
                dgvPhieu.Rows[0].Selected = true; // Chọn dòng đầu tiên
                hienThiDuLieuDong(0); // Hiển thị dữ liệu dòng đầu tiên
            }
            else
            {
                ClearInputFields(); // Làm trống các trường nhập liệu nếu không có dữ liệu
            }
        }

        private void dgvPhieu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int idrow = e.RowIndex;

            // Kiểm tra nếu không có dòng nào được chọn hoặc DataGridView trống
            if (idrow == -1 || dgvPhieu.Rows.Count == 0 || dgvPhieu.CurrentRow == null)
            {
                ClearInputFields(); // Làm trống các ô nhập liệu
                btnDK.Enabled = false;
                return;
            }

            // Kiểm tra nếu dòng được chọn không hợp lệ (ví dụ: giá trị null hoặc lỗi)
            if (dgvPhieu.Rows[idrow].Cells[0].Value == null)
            {
                ClearInputFields(); // Làm trống các ô nhập liệu
                btnDK.Enabled = false;
                return;
            }

            // Hiển thị dữ liệu dòng hiện tại
            hienThiDuLieuDong(idrow);
            lblThongTin.Enabled = false;
            lblNgayMuon.Enabled = false;
            btnHuy.Enabled = false;
            lblHanTra.Enabled = false;
            dtpNgayMuon.Enabled = false;
            dtpHanTra.Enabled = false;
            btnXacNhan.Enabled = false;
        }
        private void ClearInputFields()
        {
            txtMaPhieu.Text = "";
            txtIDSach.Text = "";
            txtIDBanDoc.Text = "";
            txtSoLuong.Text = "";
            txtTrangThai.Text = "";
            dtpNgayDK.Value = DateTime.Today.Date;
            dtpNgayMuon.Value = DateTime.Today.Date;
            dtpHanTra.Value = dtpNgayMuon.Value.AddDays(7);
            btnDK.Enabled = false; // Vô hiệu hóa nút Đăng Ký
        }
        private void hienThiDuLieuDong(int idrow)
        {
            DatabaseDataContext dp = new DatabaseDataContext();
            string madg = dgvPhieu.Rows[idrow].Cells[0].Value.ToString();
            DataGridViewRow row = dgvPhieu.Rows[idrow];
            PhieuMuon mt = dp.PhieuMuons.Where(p => p.MaPhieu.ToString() == madg).SingleOrDefault();
            if (idrow == -1 || dgvPhieu.Rows.Count == 0) // Kiểm tra nếu không có dòng nào được chọn hoặc dgvMuonTra trống
            {
                // Làm trống các ô nhập liệu
                txtMaPhieu.Text = "";
                txtIDSach.Text = "";
                txtIDBanDoc.Text = "";
                txtSoLuong.Text = "";
                txtTrangThai.Text = "";
                dtpNgayDK.Value = DateTime.Today.Date;
                dtpNgayMuon.Value = DateTime.Today.Date;
                dtpHanTra.Value = dtpNgayMuon.Value.AddDays(7);
                btnDK.Enabled = false;
                return;
            }
            if (mt != null)
            {
                txtMaPhieu.Text = mt.MaPhieu.ToString();
                txtIDSach.Text = row.Cells["IDSach"].Value.ToString();
                txtIDBanDoc.Text = row.Cells["IDBanDoc"].Value.ToString(); ;
                txtSoLuong.Text = mt.SoLuong.ToString();
                txtTrangThai.Text = mt.TrangThai == 0 ? "Chưa Mượn" :
                                    mt.TrangThai == 1 ? "Đang Mượn" :
                                    mt.TrangThai == 2 ? "Quá Hạn" : 
                                    mt.TrangThai == 3 ? "Đã Trả" : "Không xác định";
                dtpNgayDK.Text = mt.NgayDangKyMuon.ToString();
                dtpNgayMuon.Value = DateTime.Today.Date;
                dtpHanTra.Value = dtpNgayMuon.Value.AddDays(7);
                if (!string.IsNullOrWhiteSpace(txtIDBanDoc.Text) && !string.IsNullOrWhiteSpace(txtIDSach.Text))
                {
                    btnDK.Enabled = true;
                }
                else
                {
                    btnDK.Enabled = false;
                }
            }
        }

        private void btnDK_Click(object sender, EventArgs e)
        {
            lblThongTin.Enabled = true;
            lblNgayMuon.Enabled = true;
            btnHuy.Enabled = true;
            lblHanTra.Enabled = true;
            dtpNgayMuon.Enabled = true;
            dtpHanTra.Enabled = true;
            btnXacNhan.Enabled = true;
            btnDK.Enabled = false;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            dtpNgayMuon.Value = DateTime.Today.Date;
            dtpHanTra.Value = dtpNgayMuon.Value.AddDays(7);
            lblThongTin.Enabled = false;
            lblNgayMuon.Enabled = false;
            btnHuy.Enabled = false;
            lblHanTra.Enabled = false;
            dtpNgayMuon.Enabled = false;
            dtpHanTra.Enabled = false;
            btnXacNhan.Enabled = false;
            btnDK.Enabled = true;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // Kiểm tra ngày mượn phải lớn hơn hoặc bằng ngày đăng ký
            if (dtpNgayMuon.Value < dtpNgayDK.Value)
            {
                MessageBox.Show("Ngày mượn phải lớn hơn hoặc bằng ngày đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra ngày hạn trả phải lớn hơn hoặc bằng ngày mượn
            if (dtpHanTra.Value < dtpNgayMuon.Value)
            {
                MessageBox.Show("Ngày hạn trả phải lớn hơn hoặc bằng ngày mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tìm phiếu mượn theo mã phiếu
            DatabaseDataContext dp = new DatabaseDataContext();
            int maPhieu = int.Parse(txtMaPhieu.Text); // Lấy mã phiếu từ ô nhập liệu
            PhieuMuon phieu = dp.PhieuMuons.SingleOrDefault(p => p.MaPhieu == maPhieu);

            if (phieu != null)
            {
                // Cập nhật các thuộc tính của phiếu mượn
                phieu.NgayMuon = dtpNgayMuon.Value;
                phieu.HanTra = dtpHanTra.Value;
                phieu.TrangThai = 1; // Đặt tình trạng là 1 sau khi xác nhận

                // Lưu thay đổi vào cơ sở dữ liệu
                dp.SubmitChanges();
                MessageBox.Show("Phiếu mượn đã được cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy phiếu mượn với mã phiếu này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Reset các trường nhập liệu sau khi cập nhật phiếu mượn
            dtpNgayMuon.Value = DateTime.Today.Date;
            dtpHanTra.Value = dtpNgayMuon.Value.AddDays(7);
            lblThongTin.Enabled = false;
            lblNgayMuon.Enabled = false;
            btnHuy.Enabled = false;
            lblHanTra.Enabled = false;
            dtpNgayMuon.Enabled = false;
            dtpHanTra.Enabled = false;
            btnXacNhan.Enabled = false;
            txtMaPhieu.Text = "";
            txtIDSach.Text = "";
            txtIDBanDoc.Text = "";
            txtSoLuong.Text = "";
            txtTrangThai.Text = "";
            dtpNgayDK.Value = DateTime.Today.Date;
            btnDK.Enabled = false;
            // Cập nhật lại dữ liệu bảng sau khi sửa phiếu mượn
            LoadDuLieu();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
