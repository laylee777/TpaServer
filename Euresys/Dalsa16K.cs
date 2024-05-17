using Euresys.MultiCam;
using System;
using System.Diagnostics;
using System.IO;
using DSEV.Schemas;

namespace DSEV.Multicam
{
    public class Dalsa16K : CamControl
    {
        public override 카메라구분 Camera { get; set; } = 카메라구분.None;
        public override String CamFile { get; set; } = "TopCameraConfig.cam";
        public override UInt32 DriverIndex { get; set; } = 0;
        public override AcquisitionMode AcquisitionMode { get; set; } = AcquisitionMode.PAGE;
        public override LineRateMode LineRateMode { get; set; } = LineRateMode.PULSE;
        public override TrigMode TrigMode { get; set; } = TrigMode.HARD;
        public override NextTrigMode NextTrigMode { get; set; } = NextTrigMode.HARD;
        public override EndTrigMode EndTrigMode { get; set; } = EndTrigMode.AUTO;
        public override Int32 SeqLength_Pg { get; set; } = 1;
        public override Int32 PageLength_Ln { get; set; } = 40000;

        public Dalsa16K(카메라구분 camera)
        {
            this.Camera = camera;
        }

        public override void Init()
        {
            base.Init();
            String camfile = Path.Combine(Global.환경설정.기본경로, this.CamFile);
            MC.Create("CHANNEL", out this.Channel);
            MC.SetParam(this.Channel, "DriverIndex", this.DriverIndex);
            MC.SetParam(this.Channel, "Connector", this.Connector.ToString());
            MC.SetParam(this.Channel, "CamFile", camfile);
        }
    }
}
