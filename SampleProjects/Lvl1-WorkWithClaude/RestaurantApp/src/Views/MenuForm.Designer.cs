namespace RestaurantApp.Views;

partial class MenuForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox txtSearch;
    private System.Windows.Forms.CheckBox chkOnlyAvailable;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnClearSearch;
    private System.Windows.Forms.DataGridView gridDishes;
    private System.Windows.Forms.Label lblCount;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.TextBox txtCategory;
    private System.Windows.Forms.TextBox txtPrice;
    private System.Windows.Forms.CheckBox chkAvailable;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnDelete;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        txtSearch = new System.Windows.Forms.TextBox();
        chkOnlyAvailable = new System.Windows.Forms.CheckBox();
        btnSearch = new System.Windows.Forms.Button();
        btnClearSearch = new System.Windows.Forms.Button();
        gridDishes = new System.Windows.Forms.DataGridView();
        lblCount = new System.Windows.Forms.Label();
        txtName = new System.Windows.Forms.TextBox();
        txtCategory = new System.Windows.Forms.TextBox();
        txtPrice = new System.Windows.Forms.TextBox();
        chkAvailable = new System.Windows.Forms.CheckBox();
        btnAdd = new System.Windows.Forms.Button();
        btnUpdate = new System.Windows.Forms.Button();
        btnDelete = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)gridDishes).BeginInit();
        SuspendLayout();

        // --- Hàng tìm kiếm ---
        txtSearch.Location = new System.Drawing.Point(12, 12);
        txtSearch.Size = new System.Drawing.Size(240, 23);
        txtSearch.PlaceholderText = "Tìm theo tên hoặc danh mục...";

        chkOnlyAvailable.Location = new System.Drawing.Point(262, 14);
        chkOnlyAvailable.Size = new System.Drawing.Size(120, 20);
        chkOnlyAvailable.Text = "Chỉ món đang bán";

        btnSearch.Location = new System.Drawing.Point(388, 11);
        btnSearch.Size = new System.Drawing.Size(80, 25);
        btnSearch.Text = "Tìm kiếm";
        btnSearch.Click += OnSearchClicked;

        btnClearSearch.Location = new System.Drawing.Point(474, 11);
        btnClearSearch.Size = new System.Drawing.Size(80, 25);
        btnClearSearch.Text = "Xoá lọc";
        btnClearSearch.Click += OnClearSearchClicked;

        // --- Lưới món ---
        gridDishes.Location = new System.Drawing.Point(12, 44);
        gridDishes.Size = new System.Drawing.Size(542, 240);
        gridDishes.AllowUserToAddRows = false;
        gridDishes.ReadOnly = true;
        gridDishes.MultiSelect = false;
        gridDishes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        gridDishes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        gridDishes.ColumnCount = 5;
        gridDishes.Columns[0].HeaderText = "Id";
        gridDishes.Columns[1].HeaderText = "Tên món";
        gridDishes.Columns[2].HeaderText = "Danh mục";
        gridDishes.Columns[3].HeaderText = "Đơn giá";
        gridDishes.Columns[4].HeaderText = "Đang bán";
        gridDishes.SelectionChanged += OnGridSelectionChanged;

        lblCount.Location = new System.Drawing.Point(12, 288);
        lblCount.Size = new System.Drawing.Size(120, 18);
        lblCount.Text = "0 món";

        // --- Khu nhập liệu ---
        txtName.Location = new System.Drawing.Point(12, 312);
        txtName.Size = new System.Drawing.Size(180, 23);
        txtName.PlaceholderText = "Tên món";

        txtCategory.Location = new System.Drawing.Point(198, 312);
        txtCategory.Size = new System.Drawing.Size(140, 23);
        txtCategory.PlaceholderText = "Danh mục";

        txtPrice.Location = new System.Drawing.Point(344, 312);
        txtPrice.Size = new System.Drawing.Size(100, 23);
        txtPrice.PlaceholderText = "Đơn giá";

        chkAvailable.Location = new System.Drawing.Point(452, 314);
        chkAvailable.Size = new System.Drawing.Size(100, 20);
        chkAvailable.Text = "Đang bán";
        chkAvailable.Checked = true;

        // --- Nút quản lý ---
        btnAdd.Location = new System.Drawing.Point(12, 344);
        btnAdd.Size = new System.Drawing.Size(120, 28);
        btnAdd.Text = "Thêm món";
        btnAdd.Click += OnAddClicked;

        btnUpdate.Location = new System.Drawing.Point(138, 344);
        btnUpdate.Size = new System.Drawing.Size(120, 28);
        btnUpdate.Text = "Cập nhật";
        btnUpdate.Click += OnUpdateClicked;

        btnDelete.Location = new System.Drawing.Point(264, 344);
        btnDelete.Size = new System.Drawing.Size(120, 28);
        btnDelete.Text = "Xoá món";
        btnDelete.Click += OnDeleteClicked;

        // --- Form ---
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(566, 386);
        Controls.Add(txtSearch);
        Controls.Add(chkOnlyAvailable);
        Controls.Add(btnSearch);
        Controls.Add(btnClearSearch);
        Controls.Add(gridDishes);
        Controls.Add(lblCount);
        Controls.Add(txtName);
        Controls.Add(txtCategory);
        Controls.Add(txtPrice);
        Controls.Add(chkAvailable);
        Controls.Add(btnAdd);
        Controls.Add(btnUpdate);
        Controls.Add(btnDelete);
        Text = "RestaurantApp — Quản lý & Tìm kiếm món";

        ((System.ComponentModel.ISupportInitialize)gridDishes).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
