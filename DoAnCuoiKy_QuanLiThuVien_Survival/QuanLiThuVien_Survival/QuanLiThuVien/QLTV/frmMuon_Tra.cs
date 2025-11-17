using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thư_viện_mượn_trả_mới;

namespace QLTV
{
    public partial class frmMuon_Tra : Form
    {
        private int id;
        private string tenNI;
                public frmMuon_Tra()
        {
            InitializeComponent();
        }
        public frmMuon_Tra(int _id)
        {
            InitializeComponent();
            id = _id;
            DatabaseDataContext dp = new DatabaseDataContext();
            var nd = dp.NguoiDungs.SingleOrDefault(p => p.ID == id);
            tenNI = nd.HoTen;
        }
        private string Value = "";
        private void frmMuon_Tra_Load(object sender, EventArgs e)
        {
            rbDSMuon.Checked = true;
            LoadDuLieu();
        }
        private void LoadDuLieu()
        {
            using (DatabaseDataContext dp = new DatabaseDataContext())
            {
                // Cập nhật trạng thái phiếu mượn nếu đã quá hạn và chưa trả sách
                var overduePhieuMuons = dp.PhieuMuons
                    .Where(p => p.NgayTra == null && p.HanTra < DateTime.Today.Date)
                    .ToList();

                foreach (var phieu in overduePhieuMuons)
                {
                    phieu.TrangThai = 2; // Cập nhật trạng thái thành 2 - Chưa trả
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                dp.SubmitChanges();

                if (Value == "Mượn")
                {
                    var allData = dp.PhieuMuons
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
                                          temp.pm.TrangThai == 2 ? "Quá Hạn" : "Không xác định"
                                          //temp.pm.TrangThai == 3 ? "Đã trả" :
                          })
                    .Where(p => p.TrangThai == "Đang Mượn" || p.TrangThai == "Quá Hạn")
                    .OrderBy(p => p.MaPhieu)
                    .ToList();

                    dgvMuon.DataSource = allData;

                    foreach (DataGridViewColumn col in dgvMuon.Columns)
                    {
                        if (!new string[] { "MaPhieu", "IDBanDoc", "IDSach", "SoLuong", "NgayDangKyMuon", "NgayMuon", "HanTra", "NgayTra", "TrangThai" }
                                .Contains(col.Name))
                        {
                            col.Visible = false;
                        }
                    }

                    lblThongBao.Text = $"Tổng số phiếu: {dgvMuon.Rows.Count}";
                }
                else
                {
                    var allData = dp.PhieuMuons
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
                    .Where(p => p.TrangThai == "Đã Trả")
                    .OrderBy(p => p.MaPhieu)
                    .ToList();

                    dgvMuon.DataSource = allData;

                    foreach (DataGridViewColumn col in dgvMuon.Columns)
                    {
                        if (!new string[] { "MaPhieu", "IDBanDoc", "IDSach", "SoLuong", "NgayDangKyMuon", "NgayMuon", "HanTra", "NgayTra", "TrangThai" }
                                .Contains(col.Name))
                        {
                            col.Visible = false;
                        }
                    }

                    lblThongBao.Text = $"Tổng số phiếu: {dgvMuon.Rows.Count}";
                }
            }
        }

        private void dgvMuon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int idrow = e.RowIndex;

            // Kiểm tra nếu không có dòng nào được chọn hoặc DataGridView trống
            if (idrow == -1 || dgvMuon.Rows.Count == 0 || dgvMuon.CurrentRow == null)
            {
                ClearInputFields(); // Làm trống các ô nhập liệu
                lblThongBao.Text = $"Tổng số phiếu: {dgvMuon.Rows.Count}";  // Cập nhật lblThongBao
                return;
            }

