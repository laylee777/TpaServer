namespace DSEV.UI.Controls
{
    partial class DeviceLamp
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceLamp));
            this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
            this.eMES통신 = new DevExpress.XtraEditors.SvgImageBox();
            this.e조명장치 = new DevExpress.XtraEditors.SvgImageBox();
            this.e카메라1 = new DevExpress.XtraEditors.SvgImageBox();
            this.e각인리더 = new DevExpress.XtraEditors.SvgImageBox();
            this.e장치통신 = new DevExpress.XtraEditors.SvgImageBox();
            this.e통신체크 = new DevExpress.XtraEditors.SvgImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eMES통신)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.e조명장치)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.e카메라1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.e각인리더)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.e장치통신)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.e통신체크)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F)});
            this.tablePanel1.Controls.Add(this.eMES통신);
            this.tablePanel1.Controls.Add(this.e조명장치);
            this.tablePanel1.Controls.Add(this.e카메라1);
            this.tablePanel1.Controls.Add(this.e각인리더);
            this.tablePanel1.Controls.Add(this.e장치통신);
            this.tablePanel1.Controls.Add(this.e통신체크);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 0);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Padding = new System.Windows.Forms.Padding(1);
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F)});
            this.tablePanel1.Size = new System.Drawing.Size(200, 94);
            this.tablePanel1.TabIndex = 0;
            this.tablePanel1.UseSkinIndents = true;
            // 
            // eMES통신
            // 
            this.tablePanel1.SetColumn(this.eMES통신, 0);
            this.eMES통신.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eMES통신.Location = new System.Drawing.Point(4, 49);
            this.eMES통신.Name = "eMES통신";
            this.tablePanel1.SetRow(this.eMES통신, 1);
            this.eMES통신.Size = new System.Drawing.Size(45, 41);
            this.eMES통신.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            this.eMES통신.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("eMES통신.SvgImage")));
            this.eMES통신.TabIndex = 17;
            this.eMES통신.Text = "MES";
            this.eMES통신.ToolTip = "MES";
            // 
            // e조명장치
            // 
            this.tablePanel1.SetColumn(this.e조명장치, 3);
            this.e조명장치.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e조명장치.Location = new System.Drawing.Point(151, 4);
            this.e조명장치.Name = "e조명장치";
            this.tablePanel1.SetRow(this.e조명장치, 0);
            this.e조명장치.Size = new System.Drawing.Size(45, 41);
            this.e조명장치.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            this.e조명장치.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("e조명장치.SvgImage")));
            this.e조명장치.TabIndex = 16;
            this.e조명장치.Text = "Lights";
            this.e조명장치.ToolTip = "Lights";
            // 
            // e카메라1
            // 
            this.tablePanel1.SetColumn(this.e카메라1, 1);
            this.e카메라1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e카메라1.Location = new System.Drawing.Point(53, 4);
            this.e카메라1.Name = "e카메라1";
            this.tablePanel1.SetRow(this.e카메라1, 0);
            this.e카메라1.Size = new System.Drawing.Size(45, 41);
            this.e카메라1.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            this.e카메라1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("e카메라1.SvgImage")));
            this.e카메라1.TabIndex = 16;
            this.e카메라1.Text = "Camera";
            this.e카메라1.ToolTip = "Camera";
            // 
            // e각인리더
            // 
            this.tablePanel1.SetColumn(this.e각인리더, 2);
            this.e각인리더.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e각인리더.Location = new System.Drawing.Point(102, 4);
            this.e각인리더.Name = "e각인리더";
            this.tablePanel1.SetRow(this.e각인리더, 0);
            this.e각인리더.Size = new System.Drawing.Size(45, 41);
            this.e각인리더.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            this.e각인리더.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("e각인리더.SvgImage")));
            this.e각인리더.TabIndex = 13;
            this.e각인리더.Text = "QR Reader (V430-F)";
            this.e각인리더.ToolTip = "QR Reader (V430-F)";
            // 
            // e장치통신
            // 
            this.tablePanel1.SetColumn(this.e장치통신, 0);
            this.e장치통신.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e장치통신.Location = new System.Drawing.Point(4, 4);
            this.e장치통신.Name = "e장치통신";
            this.tablePanel1.SetRow(this.e장치통신, 0);
            this.e장치통신.Size = new System.Drawing.Size(45, 41);
            this.e장치통신.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Squeeze;
            this.e장치통신.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("e장치통신.SvgImage")));
            this.e장치통신.TabIndex = 10;
            this.e장치통신.Text = "PLC";
            this.e장치통신.ToolTip = "PLC";
            // 
            // e통신체크
            // 
            this.e통신체크.Dock = System.Windows.Forms.DockStyle.Fill;
            this.e통신체크.Location = new System.Drawing.Point(13, 12);
            this.e통신체크.Name = "e통신체크";
            this.e통신체크.Size = new System.Drawing.Size(32, 33);
            this.e통신체크.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.e통신체크.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("e통신체크.SvgImage")));
            this.e통신체크.TabIndex = 9;
            this.e통신체크.Text = "svgImageBox1";
            this.e통신체크.ToolTip = "PLC State";
            // 
            // DeviceLamp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tablePanel1);
            this.Name = "DeviceLamp";
            this.Size = new System.Drawing.Size(200, 94);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.eMES통신)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.e조명장치)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.e카메라1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.e각인리더)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.e장치통신)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.e통신체크)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Layout.TablePanel tablePanel1;
        private DevExpress.XtraEditors.SvgImageBox e통신체크;
        private DevExpress.XtraEditors.SvgImageBox e각인리더;
        private DevExpress.XtraEditors.SvgImageBox e장치통신;
        private DevExpress.XtraEditors.SvgImageBox e카메라1;
        private DevExpress.XtraEditors.SvgImageBox e조명장치;
        private DevExpress.XtraEditors.SvgImageBox eMES통신;
    }
}
