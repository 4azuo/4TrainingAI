namespace RestaurantApp.Views;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.ComboBox cboMenu;
    private System.Windows.Forms.NumericUpDown numQuantity;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.ListBox lstOrder;
    private System.Windows.Forms.Label lblTotal;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        cboMenu = new System.Windows.Forms.ComboBox();
        numQuantity = new System.Windows.Forms.NumericUpDown();
        btnAdd = new System.Windows.Forms.Button();
        lstOrder = new System.Windows.Forms.ListBox();
        lblTotal = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
        SuspendLayout();

        // cboMenu
        cboMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cboMenu.Location = new System.Drawing.Point(12, 12);
        cboMenu.Size = new System.Drawing.Size(200, 23);

        // numQuantity
        numQuantity.Location = new System.Drawing.Point(220, 12);
        numQuantity.Minimum = 1;
        numQuantity.Maximum = 99;
        numQuantity.Value = 1;
        numQuantity.Size = new System.Drawing.Size(60, 23);

        // btnAdd
        btnAdd.Location = new System.Drawing.Point(290, 11);
        btnAdd.Size = new System.Drawing.Size(90, 25);
        btnAdd.Text = "Thêm món";
        btnAdd.Click += OnAddClicked;

        // lstOrder
        lstOrder.Location = new System.Drawing.Point(12, 48);
        lstOrder.Size = new System.Drawing.Size(368, 180);

        // lblTotal
        lblTotal.Location = new System.Drawing.Point(12, 236);
        lblTotal.Size = new System.Drawing.Size(368, 28);
        lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        lblTotal.Text = "Tổng: 0 đ";

        // MainForm
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(392, 280);
        Controls.Add(cboMenu);
        Controls.Add(numQuantity);
        Controls.Add(btnAdd);
        Controls.Add(lstOrder);
        Controls.Add(lblTotal);
        Text = "RestaurantApp — Gọi món";

        ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
        ResumeLayout(false);
    }
}
