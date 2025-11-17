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
    public partial class frmNhapNXB : Form
    {
        public frmNhapNXB()
        {
            InitializeComponent();
        }
        #region Dữ liệu
        private void frmNhapNXB_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int tongNXB= db.TacGias.Count();
            lbTong.Text = $"Tổng NXB có trong thư viện: {tongNXB} - NXB";
            loadDuLieu();
        }
        private void loadDuLieu()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var data = db.NhaXuatBans.OrderBy(p => p.MaNXB).Select(p => new
            {
                MaNXB = p.MaNXB,
                TenNXB = p.TenNXB,
                MoTa=p.MoTa,
            }).ToList();
            dgvNXB.DataSource = data;
            if (data.Count > 0)
            {
                hienThiDuLieu(0);
            }
        }

        private void dgvNXB_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //xac dinh row dang chon
            int idrow = e.RowIndex;
            if (idrow == -1) return;
            hienThiDuLieu(idrow);
        }
        private void hienThiDuLieu(int idrow)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            int maxb = Convert.ToInt32(dgvNXB.Rows[idrow].Cells[0].Value);
            //hien thi du lieu
            NhaXuatBan nxb = db.NhaXuatBans.Where(p => p.MaNXB == maxb).SingleOrDefault();
            if (nxb != null)
            {
                
                txtTenNXB.Text = nxb.TenNXB;
                txtMoTa.Text = nxb.MoTa;

            }
        }
        #endregion
        #region Chức năng
        private void btnDau_Click(object sender, EventArgs e)
        {
            if (dgvNXB.Rows.Count > 0)
            {
                dgvNXB.CurrentCell = dgvNXB.Rows[0].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            if (dgvNXB.Rows.Count > 0)
            {
                int currentIndex = dgvNXB.CurrentCell.RowIndex;
                if (currentIndex > 0)
                {
                    dgvNXB.CurrentCell = dgvNXB.Rows[currentIndex - 1].Cells[0];
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
            if (dgvNXB.Rows.Count > 0)
            {
                int currentIndex = dgvNXB.CurrentCell.RowIndex;
                if (currentIndex < dgvNXB.Rows.Count - 1)
                {
                    dgvNXB.CurrentCell = dgvNXB.Rows[currentIndex + 1].Cells[0];
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
            if (dgvNXB.Rows.Count > 0)
            {
                dgvNXB.CurrentCell = dgvNXB.Rows[dgvNXB.Rows.Count - 1].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            string keyword = txtTenNXB.Text.Trim();

            if (!string.IsNullOrEmpty(keyword))
            {
                //var result = db.NhaXuatBans
                //              .Where(p => p.TenNXB.Contains(keyword))
                //              .Select(p => new
                //              {
                //                  MaNXB = p.MaNXB,
                //                  TenNXB = p.TenNXB,
                //                  MoTa = p.MoTa
                //              }).ToList();

                //if (result.Count > 0)
                //{
                //    dgvNXB.DataSource = result;
                //}
                //else
                //{
                //    MessageBox.Show("Không tìm thấy nhà xuất bản nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                bool found = false;

                foreach (DataGridViewRow row in dgvNXB.Rows)
                {
                    // Kiểm tra nếu cột "TenNXB" chứa từ khóa
                    if (row.Cells["TenNXB"].Value != null && row.Cells["TenNXB"].Value.ToString().Contains(keyword))
                    {
                        // Bỏ chọn tất cả các dòng trước
                        dgvNXB.ClearSelection();

                        // Chọn dòng và đặt con trỏ tại ô đầu tiên của dòng
                        row.Selected = true;
                        dgvNXB.CurrentCell = row.Cells[0];

                        // Cuộn DataGridView để dòng được chọn nằm trong tầm nhìn
                        dgvNXB.FirstDisplayedScrollingRowIndex = row.Index;

                        found = true;
                        break; // Dừng vòng lặp sau khi tìm thấy dòng
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Không tìm thấy nhà xuất bản nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvNXB.SelectedRows.Count > 0)
            {
                int maNXB = Convert.ToInt32(dgvNXB.SelectedRows[0].Cells[0].Value);
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa NXB này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DatabaseDataContext db = new DatabaseDataContext();
                    var nxb = db.NhaXuatBans.SingleOrDefault(p => p.MaNXB == maNXB);

                    if (nxb != null)
                    {
                        try
                        {
                            var checkNXB = db.Saches.Where(pm => pm.MaNXB == nxb.MaNXB).FirstOrDefault();
                            if (checkNXB != null)
                            {
                                // Nếu sách đang bị mượn, không cho phép xóa sách này, nhưng vẫn tiếp tục kiểm tra các sách còn lại
                                MessageBox.Show($"Không thể xóa nhà xuất bản {nxb.TenNXB} vi đang có sách của nhà xuất bản trong thư viện.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            db.NhaXuatBans.DeleteOnSubmit(nxb);
                            db.SubmitChanges();
                            int tongNXB = db.TacGias.Count();
                            lbTong.Text = $"Tổng NXB có trong thư viện: {tongNXB} - NXB";
                            MessageBox.Show("Xóa NXB thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    return;
                }
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            string tenNXB = txtTenNXB.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            if (!string.IsNullOrEmpty(tenNXB))
            {
                DatabaseDataContext db = new DatabaseDataContext();

                NhaXuatBan newNXB = new NhaXuatBan
                {
                    TenNXB = tenNXB,
                    MoTa = moTa
                };

                db.NhaXuatBans.InsertOnSubmit(newNXB);

                try
                {
                    db.SubmitChanges();
                    int tongNXB = db.TacGias.Count();
                    lbTong.Text = $"Tổng NXB có trong thư viện: {tongNXB} - NXB";
                    MessageBox.Show("Thêm NXB thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadDuLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Tên NXB không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNXB.SelectedRows.Count > 0)
            {
                int maNXB = Convert.ToInt32(dgvNXB.SelectedRows[0].Cells[0].Value);
                string tenNXB = txtTenNXB.Text.Trim();
                string moTa = txtMoTa.Text.Trim();

                if (!string.IsNullOrEmpty(tenNXB))
                {
                    DatabaseDataContext db = new DatabaseDataContext();
                    var nxb = db.NhaXuatBans.SingleOrDefault(p => p.MaNXB == maNXB);

                    if (nxb != null)
                    {
                        nxb.TenNXB = tenNXB;
                        nxb.MoTa = moTa;

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("Cập nhật NXB thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Tên NXB không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
