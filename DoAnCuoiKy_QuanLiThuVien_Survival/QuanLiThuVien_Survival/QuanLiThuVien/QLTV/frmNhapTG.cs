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
    public partial class frmNhapTG : Form
    {
        public frmNhapTG()
        {
            InitializeComponent();
        }
        #region Dữ liệu
        private void frmNhapTG_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int tongTG = db.TacGias.Count();
            lbTong.Text = $"Tổng số tác giả có trong thư viện: {tongTG} - Tác giả";
            loadDuLieu();
        }
        private void loadDuLieu()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var data = db.TacGias.OrderBy(p => p.MaTG).Select(p => new
            {
                MaTG = p.MaTG,
                TenTG=p.TenTG,
                MoTa=p.MoTa,
                SoSach=p.SoSach,
            }).ToList();
            dgvTacGia.DataSource = data;
            if (data.Count > 0)
            {
                hienThiDuLieu(0);
            }
        }
        private void dgvTacGia_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //xac dinh row dang chon
            int idrow = e.RowIndex;
            if (idrow == -1) return;
            hienThiDuLieu(idrow);
        }
        private void hienThiDuLieu(int idrow)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int matg = Convert.ToInt32(dgvTacGia.Rows[idrow].Cells[0].Value);
            //hien thi du lieu
            TacGia tacgia = db.TacGias.Where(p => p.MaTG == matg).SingleOrDefault();
            if (tacgia != null)
            {                
                txtTenTG.Text=tacgia.TenTG;          
                txtMoTa.Text=tacgia.MoTa;
                txtSoSach.Text = tacgia.SoSach.ToString();

            }
        }

        #endregion
        #region Chức năng
        private void btnDau_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.Rows.Count > 0)
            {
                dgvTacGia.CurrentCell = dgvTacGia.Rows[0].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.Rows.Count > 0)
            {
                int currentIndex = dgvTacGia.CurrentCell.RowIndex;
                if (currentIndex > 0)
                {
                    dgvTacGia.CurrentCell = dgvTacGia.Rows[currentIndex - 1].Cells[0];
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
            if (dgvTacGia.Rows.Count > 0)
            {
                int currentIndex = dgvTacGia.CurrentCell.RowIndex;
                if (currentIndex < dgvTacGia.Rows.Count - 1)
                {
                    dgvTacGia.CurrentCell = dgvTacGia.Rows[currentIndex + 1].Cells[0];
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
            if (dgvTacGia.Rows.Count > 0)
            {
                dgvTacGia.CurrentCell = dgvTacGia.Rows[dgvTacGia.Rows.Count - 1].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.SelectedRows.Count > 0)
            {
                int matg = Convert.ToInt32(dgvTacGia.SelectedRows[0].Cells[0].Value);
                DatabaseDataContext db = new DatabaseDataContext();
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sách này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Kiểm tra sách liên quan
                bool hasRelatedBooks = db.Saches.Any(s => s.MaTG == matg);
                if (hasRelatedBooks)
                {
                    MessageBox.Show("Không thể xóa tác giả này vì đang có sách liên quan. Vui lòng xử lý trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xóa tác giả
                var tacGia = db.TacGias.SingleOrDefault(t => t.MaTG == matg);
                if (tacGia != null)
                {
                    db.TacGias.DeleteOnSubmit(tacGia);
                    db.SubmitChanges();
                    int tongTG = db.TacGias.Count();
                    lbTong.Text = $"Tổng số tác giả có trong thư viện: {tongTG} - Tác giả";
                    MessageBox.Show("Xóa tác giả thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadDuLieu();
                }
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            string tenTG = txtTenTG.Text;
            string moTa = txtMoTa.Text;
            int soSach;

            if (!int.TryParse(txtSoSach.Text, out soSach) || soSach < 0)
            {
                MessageBox.Show("Số sách phải là số nguyên dương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tenTG))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin tác giả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DatabaseDataContext db = new DatabaseDataContext();
                TacGia newTacGia = new TacGia
                {
                    TenTG = tenTG,
                    MoTa = moTa,
                    SoSach = soSach
                };

                db.TacGias.InsertOnSubmit(newTacGia);
                db.SubmitChanges();
                int tongTG = db.TacGias.Count();
                lbTong.Text = $"Tổng số tác giả có trong thư viện: {tongTG} - Tác giả";

                MessageBox.Show("Thêm tác giả thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                loadDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            string tenTG = txtTenTG.Text;
            string moTa = txtMoTa.Text;
            int soSach;

            if (!int.TryParse(txtSoSach.Text, out soSach) || soSach < 0)
            {
                MessageBox.Show("Số sách phải là số nguyên dương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tenTG) || string.IsNullOrWhiteSpace(moTa))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin tác giả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvTacGia.SelectedRows.Count > 0)
            {
                int matg = Convert.ToInt32(dgvTacGia.SelectedRows[0].Cells[0].Value);
                try
                {
                    DatabaseDataContext db = new DatabaseDataContext();
                    TacGia tacgia = db.TacGias.Where(p => p.MaTG == matg).SingleOrDefault();
                    if (tacgia != null)
                    {
                        tacgia.TenTG = tenTG;
                        tacgia.MoTa = moTa;
                        tacgia.SoSach = soSach;

                        db.SubmitChanges();
                        MessageBox.Show("Cập nhật thông tin tác giả thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadDuLieu();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi sửa thông tin tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTenTG.Text.ToString();
            bool found = false;

            // Duyệt qua tất cả các dòng trong DataGridView
            foreach (DataGridViewRow row in dgvTacGia.Rows)
            {
                if (row.Cells["TenTG"].Value != null && row.Cells["TenTG"].Value.ToString().Contains(keyword))
                {
                    // Chọn dòng nếu tìm thấy
                    dgvTacGia.ClearSelection();
                    row.Selected = true;
                    dgvTacGia.FirstDisplayedScrollingRowIndex = row.Index; // Cuộn đến dòng được chọn
                    found = true;
                    break;
                }
            }

            // Hiển thị thông báo kết quả
            if (!found)
            {
                MessageBox.Show("Không tìm thấy tác giả nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Tìm thấy một tác giả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //DatabaseDataContext db = new DatabaseDataContext();
            //var data = db.TacGias
            //    .Where(p => p.TenTG.Contains(keyword))
            //    .OrderBy(p => p.MaTG)
            //    .Select(p => new
            //    {
            //        MaTG = p.MaTG,
            //        TenTG = p.TenTG,
            //        MoTa = p.MoTa,
            //        SoSach = p.SoSach,
            //    }).ToList();

            //dgvTacGia.DataSource = data;
            //if (data.Count == 0)
            //{
            //    MessageBox.Show("Không tìm thấy tác giả nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show($"Tìm thấy {data.Count} tác giả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }


        #endregion
    }
}
