namespace DSEV.UI.Forms
{
    partial class VMForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
            this.vmMainViewConfigControl1 = new VMControls.Winform.Release.VmMainViewConfigControl();
            this.vmGlobalToolControl1 = new VMControls.Winform.Release.VmGlobalToolControl();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 5F)});
            this.tablePanel1.Controls.Add(this.vmMainViewConfigControl1);
            this.tablePanel1.Controls.Add(this.vmGlobalToolControl1);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 0);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Padding = new System.Windows.Forms.Padding(1);
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 75F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
            this.tablePanel1.Size = new System.Drawing.Size(1091, 803);
            this.tablePanel1.TabIndex = 2;
            this.tablePanel1.UseSkinIndents = true;
            // 
            // vmMainViewConfigControl1
            // 
            this.tablePanel1.SetColumn(this.vmMainViewConfigControl1, 0);
            this.vmMainViewConfigControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vmMainViewConfigControl1.Location = new System.Drawing.Point(3, 78);
            this.vmMainViewConfigControl1.Margin = new System.Windows.Forms.Padding(2);
            this.vmMainViewConfigControl1.Name = "vmMainViewConfigControl1";
            this.tablePanel1.SetRow(this.vmMainViewConfigControl1, 1);
            this.vmMainViewConfigControl1.Size = new System.Drawing.Size(1085, 722);
            this.vmMainViewConfigControl1.TabIndex = 1;
// TODO: '기본 형식이 잘못되었습니다. System.IntPtr. CodeObjectCreateExpression을 사용하십시오.' 예외가 발생하여 ''의 코드를 생성하지 못했습니다.
            // 
            // vmGlobalToolControl1
            // 
            this.tablePanel1.SetColumn(this.vmGlobalToolControl1, 0);
            this.vmGlobalToolControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vmGlobalToolControl1.Location = new System.Drawing.Point(5, 5);
            this.vmGlobalToolControl1.Margin = new System.Windows.Forms.Padding(4);
            this.vmGlobalToolControl1.Name = "vmGlobalToolControl1";
            this.tablePanel1.SetRow(this.vmGlobalToolControl1, 0);
            this.vmGlobalToolControl1.Size = new System.Drawing.Size(1081, 67);
            this.vmGlobalToolControl1.TabIndex = 0;
            // 
            // VMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 803);
            this.Controls.Add(this.tablePanel1);
            this.Name = "VMForm";
            this.Text = "VMForm";
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Layout.TablePanel tablePanel1;
        private VMControls.Winform.Release.VmMainViewConfigControl vmMainViewConfigControl1;
        private VMControls.Winform.Release.VmGlobalToolControl vmGlobalToolControl1;
    }
}