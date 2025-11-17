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
    public partial class frmNhapTL : Form
    {
        public frmNhapTL()
        {
            InitializeComponent();
        }
        #region Dữ liệu
        private void frmNhapTL_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int tongTL = db.TheLoais.Count();
            lbTong.Text = $"Tổng thể loại có trong thư viện: {tongTL} - Thể loại";
            loadDuLieu();
        }
        private void loadDuLieu()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var data = db.TheLoais.OrderBy(p => p.MaTheLoai).Select(p => new
            {
                MaTheLoai=p.MaTheLoai,
                TenTheLoai=p.TenTheLoai,
                MoTa=p.MoTa,
                SoSach=p.SoSach,
            }).ToList();
            dgvTheLoai.DataSource = data;
            if (data.Count > 0)
            {
                hienThiDuLieu(0);
            }
        }

        private void dgvTheLoai_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int idrow = e.RowIndex;
            if (idrow == -1) return;
            hienThiDuLieu(idrow);
        }
        private void hienThiDuLieu(int idrow)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int matl = Convert.ToInt32(dgvTheLoai.Rows[idrow].Cells[0].Value);
            //hien thi du lieu
            TheLoai theloai = db.TheLoais.Where(p => p.MaTheLoai == matl).SingleOrDefault();
            if (theloai != null)
            {
                txtTenTL.Text = theloai.TenTheLoai;
                txtMoTa.Text = theloai.MoTa;
                txtSoSach.Text = theloai.SoSach.ToString();

            }
        }

        #endregion

        private void btnDau_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.Rows.Count > 0)
            {
                dgvTheLoai.CurrentCell = dgvTheLoai.Rows[0].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.Rows.Count > 0)
            {
                int currentIndex = dgvTheLoai.CurrentCell.RowIndex;
                if (currentIndex > 0)
                {
                    dgvTheLoai.CurrentCell = dgvTheLoai.Rows[currentIndex - 1].Cells[0];
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

        private void btnTiep_Click(object sender, EventArgs e)
        {
            if(dgvTheLoai.Rows.Count > 0)
            {
                int currentIndex = dgvTheLoai.CurrentCell.RowIndex;
                if (currentIndex < dgvTheLoai.Rows.Count - 1)
                {
                    dgvTheLoai.CurrentCell = dgvTheLoai.Rows[currentIndex + 1].Cells[0];
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

        private void btnCuoi_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.Rows.Count > 0)
            {
                dgvTheLoai.CurrentCell = dgvTheLoai.Rows[dgvTheLoai.Rows.Count - 1].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTenTL.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                bool found = false;

                // Duyệt qua các dòng hiện có trong DataGridView
                foreach (DataGridViewRow row in dgvTheLoai.Rows)
                {
                    if (row.Cells["TenTheLoai"].Value != null && row.Cells["TenTheLoai"].Value.ToString().Contains(keyword))
                    {
                        // Chọn dòng nếu tìm thấy
                        dgvTheLoai.ClearSelection();
                        row.Selected = true;
                        dgvTheLoai.FirstDisplayedScrollingRowIndex = row.Index; // Cuộn đến dòng được chọn
                        found = true;
                        break;
                    }
                }

                // Hiển thị thông báo kết quả
                if (!found)
                {
                    MessageBox.Show("Không tìm thấy thể loại nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.SelectedRows.Count > 0)
            {
                int maTheLoai = Convert.ToInt32(dgvTheLoai.SelectedRows[0].Cells[0].Value);
                DatabaseDataContext db = new DatabaseDataContext();
                var sachLienQuan = db.Saches.Where(s => s.MaTheLoai == maTheLoai).ToList();
                if (sachLienQuan.Count > 0)
                {
                    MessageBox.Show(
                        "Thể loại này có sách liên quan. Bạn cần xóa sách trước khi xóa thể loại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Xác nhận xóa nếu không có sách liên quan
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa thể loại này không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    var theLoai = db.TheLoais.SingleOrDefault(p => p.MaTheLoai == maTheLoai);
                    if (theLoai != null)
                    {
                        db.TheLoais.DeleteOnSubmit(theLoai);

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("Xóa thể loại thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadDuLieu();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn thể loại cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string tenTheLoai = txtTenTL.Text.Trim();
            string moTa = txtMoTa.Text.Trim();
            int soSach = int.TryParse(txtSoSach.Text.Trim(), out int ss) ? ss : 0;

            if (!string.IsNullOrEmpty(tenTheLoai))
            {
                DatabaseDataContext db = new DatabaseDataContext();

                TheLoai newTheLoai = new TheLoai
                {
                    TenTheLoai = tenTheLoai,
                    MoTa = moTa,
                    SoSach = soSach
                };
                db.TheLoais.InsertOnSubmit(newTheLoai);
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("Thêm thể loại thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadDuLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Tên thể loại không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvTheLoai.SelectedRows.Count > 0)
            {
                int maTheLoai = Convert.ToInt32(dgvTheLoai.SelectedRows[0].Cells[0].Value);
                string tenTheLoai = txtTenTL.Text.Trim();
                string moTa = txtMoTa.Text.Trim();
                int soSach = int.TryParse(txtSoSach.Text.Trim(), out int ss) ? ss : 0;

                if (!string.IsNullOrEmpty(tenTheLoai))
                {
                    DatabaseDataContext db = new DatabaseDataContext();
                    var theloai = db.TheLoais.SingleOrDefault(p => p.MaTheLoai == maTheLoai);

                    if (theloai != null)
                    {
                        theloai.TenTheLoai = tenTheLoai;
                        theloai.MoTa = moTa;
                        theloai.SoSach = soSach;

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("Cập nhật thể loại thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadDuLieu();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Tên thể loại không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
