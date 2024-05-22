using MvUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DSEV.Schemas
{
    partial class 장치통신
    {
        private DateTime 오류알림시간 = DateTime.Today.AddDays(-1);
        private Int32 오류알림간격 = 30; // 초

        public void 통신오류알림(Int32 오류코드)
        {
            if (오류코드 == 0)
            {
                this.정상여부 = true;
                return;
            }
            if ((DateTime.Now - this.오류알림시간).TotalSeconds < this.오류알림간격) return;
            this.오류알림시간 = DateTime.Now;
            this.정상여부 = false;
            Global.오류로그(로그영역, "Communication", $"[{오류코드.ToString("X8")}] Communication error.", false);
        }

        private Boolean 입출자료갱신()
        {
            DateTime 현재 = DateTime.Now;
            // 입출자료 갱신
            Int32[] 자료 = ReadDeviceRandom(입출자료.주소목록, out Int32 오류);
            if (오류 != 0)
            {
                통신오류알림(오류);
                Debug.WriteLine("오휴확인");
                return false;
            }
            this.입출자료.Set(자료);
            //Debug.WriteLine("오류없음.");
            return true;
        }

        private Boolean 입출자료분석()
        {
            if (Global.환경설정.동작구분 == 동작구분.LocalTest) return 테스트수행();
            if (!입출자료갱신()) return false;
            검사위치확인();
            제품검사수행();
            장치상태확인();
            통신핑퐁수행();
            return true;
        }

        private void 장치상태확인()
        {
            if (this.입출자료.Changed(정보주소.자동수동) || this.입출자료.Changed(정보주소.시작정지))
                this.동작상태알림?.Invoke();
        }

        // 검사위치 변경 확인
        private void 검사위치확인()
        {
            Dictionary<정보주소, Int32> 변경 = this.입출자료.Changes(정보주소.하부큐알트리거, 정보주소.결과요청트리거);
            
            if (변경.Count < 1) return;

            if (!this.하부큐알트리거신호 && this.하부큐알확인완료신호)
            {
                this.하부큐알결과OK신호 = false;
                this.하부큐알결과NG신호 = false;
                this.하부큐알확인완료신호 = false;
            }

            if (!this.바닥평면트리거신호 && this.바닥평면확인완료신호) this.바닥평면확인완료신호 = false;
            if (!this.측상촬영트리거신호 && this.측상촬영완료신호) this.측상촬영완료신호 = false;
            if (!this.상부큐알트리거신호 && this.상부큐알확인완료신호) this.상부큐알확인완료신호 = false;
            if (!this.하부촬영트리거신호 && this.하부촬영완료신호) this.하부촬영완료신호 = false;
            if (!this.커넥터촬영트리거신호 && this.커넥터촬영완료신호) this.커넥터촬영완료신호 = false;
            if (!this.커버조립트리거신호 && this.커버조립확인완료신호)
            {
                this.커버조립결과OK신호 = false;
                this.커버조립결과NG신호 = false;
                this.커버조립확인완료신호 = false;
            }
            if (!this.커버들뜸트리거신호 && this.커버들뜸확인완료신호) this.커버들뜸확인완료신호 = false;
            if (!this.결과요청트리거신호 && this.결과요청확인완료신호)
            {
                this.결과요청결과OK신호 = false;
                this.결과요청결과NG신호 = false;
                this.결과요청확인완료신호 = false;
            }
            this.검사위치알림?.Invoke();
        }

        private void 제품검사수행()
        {
            영상촬영수행();
            평탄검사수행();
            큐알리딩수행();
            커버조립확인();
            검사결과전송();
        }

        private void 커버조립여부전송(Boolean 사용여부)
        {
            Debug.WriteLine("커버조립 여부전송시작");
            this.커버조립결과OK신호 = 사용여부;
            this.커버조립결과NG신호 = !사용여부;
            this.커버조립확인완료신호 = true;

            //await Task.Delay(200);

            //this.커버조립결과OK신호 = false;
            //this.커버조립결과NG신호 = false;
            //this.커버조립확인완료신호 = false;

            Debug.WriteLine("커버조립 여부전송완료");
        }

        private void 커버조립확인()
        {

            Int32 검사번호 = this.검사위치번호(정보주소.커버조립트리거);

            if (검사번호 > 0)
            {
                Debug.WriteLine("커버조립트리거 진입");
                new Thread(() =>
                {
                    검사결과 검사 = Global.검사자료.검사항목찾기(검사번호);

                    if (Global.환경설정.강제커버조립사용)
                    {
                        Debug.WriteLine("강제커버조립 들어옴");
                        커버조립여부전송(Global.환경설정.커버조립여부);
                    }
                    else
                    {
                        if (검사.측정결과 == 결과구분.OK || 검사.측정결과 == 결과구분.PS)
                            커버조립여부전송(true);
                        else
                            커버조립여부전송(false);
                    }
                })
                { Priority = ThreadPriority.Highest }.Start();
                Debug.WriteLine("커버조립부분 확인 완료!!!");
            }
        }

        // 카메라 별 현재 검사 위치의 검사번호를 요청
        public Int32 촬영위치번호(카메라구분 구분)
        {
            if (구분 == 카메라구분.Cam01 || 구분 == 카메라구분.Cam02 || 구분 == 카메라구분.Cam03) return this.인덱스버퍼[정보주소.측상촬영트리거];
            if (구분 == 카메라구분.Cam04 || 구분 == 카메라구분.Cam05) return this.인덱스버퍼[정보주소.하부촬영트리거];
            if (구분 == 카메라구분.Cam06 || 구분 == 카메라구분.Cam07) return this.인덱스버퍼[정보주소.커넥터촬영트리거];
            return 0;
        }

        // 트리거 입력 시 현재 인덱스를 버퍼에 입력하고 검사 수행 시 해당 버퍼의 인덱스를 기준으로 검사
        private Int32 검사위치번호(정보주소 구분)
        {
            if (!this.입출자료.Firing(구분, true, out Boolean 현재, out Boolean 변경))
            {
                return -1;
            }

            Int32 index = 0;
            if (구분 == 정보주소.하부큐알트리거) index = this.제품투입번호;
            else if (구분 == 정보주소.바닥평면트리거) index = this.평탄도측정검사번호;
            else if (구분 == 정보주소.측상촬영트리거) index = this.측상검사번호;
            else if (구분 == 정보주소.상부큐알트리거) index = this.상부큐알검사번호;
            else if (구분 == 정보주소.하부촬영트리거) index = this.인슐폭측정검사번호;
            else if (구분 == 정보주소.커넥터촬영트리거) index = this.커넥턱검사번호;
            else if (구분 == 정보주소.커버조립트리거) index = this.커버체결번호;
            else if (구분 == 정보주소.커버들뜸트리거) index = this.커버검사번호;
            else if (구분 == 정보주소.결과요청트리거) index = this.결과요청번호;

            this.인덱스버퍼[구분] = index;

            //Debug.WriteLine("----------------------------------");
            if (index == 0) Global.경고로그(로그영역, 구분.ToString(), $"해당 위치에 검사할 제품의 Index가 없습니다.", false);
            else Debug.WriteLine($"{Utils.FormatDate(DateTime.Now, "{0:HH:mm:ss.fff}")}  {구분} => {index}", "Trigger");
            //Debug.WriteLine($"이송장치1=>{정보읽기(정보주소.이송장치1)}, 검사지그1=>{정보읽기(정보주소.검사지그1)}, 이송장치2=>{정보읽기(정보주소.이송장치2)}, 검사지그2=>{정보읽기(정보주소.검사지그2)}, 이송장치3=>{정보읽기(정보주소.이송장치3)}, 검사지그3=>{정보읽기(정보주소.검사지그3)}, 투입버퍼=>{정보읽기(정보주소.투입버퍼)}", "PLC Index");
            return index;
        }

        public List<Int32> 검사중인항목()
        {
            List<Int32> 대상 = new List<Int32>();
            Int32 시작 = (Int32)정보주소.셔틀08제품인덱스;
            Int32 종료 = (Int32)정보주소.셔틀01제품인덱스;
            for (Int32 i = 종료; i >= 시작; i--)
            {
                정보주소 구분 = (정보주소)i;
                if (this.입출자료[구분].정보 <= 0) continue;
                대상.Add(this.입출자료[구분].정보);
            }
            return 대상;
        }

        private void 큐알리딩수행()
        {
            Int32 하부큐알검사번호 = this.검사위치번호(정보주소.하부큐알트리거);
            Int32 상부큐알검사번호 = this.검사위치번호(정보주소.상부큐알트리거);

            if (하부큐알검사번호 > 0)
            {
                Debug.WriteLine("하부 큐알 검사 시작!!!");
                new Thread(() =>
                {
                    Global.모델자료.선택모델.검사시작(하부큐알검사번호);
                    Debug.WriteLine("선택모델 검사시작");
                    Global.검사자료.검사시작(하부큐알검사번호);
                    Debug.WriteLine("검사자료 검사시작");


                    검사결과 검사 = Global.검사자료.하부큐알리딩수행(하부큐알검사번호);

                    //임시 OK 신호 - MES통신 부분 추가해야됨.
                    if (!Global.환경설정.MES사용유무)
                    {
                        this.하부큐알결과OK신호 = true;
                        this.하부큐알결과NG신호 = false;
                        this.하부큐알확인완료신호 = true;
                    }
                    else
                    {
                        //mes 에 보내고 결과 받아서 신호 설정 로직 추가예정
                        //결과 송부받아서 변경
                        this.하부큐알결과OK신호 = true;
                        this.하부큐알결과NG신호 = false;
                        this.하부큐알확인완료신호 = true;
                    }

                    Global.검사자료.하부큐알리딩수행종료();

                })
                { Priority = ThreadPriority.Highest }.Start();
                Debug.WriteLine("하부 큐알 검사 완료!!!");
            }

            if (상부큐알검사번호 > 0)
            {

                new Thread(() =>
                {
                    Debug.WriteLine("상부 큐알 검사 시작><");
                    검사결과 검사 = Global.검사자료.상부큐알리딩수행(상부큐알검사번호);
                    this.상부큐알확인완료신호 = true;

                    Global.검사자료.상부큐알리딩수행종료();
                })
                { Priority = ThreadPriority.Highest }.Start();
                Debug.WriteLine("상부 큐알 검사 완료><");
            }
        }

        private Boolean 센서제로모드 = false;
        public void 센서제로수행(Boolean 모드)
        {
            this.센서제로모드 = 모드;
            //if (!모드) 정보쓰기(정보주소.평탄센서, 0);
        }
        private void 평탄검사수행()
        {
            Int32 바닥평면검사번호 = this.검사위치번호(정보주소.바닥평면트리거);
            Int32 커버들뜸검사번호 = this.검사위치번호(정보주소.커버들뜸트리거);

            if (바닥평면검사번호 > 0)
            {
                new Thread(() =>
                {
                    Debug.WriteLine("바닥평면도 검사시작");

                    try
                    {
                        if (Global.환경설정.제로셋모드)
                        {
                            Global.센서제어.DoZeroSet(센서컨트롤러.컨트롤러1, 6);
                            Global.센서제어.DoZeroSet(센서컨트롤러.컨트롤러2, 6);
                        }

                        //첫번째 항목 "M0" 제외하고 배열로 만듦
                        string[] cont1Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러1, 바닥평면검사번호).Skip(1).ToArray();
                        string[] cont2Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러2, 바닥평면검사번호).Skip(1).ToArray();
                        this.바닥평면확인완료신호 = true;

                        //배열을 붙임!
                        string[] mergedValues = cont1Values.Concat(cont2Values).ToArray();

                        //Key,Value형태의 자료형으로 생성!
                        Dictionary<센서항목, Single> 센서자료 = new Dictionary<센서항목, Single>();
                        for (int i = 0; i < mergedValues.Length; i++)
                        {
                            센서자료.Add((센서항목)i + 1, Single.Parse(mergedValues[i]) / 1000);
                        }
                        Global.검사자료.평탄검사수행(바닥평면검사번호, 센서자료);
                        //this.바닥평면확인완료신호 = false;
                    }
                    catch (Exception ex)
                    {
                        Global.오류로그(로그영역, "바닥평면검사", ex.Message, true);
                        this.바닥평면확인완료신호 = false;
                    }

                    Debug.WriteLine("바닥평면검사 종료");
                })
                { Priority = ThreadPriority.Highest }.Start();
            }

            if (커버들뜸검사번호 > 0)
            {
                new Thread(() =>
                {

                    Debug.WriteLine("커버들뜸 검사시작");

                    try
                    {
                        if (Global.환경설정.제로셋모드)
                        {
                            Global.센서제어.DoZeroSet(센서컨트롤러.컨트롤러3, 7);
                            Global.센서제어.DoZeroSet(센서컨트롤러.컨트롤러4, 8);
                        }

                        //첫번째 항목 "M0" 제외하고 배열로 만듦
                        string[] cont1Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러3, 커버들뜸검사번호).Skip(1).ToArray();
                        string[] cont2Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러4, 커버들뜸검사번호).Skip(1).ToArray();
                        this.커버들뜸확인완료신호 = true;

                        //배열을 붙임!
                        string[] mergedValues = cont1Values.Concat(cont2Values).ToArray();

                        //Key,Value형태의 자료형으로 생성!
                        Dictionary<센서항목, Single> 센서자료 = new Dictionary<센서항목, Single>();
                        for (int i = 0; i < mergedValues.Length; i++)
                        {
                            센서자료.Add((센서항목)i + 21, Single.Parse(mergedValues[i]) / 1000);
                        }
                        Global.검사자료.평탄검사수행(커버들뜸검사번호, 센서자료);
                        //this.커버들뜸확인완료신호 = false;
                    }
                    catch (Exception ex)
                    {
                        Global.오류로그(로그영역, "커버들뜸검사", ex.Message, true);
                        this.커버들뜸확인완료신호 = false;
                    }

                    Debug.WriteLine("커버들뜸검사 종료");
                })
                { Priority = ThreadPriority.Highest }.Start();
            }
        }

        private void 영상촬영수행()
        {
            Int32 측상카메라검사번호 = this.검사위치번호(정보주소.측상촬영트리거);
            Int32 하부카메라검사번호 = this.검사위치번호(정보주소.하부촬영트리거);
            Int32 커넥터카메라검사번호 = this.검사위치번호(정보주소.커넥터촬영트리거);

            if (측상카메라검사번호 > 0)
            {
                new Thread(() =>
                {
                    Global.조명제어.TurnOn(카메라구분.Cam01);
                    Global.조명제어.TurnOn(카메라구분.Cam02);
                    Global.조명제어.TurnOn(카메라구분.Cam03);
                    Global.그랩제어.Active(카메라구분.Cam01);
                    Global.그랩제어.Active(카메라구분.Cam02);
                    Global.그랩제어.Active(카메라구분.Cam03);

                    this.측상촬영완료신호 = true;
                })
                { Priority = ThreadPriority.Highest }.Start();
            }

            if (하부카메라검사번호 > 0)
            {
                new Thread(() =>
                {
                    Global.조명제어.TurnOn(카메라구분.Cam04);
                    Global.조명제어.TurnOn(카메라구분.Cam05);
                    Global.그랩제어.Active(카메라구분.Cam04);
                    Global.그랩제어.Active(카메라구분.Cam05);

                    this.하부촬영완료신호 = true;
                })
                { Priority = ThreadPriority.Highest }.Start();
            }

            if (커넥터카메라검사번호 > 0)
            {
                new Thread(() =>
                {
                    Global.조명제어.TurnOn(카메라구분.Cam06);
                    Global.조명제어.TurnOn(카메라구분.Cam07);
                    Global.그랩제어.Active(카메라구분.Cam06);
                    Global.그랩제어.Active(카메라구분.Cam07);

                    this.커넥터촬영완료신호 = true;
                })
                { Priority = ThreadPriority.Highest }.Start();
            }
        }

        // 최종 검사 결과 보고
        private void 검사결과전송()
        {
            Int32 검사번호 = this.검사위치번호(정보주소.결과요청트리거);
            if (검사번호 <= 0) return;

            Global.모델자료.선택모델.검사종료(검사번호);
            검사결과 검사 = Global.검사자료.검사결과계산(검사번호);

            // 강제배출
            Debug.WriteLine("검사결과 강제배출 확인중");
            if (Global.환경설정.강제배출)
            {
                결과전송(Global.환경설정.양품불량);
                Global.검사자료.검사완료알림함수(검사);
                return;
            }

            Debug.WriteLine("강제배출 아님. 검사 비어있는지 확인 중");
            if (검사 == null)
            {
                결과전송(false);
                Global.검사자료.검사완료알림함수(검사);
                return;
            }

            Debug.WriteLine("안비어있음. 결과전송 진행 예정");
            // 배출 수행
            결과전송(검사.측정결과 == 결과구분.OK);
            Debug.WriteLine($"{검사.측정결과}");

            Global.검사자료.검사완료알림함수(검사);
        }

        // 신호 Writing 순서 중요
        private void 결과전송(Boolean 양품여부)
        {
            Debug.WriteLine("결과전송시작");
            this.결과요청결과NG신호 = !양품여부;
            this.결과요청결과OK신호 = 양품여부;
            this.결과요청확인완료신호 = true;

            // 1초 후에 a를 false로 변경하는 비동기 작업 예약
            //await Task.Delay(200);

            //this.결과요청결과OK신호 = false;
            //this.결과요청결과NG신호 = false;
            //this.결과요청확인완료신호 = false;

            Debug.WriteLine("결과전송완료");
        }

        // 핑퐁
        private void 통신핑퐁수행()
        {
            if (!this.입출자료[정보주소.통신핑퐁].Passed()) return;
            if (this.시작일시.Day != DateTime.Today.Day)
            {
                this.시작일시 = DateTime.Now;
                this.검사번호리셋 = true;
                Global.모델자료.선택모델.날짜변경();
            }
            this.통신확인핑퐁 = !this.통신확인핑퐁;
            this.통신상태알림?.Invoke();
        }

        private Boolean 테스트수행()
        {
            통신핑퐁수행();
            return true;
        }
    }
}
