using MvUtils;
using DevExpress.XtraEditors;
using System;
using DSEV.Schemas;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Controls;
using System.Threading.Tasks;

namespace DSEV.UI.Controls
{
    public partial class DeviceSettings : XtraUserControl
    {
        private LocalizationDeviceSetting 번역 = new LocalizationDeviceSetting();
        public DeviceSettings()
        {
            InitializeComponent();
            this.BindLocalization.DataSource = this.번역;
        }

        public void Init()
        {
            this.e강제배출.IsOn = Global.환경설정.강제배출;
            this.e배출구분.IsOn = Global.환경설정.양품불량;
            this.e이미지자동삭제.IsOn = Global.환경설정.이미지자동삭제모드;
            this.e강제커버조립.IsOn = Global.환경설정.강제커버조립사용;
            this.e커버조립여부.IsOn = Global.환경설정.커버조립여부;
            this.eMES사용유무.IsOn = Global.환경설정.MES사용유무;
            this.e표면검사이미지저장.IsOn = Global.환경설정.표면검사이미지저장;
            this.e표면검사사용.IsOn = Global.환경설정.표면검사사용;

            this.e강제배출.EditValueChanged += 강제배출Changed;
            this.e배출구분.EditValueChanged += 배출구분Changed;
            this.e이미지자동삭제.EditValueChanged += 이미지자동삭제Changed;
            this.e강제커버조립.EditValueChanged += 강제커버조립Changed;
            this.e커버조립여부.EditValueChanged += 커버조립여부Changed;
            this.eMES사용유무.EditValueChanged += MES사용유무Changed;
            this.e표면검사이미지저장.EditValueChanged += 표면검사이미지저장Changed;
            this.e표면검사사용.EditValueChanged += 표면검사사용Changed;

            this.b캠트리거리셋.Click += 캠트리거리셋;
            this.e센서리셋.IsOn = false;
            this.e센서리셋.EditValueChanged += 제로셋모드Changed;
            this.b설정저장.Click += 환경설정저장;
            this.e이미지삭제시작시간.EditValue = Global.환경설정.이미지자동삭제시작시간;
            this.e이미지저장일수.Value = Global.환경설정.이미지보관일수;
            this.e이미지저장일수.ValueChanged += 이미지저장일수Changed;
            this.e이미지삭제시작시간.EditValueChanging += 이미지삭제시작시간Changing;
            this.e이미지삭제시작시간.EditValueChanged += 이미지삭제시작시간Changed;

            this.e카메라.Init();
            this.e큐알장치.Init();
            this.e기본설정.Init();
            this.e유저관리.Init();
        }

      
        private void MES사용유무Changed(object sender, EventArgs e)
        {
            Global.환경설정.MES사용유무 = this.eMES사용유무.IsOn;

            if (Global.환경설정.MES사용유무)
            {
                Global.mes통신.Init();
                Global.mes통신.Start();
            }
            else Global.mes통신.Close();
        }

        private void BTest_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Global.사진자료.사진정리();
            });
        }

        private void 이미지삭제시작시간Changing(object sender, ChangingEventArgs e)
        {
            if (DateTime.Now > (DateTime)e.NewValue)
            {
                e.Cancel = true;
                return;
            }
        }

        private void 환경설정저장(object sender, EventArgs e)
        {
            this.Bind환경설정.EndEdit();
            if (!Utils.Confirm(this.FindForm(), 번역.저장확인, Localization.확인.GetString())) return;
            Global.환경설정.Save();
            Global.정보로그(환경설정.로그영역.GetString(), 번역.설정저장, 번역.저장완료, true);
        }

        public void Close()
        {
            this.e카메라.Close();
            this.e큐알장치.Close();
            this.e기본설정.Close();
            this.e유저관리.Close();
        }

        private void 커버조립여부Changed(object sender, EventArgs e) => Global.환경설정.커버조립여부 = this.e커버조립여부.IsOn;

        private void 강제커버조립Changed(object sender, EventArgs e) => Global.환경설정.강제커버조립사용 = e강제커버조립.IsOn;

        private void 이미지삭제시작시간Changed(object sender, EventArgs e) => Global.환경설정.이미지자동삭제시작시간 = this.e이미지삭제시작시간.Time;
        private void 표면검사사용Changed(object sender, EventArgs e) => Global.환경설정.표면검사사용 = this.e표면검사사용.IsOn;
        private void 표면검사이미지저장Changed(object sender, EventArgs e) => Global.환경설정.표면검사이미지저장 = this.e표면검사이미지저장.IsOn;
        private void 강제배출Changed(object sender, EventArgs e) => Global.환경설정.강제배출 = this.e강제배출.IsOn;
        private void 배출구분Changed(object sender, EventArgs e) => Global.환경설정.양품불량 = this.e배출구분.IsOn;
        private void 제로셋모드Changed(object sender, EventArgs e) => Global.환경설정.제로셋모드 = this.e센서리셋.IsOn;
        private void 이미지자동삭제Changed(object sender, EventArgs e) => Global.환경설정.이미지자동삭제모드 = this.e이미지자동삭제.IsOn;
        private void 이미지저장일수Changed(object sender, EventArgs e) => Global.환경설정.이미지보관일수 = this.e이미지저장일수.Value;
        private void 캠트리거리셋(object sender, EventArgs e)
        {
            if (!Utils.Confirm(this.FindForm(), "트리거 보드의 위치를 초기화 하시겠습니까?")) return;
            직렬포트[] 포트들 = new 직렬포트[] { 직렬포트.COM7 };
            포트들.ForEach(port =>
            {
                Enc852 트리거보드 = new Enc852(port);
                트리거보드.Clear();
                트리거보드.Close();
            });
            Global.정보로그("트리거보드", "초기화", "초기화 되었습니다.", true);
        }

        private class LocalizationDeviceSetting
        {
            private enum Items
            {
                [Translation("Save", "설정저장")]
                설정저장,
                [Translation("It's saved.", "저장되었습니다.")]
                저장완료,
                [Translation("Save your preferences?", "환경설정을 저장하시겠습니까?")]
                저장확인,
            }

            public String 설정저장 => Localization.GetString(Items.설정저장);
            public String 저장완료 => Localization.GetString(Items.저장완료);
            public String 저장확인 => Localization.GetString(Items.저장확인);
        }
    }
}