            // Hiển thị dữ liệu dòng hiện tại
            hienThiDuLieuDong(idrow);
        }
        private void ClearInputFields()
        {
            txtMaPhieu.Text = "";
            txtIDSach.Text = "";
            txtIDBanDoc.Text = "";
            txtSoLuong.Text = "";
            txtTrangThai.Text = "";
            dtpNgayDK.Value = DateTime.Today.Date;
            dtpHanTra.Value = DateTime.Today.Date;
            dtpNgayMuon.Value = DateTime.Today.Date;
        }
        private void hienThiDuLieuDong(int idrow)
        {
            using (DatabaseDataContext dp = new DatabaseDataContext())
            {
                // Lấy mã phiếu từ dòng được chọn
                string madg = dgvMuon.Rows[idrow].Cells[0].Value.ToString();
                PhieuMuon mt = dp.PhieuMuons.SingleOrDefault(p => p.MaPhieu.ToString() == madg);
                DataGridViewRow row = dgvMuon.Rows[idrow];
                // Nếu có phiếu mượn, hiển thị thông tin lên các ô nhập liệu
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
                    dtpHanTra.Text = mt.HanTra.ToString();
                    dtpNgayMuon.Text = mt.NgayMuon.ToString();
                }
                else
                {
                    ClearInputFields(); // Nếu không có phiếu mượn, làm trống các ô nhập liệu
                }
                if (txtTrangThai.Text == "Đã Trả")
                {
                    btnTraSach.Enabled = false;
                }
                else
                {
                    btnTraSach.Enabled = true;
                }
            }
        }

        private void btnGhiPhieu_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmPhieuDK phieuDKForm = new frmPhieuDK();
            phieuDKForm.ShowDialog();
            this.Show();
            LoadDuLieu();
        }

        private void rbDSTra_CheckedChanged(object sender, EventArgs e)
        {
            if (rdDSTra.Checked)
            {
                Value = "Trả";
                LoadDuLieu();
            }
        }

        private void rbDSMuon_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDSMuon.Checked)
            {
                Value = "Mượn";
                LoadDuLieu();
            }
        }

        private void btnTraSach_Click(object sender, EventArgs e)
        {
            //string maPhieu = txtMaPhieu.Text;

            /* Kiểm tra nếu mã phiếu rỗng
            if (string.IsNullOrEmpty(maPhieu))
            {
                MessageBox.Show("Vui lòng chọn một phiếu mượn để trả sách.");
                return;
            }

            using (DatabaseDataContext dp = new DatabaseDataContext())
            {
                // Tìm phiếu mượn theo mã phiếu
                PhieuMuon phieuMuon = dp.PhieuMuons.SingleOrDefault(p => p.MaPhieu.ToString() == maPhieu);

                // Nếu tìm thấy phiếu mượn
                if (phieuMuon != null)
                {
                    // Cập nhật ngày trả và trạng thái phiếu
                    phieuMuon.NgayTra = DateTime.Today.Date;
                    phieuMuon.TrangThai = 3; // Trạng thái 3 là "Đã trả"

                    // Lưu thay đổi vào cơ sở dữ liệu
                    dp.SubmitChanges();

                    // Cập nhật lại giao diện
                    LoadDuLieu(); // Tải lại dữ liệu để cập nhật bảng
                    ClearInputFields(); // Xóa các ô nhập liệu
                    MessageBox.Show("Đã trả sách thành công.");
                }
            }*/

            if (string.IsNullOrEmpty(txtMaPhieu.Text))
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn!");
                return;
            }

            int maPhieu;
            if (!int.TryParse(txtMaPhieu.Text, out maPhieu))
            {
                MessageBox.Show("Mã phiếu không hợp lệ!");
                return;
            }

            using (var dp = new DatabaseDataContext())
            {
                var phieu = dp.PhieuMuons.SingleOrDefault(p => p.MaPhieu == maPhieu);
                if (phieu == null || phieu.TrangThai == 3)
                {
                    MessageBox.Show("Phiếu không tồn tại hoặc đã trả!");
                    return;
                }

                try
                {
                    // Cập nhật phiếu
                    phieu.NgayTra = DateTime.Today;
                    phieu.TrangThai = 3;

                    // CẬP NHẬT SỐ LƯỢNG SÁCH
                    var sach = dp.Saches.SingleOrDefault(s => s.ID == phieu.IDSach);
                    if (sach != null)
                    {
                        sach.SoLuong += phieu.SoLuong;
                    }

                    dp.SubmitChanges();
                    LoadDuLieu();
                    ClearInputFields();
                    MessageBox.Show("Trả sách thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnHead_Click(object sender, EventArgs e)
        {
            if (dgvMuon.Rows.Count > 0)
            {
                dgvMuon.CurrentCell = dgvMuon.Rows[0].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (dgvMuon.Rows.Count > 0)
            {
                int currentIndex = dgvMuon.CurrentCell.RowIndex;
                if (currentIndex > 0)
                {
                    dgvMuon.CurrentCell = dgvMuon.Rows[currentIndex - 1].Cells[0];
                }
                else
                {
                    MessageBox.Show("Đây là dòng đầu tiên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (dgvMuon.Rows.Count > 0)
            {
                int currentIndex = dgvMuon.CurrentCell.RowIndex;
                if (currentIndex < dgvMuon.Rows.Count - 1)
                {
                    dgvMuon.CurrentCell = dgvMuon.Rows[currentIndex + 1].Cells[0];
                }
                else
                {
                    MessageBox.Show("Đây là dòng cuối cùng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTail_Click(object sender, EventArgs e)
        {
            if (dgvMuon.Rows.Count > 0)
            {
                dgvMuon.CurrentCell = dgvMuon.Rows[dgvMuon.Rows.Count - 1].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            frmAdmin frmAd = new frmAdmin(id);
            frmAd.Show();
            this.Close();
        }

        private void btnINDS_Click(object sender, EventArgs e)
        {
            int loaiPhieu;
            if (cbbKieuIn.Text == "Đang Mượn")
            {
                loaiPhieu = 1;
            }
            else if (cbbKieuIn.Text == "Quá Hạn")
            {
                loaiPhieu = 3;
            }
            else
            {
                loaiPhieu = 2;
            }
            frmINDSPhieuMuon frmINDSMuon = new frmINDSPhieuMuon(tenNI, loaiPhieu);
            frmINDSMuon.Show();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maTimKiem = txtMaTimKiem.Text.Trim(); // Lấy giá trị từ ô nhập tìm kiếm
            if (!string.IsNullOrEmpty(maTimKiem))
            {
                foreach (DataGridViewRow row in dgvMuon.Rows)
                {
                    if (row.Cells["MaPhieu"].Value.ToString().Equals(maTimKiem, StringComparison.OrdinalIgnoreCase))
                    {
                        // Chọn dòng tìm thấy và đặt focus cho DataGridView
                        dgvMuon.ClearSelection();
                        row.Selected = true;
                        dgvMuon.CurrentCell = row.Cells[0];
                        return;
                    }
                }

                // Nếu không tìm thấy kết quả, có thể hiển thị thông báo
                MessageBox.Show("Không tìm thấy mã phiếu trong danh sách.");
            }
            else
            {
                // Nếu không có giá trị tìm kiếm
                MessageBox.Show("Vui lòng nhập mã phiếu tìm kiếm.");
            }
        }

        private void btnInPhieu_Click(object sender, EventArgs e)
        {
            string maPhieu = txtMaPhieu.Text.Trim();

            // Kiểm tra nếu MaPhieu trống
            if (string.IsNullOrEmpty(maPhieu))
            {
                MessageBox.Show("Vui lòng chọn phiếu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở form In Phiếu Chọn với MaPhieu
            frmINPhieuChon frm = new frmINPhieuChon(id, maPhieu);
            frm.Show();
        }
    }
}
