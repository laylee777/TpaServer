using Newtonsoft.Json;
using System;

namespace DSEV.Schemas
{
    public partial class 조명제어
    {
        [JsonIgnore]
        private LCP200QC 상부조명컨트롤러;
        private LCP100DC 측면조명컨트롤러;
        private LCP100DC 하부조명컨트롤러;
        private LCP24_30Q 커넥터조명컨트롤러;

        [JsonIgnore]
        public Boolean 정상여부 => 측면조명컨트롤러.IsOpen() && 상부조명컨트롤러.IsOpen() && 하부조명컨트롤러.IsOpen() && 커넥터조명컨트롤러.IsOpen();

        public void Init()
        {
            this.상부조명컨트롤러 = new LCP200QC() { 포트 = 직렬포트.COM3 };
            this.측면조명컨트롤러 = new LCP100DC() { 포트 = 직렬포트.COM5, 통신속도 = 19200 };
            this.하부조명컨트롤러 = new LCP100DC() { 포트 = 직렬포트.COM4, 통신속도 = 19200 };
            this.커넥터조명컨트롤러 = new LCP24_30Q() { 포트 = 직렬포트.COM2 };

            this.상부조명컨트롤러.Init();
            this.측면조명컨트롤러.Init();
            this.하부조명컨트롤러.Init();
            this.커넥터조명컨트롤러.Init();

            this.Add(new 조명정보(카메라구분.Cam01, 상부조명컨트롤러) { 채널 = 조명채널.CH01, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam01, 상부조명컨트롤러) { 채널 = 조명채널.CH02, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam01, 상부조명컨트롤러) { 채널 = 조명채널.CH03, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam01, 상부조명컨트롤러) { 채널 = 조명채널.CH04, 밝기 = 100 });

            this.Add(new 조명정보(카메라구분.Cam02, 측면조명컨트롤러) { 채널 = 조명채널.CH01, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam02, 측면조명컨트롤러) { 채널 = 조명채널.CH02, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam03, 측면조명컨트롤러) { 채널 = 조명채널.CH03, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam03, 측면조명컨트롤러) { 채널 = 조명채널.CH04, 밝기 = 100 });
            
            this.Add(new 조명정보(카메라구분.Cam04, 하부조명컨트롤러) { 채널 = 조명채널.CH01, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam04, 하부조명컨트롤러) { 채널 = 조명채널.CH02, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam05, 하부조명컨트롤러) { 채널 = 조명채널.CH03, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam05, 하부조명컨트롤러) { 채널 = 조명채널.CH04, 밝기 = 100 });

            this.Add(new 조명정보(카메라구분.Cam06, 커넥터조명컨트롤러) { 채널 = 조명채널.CH01, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam06, 커넥터조명컨트롤러) { 채널 = 조명채널.CH03, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam07, 커넥터조명컨트롤러) { 채널 = 조명채널.CH02, 밝기 = 100 });
            this.Add(new 조명정보(카메라구분.Cam07, 커넥터조명컨트롤러) { 채널 = 조명채널.CH04, 밝기 = 100 });

            this.Load();
            if (Global.환경설정.동작구분 != 동작구분.Live) return;
            this.Open();
            this.Set();
        }
    }
}
