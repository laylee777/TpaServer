using DevExpress.XtraEditors;
using IVM.Schemas;

using System;



namespace IVM.UI.Controls
{
    public partial class CamViewers : DevExpress.XtraEditors.XtraUserControl
    {
        public CamViewers() => InitializeComponent();

        public void Init()
        {
            this.e상부캠.Init(false);
            this.e왼쪽측면캠.Init(false);
            this.e오른쪽측면캠.Init(false);
            this.e왼쪽하부캠.Init(false);
            this.e오른쪽하부캠.Init(false);
            this.e하부커넥터캠.Init(false);
            this.e상부커넥터캠.Init(false);

            Global.비전검사.SetDisplay(카메라구분.Cam01, this.e상부캠);
            Global.비전검사.SetDisplay(카메라구분.Cam02, this.e왼쪽측면캠);
            Global.비전검사.SetDisplay(카메라구분.Cam03, this.e오른쪽측면캠);
            Global.비전검사.SetDisplay(카메라구분.Cam04, this.e왼쪽하부캠);
            Global.비전검사.SetDisplay(카메라구분.Cam05, this.e오른쪽하부캠);
            Global.비전검사.SetDisplay(카메라구분.Cam06, this.e하부커넥터캠);
            Global.비전검사.SetDisplay(카메라구분.Cam07, this.e상부커넥터캠);

            if (Global.VM제어.Count != 0)
            {
                this.e표면검사.ModuleSource = Global.VM제어.GetItem(Flow구분.표면검사).출력이미지;
            }
        }
        public void Close() { }
    }
}
