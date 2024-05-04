﻿using DevExpress.Utils.Extensions;
using MvUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                return false;
            }
            this.입출자료.Set(자료);
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
            Dictionary<정보주소, Int32> 변경 = this.입출자료.Changes(정보주소.이송장치1, 정보주소.투입버퍼);
            if (변경.Count < 1) return;
            this.검사위치알림?.Invoke();
        }

        private void 제품검사수행()
        {
            영상촬영수행();
            평탄검사수행();
            //외폭검사수행();
            //두께측정수행();
            레이져마킹수행();
            큐알리딩수행();
            //라벨부착수행();
            검사결과전송();
        }

        // 카메라 별 현재 검사 위치의 검사번호를 요청
        public Int32 촬영위치번호(카메라구분 구분)
        {
            if (구분 == 카메라구분.Cam01) return this.인덱스버퍼[정보주소.하부표면];
            //if (구분 == 카메라구분.Cam02 || 구분 == 카메라구분.Cam03) return this.인덱스버퍼[정보주소.상부촬영];
            if (구분 == 카메라구분.Cam02 || 구분 == 카메라구분.Cam03) return this.인덱스버퍼[정보주소.상부인슐폭];
            //if (구분 == 카메라구분.Cam04 || 구분 == 카메라구분.Cam05) return this.인덱스버퍼[정보주소.측면표면];
            //if (구분 == 카메라구분.Cam10) return this.인덱스버퍼[정보주소.제품투입];
            return 0;
        }

        // 트리거 입력 시 현재 인덱스를 버퍼에 입력하고 검사 수행 시 해당 버퍼의 인덱스를 기준으로 검사
        private Int32 검사위치번호(정보주소 구분)
        {
            if (!this.입출자료.Firing(구분, true, out Boolean 현재, out Boolean 변경))
            {
                //if (현재) 정보쓰기(구분, false);
                return -1;
            }

            Int32 index = 0;
            if (구분 == 정보주소.제품투입) index = this.제품투입번호;
            else if (구분 == 정보주소.내부인슐거리) index = this.내부인슐촬영번호;
            else if (구분 == 정보주소.상부표면) index = this.상부촬영번호;
            else if (구분 == 정보주소.CTQ검사1) index = this.CTQ1촬영번호;
            else if (구분 == 정보주소.CTQ검사2) index = this.CTQ2촬영번호;
            else if (구분 == 정보주소.상부인슐폭) index = this.상부인슐폭촬영번호;
            else if (구분 == 정보주소.평탄센서) index = this.평탄도측정번호;
            else if (구분 == 정보주소.하부표면) index = this.하부표면검사번호;
            else if (구분 == 정보주소.측면표면) index = this.측면표면검사번호;
            else if (구분 == 정보주소.레이져마킹) index = this.레이져각인검사번호;
            else if (구분 == 정보주소.검증기구동) index = this.큐알검증기검사번호;
            else if (구분 == 정보주소.라벨결과요구) index = this.라벨부착기검사번호;
            else if (구분 == 정보주소.결과요청) index = this.결과요청번호;

            //index = 1;
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
            Int32 시작 = (Int32)정보주소.이송장치1;
            Int32 종료 = (Int32)정보주소.투입버퍼;
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
            Int32 검사번호 = this.검사위치번호(정보주소.검증기구동);
            if (검사번호 <= 0) return;

            //Global.모델자료.선택모델.검사시작(검사번호);
            검사결과 사전검사 = Global.검사자료.검사항목찾기(검사번호);

            if (사전검사.마킹전결과 == 결과구분.NG)
            {
                Global.라벨부착기제어.라벨부착(검사번호);
                return;
            }

            new Thread(() =>
            {



                검사결과 검사 = Global.검사자료.큐알리딩수행(검사번호);
                this.검증기구동신호 = false;


                //큐알리딩이랑 라벨 부착 합치기
                //검사결과 부착전검사 = Global.검사자료.검사항목찾기(검사번호);

                Debug.WriteLine(검사.큐알등급.ToString());
                if(검사.큐알등급 > 큐알등급.C || 검사.큐알등급 < 큐알등급.A) Global.라벨부착기제어.라벨부착(검사번호);

                //if (부착전검사.결과계산() == 결과구분.NG){
                //    Global.라벨부착기제어.라벨부착(검사번호);
                //};

                
            }) { Priority = ThreadPriority.Highest }.Start();
        }

        private void 라벨부착수행()
        {
            
            Int32 검사번호 = this.검사위치번호(정보주소.라벨결과요구);
            if (검사번호 <= 0) return;

            //new Thread(() =>
            //{
            //    Global.라벨부착기제어.라벨부착(검사번호);
            //})
            //{ Priority = ThreadPriority.Highest }.Start();
        }

        private void 레이져마킹수행()
        {

            //잠시만 레이져 마킹으로 테스트
            Int32 검사번호 = this.검사위치번호(정보주소.레이져마킹);
            if (검사번호 <= 0) return;

            //Global.모델자료.선택모델.검사시작(검사번호);
            //Global.피씨통신.결과요청(검사번호);

            검사결과 검사 = Global.검사자료.검사항목찾기(검사번호);
            검사.결과계산();

            if (검사.마킹전결과 == 결과구분.NG)
            {
                return;
            }

            new Thread(() =>
            {
                Global.레이져마킹제어.레이져마킹시작(검사번호);
                //검사결과 검사 = Global.검사자료.큐알리딩수행(검사번호);
                //this.검증기구동신호 = false;
            })
            { Priority = ThreadPriority.Highest }.Start();
        }

        private Boolean 센서제로모드 = false;
        public void 센서제로수행(Boolean 모드)
        {
            this.센서제로모드 = 모드;
            if (!모드) 정보쓰기(정보주소.평탄센서, 0);
        }
        private void 평탄검사수행()
        {
            Int32 검사번호 = this.검사위치번호(정보주소.평탄센서);
            if (검사번호 <= 0) return;
            
            
            
            new Thread(() => {
                //Dictionary<센서주소, Single> 자료 = new Dictionary<센서주소, Single>();
                //Boolean r = this.평탄센서.Read(out 자료);

                
                Debug.WriteLine("평탄검사 검사시작");

                try
                {
                    //첫번째 항목 "M0" 제외하고 배열로 만듦
                    string[] cont1Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러2, 검사번호).Skip(1).ToArray();
                    string[] cont2Values = Global.센서제어.ReadValues(센서컨트롤러.컨트롤러3, 검사번호).Skip(1).ToArray();

                    //배열을 붙임!
                    string[] mergedValues = cont1Values.Concat(cont2Values).ToArray();

                    //Key,Value형태의 자료형으로 생성!
                    Dictionary<센서항목, Single> 센서자료 = new Dictionary<센서항목, Single>();
                    for (int i = 0; i < mergedValues.Length; i++)
                    {
                        센서자료.Add((센서항목)i, Single.Parse(mergedValues[i])/1000);
                    }
                    Global.검사자료.평탄검사수행(검사번호, 센서자료);
                }
                catch(Exception ex)
                {
                    Global.오류로그(로그영역, "평탄검사", ex.Message, true);
                }

                Debug.WriteLine("평탄검사 종료");

                if (!this.센서제로모드) this.평탄센서리딩신호 = false;
                //if (r) { Global.검사자료.평탄검사수행(검사번호, 자료); }
            }) { Priority = ThreadPriority.AboveNormal }.Start();
        }
        //private void 외폭검사수행()
        //{
        //    Int32 검사번호 = this.검사위치번호(정보주소.외폭센서);
        //    if (검사번호 <= 0) return;
        //    new Thread(() => {
        //        Dictionary<센서주소, Single> 자료 = new Dictionary<센서주소, Single>();
        //        Boolean r = this.외폭센서.Read(out 자료);
        //        if (!this.센서제로모드) this.외폭센서리딩 = false;
        //        if (r) { Global.검사자료.외폭검사수행(검사번호, 자료); }
        //    })
        //    { Priority = ThreadPriority.AboveNormal }.Start();

        //}
        //private void 두께측정수행()
        //{
        //    Int32 검사번호 = this.검사위치번호(정보주소.두께센서);
        //    if (검사번호 <= 0) return;
        //    new Thread(() => {
        //        Dictionary<센서주소, Single> 자료 = new Dictionary<센서주소, Single>();
        //        Boolean r = this.두께센서.Read(out 자료);
        //        if (!this.센서제로모드) this.두께센서리딩 = false;
        //        //if (r) { Global.검사자료.평탄검사수행(검사번호, 자료); }
        //    })
        //    { Priority = ThreadPriority.AboveNormal }.Start();
        //}

        private void 영상촬영수행()
        {
            Int32 제품투입 = this.검사위치번호(정보주소.제품투입); 
            Int32 내부인슐거리검사번호 = this.검사위치번호(정보주소.내부인슐거리);
            Int32 상부표면검사번호 = this.검사위치번호(정보주소.상부표면);
            Int32 CTQ1검사번호 = this.검사위치번호(정보주소.CTQ검사1);
            Int32 CTQ2검사번호 = this.검사위치번호(정보주소.CTQ검사2);
            Int32 상부인슐폭검사번호 = this.검사위치번호(정보주소.상부인슐폭);
            
            //동시 신호임
            Int32 하부표면검사번호 = this.검사위치번호(정보주소.하부표면);
            Int32 측면표면검사번호 = this.검사위치번호(정보주소.측면표면);
            
            if (제품투입 > 0)
            {
                new Thread(() => {
                    Global.모델자료.선택모델.검사시작(제품투입);
                    Debug.WriteLine("선택모델 검사시작");
                    Global.검사자료.검사시작(제품투입);
                    Global.피씨통신.검사시작(제품투입);
                    Debug.WriteLine("검사자료 검사시작");
                    //Global.피씨통신.검사시작(제품투입); 
                }) { Priority = ThreadPriority.AboveNormal }.Start();
                    Debug.WriteLine("PC통신 검사시작");
                this.제품투입신호 = false;
            }
            
            if (내부인슐거리검사번호 > 0)
            {
                new Thread(() => { Global.피씨통신.상부인슐검사(내부인슐거리검사번호); }) { Priority = ThreadPriority.AboveNormal }.Start();
                this.내부인슐거리검사신호 = false;
            }
            
            if (상부표면검사번호 > 0)
            {
                new Thread(() => { Global.피씨통신.상부검사(상부표면검사번호); }) { Priority = ThreadPriority.AboveNormal }.Start();
                this.상부표면검사신호 = false;
            }
            
            if (CTQ1검사번호 > 0)
            {
                new Thread(() => { 

                    Global.피씨통신.CTQ1검사(CTQ1검사번호); 
                
                }) { Priority = ThreadPriority.Highest }.Start();
                
                this.CTQ1검사신호 = false;
            }

            if (CTQ2검사번호 > 0)
            {
                new Thread(() => {

                    Global.피씨통신.CTQ2검사(CTQ2검사번호);

                })
                { Priority = ThreadPriority.Highest }.Start();

                this.CTQ2검사신호 = false;
            }
            //if (CTQ2검사번호 > 0)
            //{
            //    new Thread(() => { Global.피씨통신.상부검사(CTQ2검사번호); }) { Priority = ThreadPriority.AboveNormal }.Start();
            //    this.CTQ2검사신호 = false;
            //}
            //상부인슐 대신 테스트
            //if (CTQ2검사번호 > 0)
            //{
            //    new Thread(() =>
            //    {
            //        Global.조명제어.TurnOn(카메라구분.Cam02);
            //        Global.조명제어.TurnOn(카메라구분.Cam03);
            //        Global.그랩제어.Active(카메라구분.Cam02);
            //        Global.그랩제어.Active(카메라구분.Cam03);
            //    }).Start();
            //    this.CTQ2검사신호 = false;
            //}
            //if (측면표면검사번호 > 0)
            //{
            //    new Thread(() => { Global.피씨통신.측면검사(측면표면검사번호); }) { Priority = ThreadPriority.AboveNormal }.Start();
            //    this.측면촬영신호 = false;
            //}
            if (상부인슐폭검사번호 > 0)
            {
                new Thread(() =>
                {
                    Global.조명제어.TurnOn(카메라구분.Cam02);
                    Global.조명제어.TurnOn(카메라구분.Cam03);
                    Global.그랩제어.Active(카메라구분.Cam02);
                    Global.그랩제어.Active(카메라구분.Cam03);
                }).Start();
                this.상부인슐폭검사신호 = false;
            }

            //Global.모델자료.선택모델.검사시작(1);
            //Global.검사자료.검사시작(1);

            if (하부표면검사번호 > 0)
            {
                new Thread(() => {
                    Global.피씨통신.측면검사(하부표면검사번호);
                }) { Priority = ThreadPriority.AboveNormal }.Start();
                this.측면촬영신호 = false;

                new Thread(() =>
                {
                    Global.조명제어.TurnOn(카메라구분.Cam01);
                    Global.그랩제어.Active(카메라구분.Cam01);
                }).Start();
                this.하부촬영신호 = false;
            }
        }

        // 최종 검사 결과 보고
        private void 검사결과전송()
        {
            Int32 검사번호 = this.검사위치번호(정보주소.결과요청);
            if (검사번호 <= 0) return;

            //Global.피씨통신.결과요청(검사번호);


            Debug.WriteLine("1");
            Global.모델자료.선택모델.검사종료(검사번호);
            Debug.WriteLine("2");
            검사결과 검사 = Global.검사자료.검사결과계산(검사번호);
            Debug.WriteLine("3");
            생산수량전송();
            // 강제배출
            Debug.WriteLine("검사결과 강제배출 확인중");
            if (Global.환경설정.강제배출) { 
                결과전송(Global.환경설정.양품불량);

                Global.검사자료.검사완료알림함수(검사);


                return; }

            Debug.WriteLine("강제배출 아님. 검사 비어있는지 확인 중");
            if (검사 == null) { 
                결과전송(false);
                Global.검사자료.검사완료알림함수(검사);

                return; }
            
            
            Debug.WriteLine("안비어있음. 결과전송 진행 예정");
            // 배출 수행
            결과전송(검사.측정결과 == 결과구분.OK);
            Debug.WriteLine($"{검사.측정결과}");

            Global.검사자료.검사완료알림함수(검사);
        }
        // 신호 Writing 순서 중요
        private async void 결과전송(Boolean 양품여부)
        {
            Debug.WriteLine("결과전송시작_test");
            this.양품여부요청 = 양품여부;
            this.불량여부요청 = !양품여부;

            // 1초 후에 a를 false로 변경하는 비동기 작업 예약
            await Task.Delay(1000);
            
            this.양품여부요청 = false;
            this.불량여부요청 = false;

            this.검사결과요청 = false;
            Debug.WriteLine("결과전송완료_test");
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

        //private DateTime 테스트위치확인 = DateTime.Now;
        private Boolean 테스트수행()
        {
            통신핑퐁수행();
            //if ((DateTime.Now - 테스트위치확인).TotalSeconds < 5) return true;
            //테스트위치확인 = DateTime.Now;
            //Random rnd = new Random();
            //List<Int32> 목록 = new List<Int32>();
            //foreach (정보주소 주소 in typeof(정보주소).GetEnumValues())
            //{
            //    Int32 val = 0;
            //    if (주소 >= 정보주소.인덱스02 && 주소 <= 정보주소.인덱스07)
            //        val = rnd.Next(0, 32);
            //    목록.Add(val);
            //}
            //입출자료.Set(목록.ToArray());
            //검사위치확인();
            return true;
        }
    }
}
