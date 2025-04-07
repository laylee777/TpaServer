using DevExpress.Diagram.Core.Shapes;
using DevExpress.XtraRichEdit.Export;
using IVM.UI.Forms;
using MvUtils;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace IVM.Schemas
{
    [Table("inspl")]
    public class 검사결과
    {
        [Column("ilwdt"), Required, Key, JsonProperty("ilwdt"), Translation("Time", "일시")]
        public DateTime 검사일시 { get; set; } = DateTime.Now;
        [Column("ilmcd"), JsonProperty("ilmcd"), Translation("Model", "모델")]
        public 모델구분 모델구분 { get; set; } = 모델구분.None;
        [Column("ilnum"), JsonProperty("ilnum"), Translation("Index", "번호")]
        public Int32 검사코드 { get; set; } = 0;
        [Column("ilres"), JsonProperty("ilres"), Translation("Result", "판정")]
        public 결과구분 측정결과 { get; set; } = 결과구분.WA;
        [Column("ilctq"), JsonProperty("ilctq"), Translation("CTQ", "CTQ결과")] //Critical to Quality
        public 결과구분 CTQ결과 { get; set; } = 결과구분.WA;
        [Column("ilsuf"), JsonProperty("ilsuf"), Translation("Suface", "외관결과")]
        public 결과구분 외관결과 { get; set; } = 결과구분.WA;
        //[Column("ilqrg"), JsonProperty("ilqrg"), Translation("QR Legibility", "QR등급")]
        //public 큐알등급 큐알등급 { get; set; } = 큐알등급.X;
        [Column("ilqrs"), JsonProperty("ilqrs"), Translation("QR Code", "QR코드")]
        public String 큐알내용 { get; set; } = String.Empty;
        //[Column("ilngs"), JsonProperty("ilngs"), Translation("NG Items", "불량정보")]
        //public String 불량정보 { get; set; } = String.Empty;
        [NotMapped, JsonIgnore, Translation("NG Items", "불량정보")]
        public String 불량정보 { get; set; } = String.Empty;

        [NotMapped, JsonIgnore]
        public String 결과문구 => Localization.GetString(측정결과);
        [NotMapped, JsonIgnore]
        public String 품질문구 => Localization.GetString(CTQ결과);
        [NotMapped, JsonIgnore]
        public String 외관문구 => Localization.GetString(외관결과);

        [NotMapped, JsonProperty("inspd")]
        public List<검사정보> 검사내역 { get; set; } = new List<검사정보>();
        [NotMapped, JsonProperty("ilreg"), Browsable(false)]
        public List<불량영역> 표면불량 { get; set; } = new List<불량영역>();
        [NotMapped, JsonIgnore, Browsable(false)]
        public List<String> 불량내역 = new List<String>();


        //마킹 전 확인용
        [NotMapped, JsonIgnore]
        public 결과구분 마킹전결과 { get; set; } = 결과구분.WA;


        public 검사결과()
        {
            this.검사일시 = DateTime.Now;
            this.모델구분 = Global.환경설정.선택모델;
        }

        public 검사결과 Reset()
        {
            this.검사일시 = DateTime.Now;
            this.모델구분 = Global.환경설정.선택모델;
            this.측정결과 = 결과구분.WA;
            this.CTQ결과 = 결과구분.WA;
            this.외관결과 = 결과구분.WA;
            //this.큐알등급 = 큐알등급.X;
            this.큐알내용 = String.Empty;
            this.불량정보 = String.Empty;
            this.검사내역.Clear();
            this.표면불량.Clear();
            this.불량내역.Clear();

            검사설정 자료 = Global.모델자료.GetItem(this.모델구분)?.검사설정;
            foreach (검사정보 정보 in 자료)
            {
                if (!정보.검사여부) continue;
                this.검사내역.Add(new 검사정보(정보) { 검사일시 = this.검사일시 });
            }
            return this;
        }
        public 검사결과 Reset(카메라구분 카메라)
        {
            검사설정 자료 = Global.모델자료.GetItem(this.모델구분)?.검사설정;
            foreach (검사정보 정보 in 자료)
            {
                if ((Int32)정보.검사장치 != (Int32)카메라) continue;
                검사정보 수동 = this.검사내역.Where(e => e.검사항목 == 정보.검사항목).FirstOrDefault();
                if (정보 == null) continue;
                if (수동 == null)
                {
                    Global.정보로그("DataReset", "수동", $"검사내역에 {정보.검사명칭}이 없습니다.", true);
                    continue;
                }
                수동.검사명칭 = 정보.검사명칭;
                수동.최소값 = 정보.최소값;
                수동.기준값 = 정보.기준값;
                수동.최대값 = 정보.최대값;
                수동.보정값 = 정보.보정값;
                수동.교정값 = 정보.교정값;
            }
            this.표면불량.RemoveAll(e => (Int32)e.장치구분 == (Int32)카메라);
            return this;
        }

        public void AddRange(List<검사정보> 자료)
        {
            this.검사내역.AddRange(자료);
            this.검사내역.ForEach(e => { e.Init(); e.검사명칭 = Global.모델자료.GetItemName(this.모델구분, e.검사항목); });
            List<String> 불량내역 = this.검사내역.Where(e => e.측정결과 != 결과구분.OK && e.측정결과 != 결과구분.PS).Select(e => e.검사명칭).ToList();
            if (불량내역.Count > 0) this.불량정보 = String.Join(",", 불량내역);
        }

        private String[] AppearanceFields = new String[] { nameof(측정결과), nameof(CTQ결과), nameof(외관결과) };
        public void SetAppearance(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null || !AppearanceFields.Contains(e.Column.FieldName)) return;
            PropertyInfo p = typeof(검사결과).GetProperty(e.Column.FieldName);
            if (p == null || p.PropertyType != typeof(결과구분)) return;
            Object v = p.GetValue(this);
            if (v == null) return;
            e.Appearance.ForeColor = 환경설정.ResultColor((결과구분)v);
        }

        public 검사정보 GetItem(장치구분 장치, String name) => 검사내역.Where(e => e.검사장치 == 장치 && e.변수명칭 == name).FirstOrDefault();
        public 검사정보 GetItem(검사항목 항목) => 검사내역.Where(e => e.검사항목 == 항목).FirstOrDefault();

        private Decimal PixelToMeter(검사정보 검사, Double value)
        {
            Double result = 0;
            if (value == 0 || 검사.교정값 <= 0) result = value;
            else if (검사.카메라여부) result = value * (Decimal.ToDouble(검사.교정값) / 1000);
            else result = value;
            return (Decimal)Math.Round(result, Global.환경설정.결과자릿수);
        }
        private Double MeterToPixel(검사정보 검사, Decimal value)
        {
            if (검사.교정값 <= 0 || !검사.카메라여부) return Decimal.ToDouble(value);
            return Decimal.ToDouble(value) / Decimal.ToDouble(검사.교정값);
        }

        public Boolean SetResultValue_Client(검사정보 검사, Double value, out Decimal 결과값, out Decimal 측정값, Boolean 마진포함 = false)
        {
            Decimal result = (Decimal)value;
            Boolean r = result >= 검사.최소값 && result <= 검사.최대값;
            결과값 = result;
            측정값 = (Decimal)Math.Round(value, Global.환경설정.결과자릿수);
            if (r) return true;
            //if (검사.마진값 <= 0 || 마진포함) return false;

            //if (검사.최소값 > result)
            //{
            //    if (검사.최소값 > result + 검사.마진값 * 검사.결과부호) return false;
            //}
            //else if (검사.최대값 < result)
            //{
            //    if (검사.최대값 < result - 검사.마진값 * 검사.결과부호) return false;
            //}
            return false;
        }
        public Boolean SetResultValue(검사정보 검사, Double value, out Decimal 결과값, out Decimal 측정값, Boolean 마진포함 = false)
        {
            Decimal result = PixelToMeter(검사, value);
            result += 검사.보정값;
            result *= 검사.결과부호;
            Boolean r = result >= 검사.최소값 && result <= 검사.최대값;
            결과값 = result;
            측정값 = (Decimal)Math.Round(value, Global.환경설정.결과자릿수);
            if (r) return true;
            if (검사.마진값 <= 0 || 마진포함) return false;

            Int32 factor = 0;
            if (검사.최소값 > result)
            {
                if (검사.최소값 > result + 검사.마진값 * 검사.결과부호) return false;
                factor = 1;
            }
            else if (검사.최대값 < result)
            {
                if (검사.최대값 < result - 검사.마진값 * 검사.결과부호) return false;
                factor = -1;
            }
            Double value2 = value + MeterToPixel(검사, 검사.마진값) * factor;
            if (value2 == value) return false;

            Boolean r2 = SetResultValue(검사, value2, out Decimal 결과값2, out Decimal 측정값2, true);
            if (r2)
            {
                결과값 = 결과값2;
                측정값 = 측정값2;
                return true;
            }
            return false;
        }

        public 검사정보 SetResult(검사정보 검사, Double value)
        {
            if (검사 == null) return null;
            if (Double.IsNaN(value)) { 검사.측정결과 = 결과구분.ER; return 검사; }
            Boolean ok = SetResultValue(검사, value, out Decimal 결과값, out Decimal 측정값);
            검사.측정값 = 측정값;
            검사.결과값 = 결과값;
            검사.측정결과 = ok ? 결과구분.OK : 결과구분.NG;
            return 검사;
        }
        public 검사정보 SetResult(String name, Double value) => SetResult(검사내역.Where(e => e.검사항목.ToString() == name).FirstOrDefault(), value);
        public 검사정보 SetResult(검사항목 항목, Double value) => SetResult(검사내역.Where(e => e.검사항목 == 항목).FirstOrDefault(), value);
        public void SetResults(카메라구분 카메라, Dictionary<String, Object> results)
        {
            //불량영역제거(카메라);
            //String scratch = ResultAttribute.VarName(검사항목.BottomScratch);
            //String dent = ResultAttribute.VarName(검사항목.BottomDent);
            foreach (var result in results)
            {
                //if (result.Key.Equals(scratch) || result.Key.Equals(dent))
                //{
                //    this.표면불량.AddRange(result.Value as List<불량영역>);
                //    continue;
                //}
                검사정보 정보 = GetItem((장치구분)카메라, result.Key);
                if (정보 == null) continue;
                Double value = result.Value == null ? Double.NaN : (Double)result.Value;
                SetResult(정보, value);
            }
        }
        public void SetResults(Dictionary<Int32, Decimal> 내역)
        {
            if (내역 == null) return;
            foreach (Int32 index in 내역.Keys)
            {
                검사항목 항목 = (검사항목)index;
                SetResult(항목, Convert.ToDouble(내역[index]));
            }
        }

        public 검사정보 SetValue(검사항목 항목, Double value) => SetValue(검사내역.Where(e => e.검사항목 == 항목).FirstOrDefault(), value);
        //결과만 추가하도록 새롭게 추가
        public 검사정보 SetValue(검사정보 검사, Double value)
        {
            if (검사 == null) return null;
            if (Double.IsNaN(value)) { 검사.측정결과 = 결과구분.ER; return 검사; }
            Boolean ok = SetResultValue_Client(검사, value, out Decimal 결과값, out Decimal 측정값);
            검사.측정값 = 측정값;
            검사.결과값 = 결과값;
            검사.측정결과 = ok ? 결과구분.OK : 결과구분.NG;

            return 검사;
        }

        public void SetValues(Dictionary<Int32, Decimal> 내역)
        {
            if (내역 == null) return;
            foreach (Int32 index in 내역.Keys)
            {

                검사항목 항목 = (검사항목)index;
                SetValue(항목, Convert.ToDouble(내역[index]));
                Debug.WriteLine($"{항목}, {내역[index]}");
            }
        }

        public List<불량영역> 불량영역(카메라구분 카메라) => this.표면불량.Where(e => e.장치구분 == (장치구분)카메라).ToList();
        public void 불량영역제거(카메라구분 카메라)
        {
            List<불량영역> 불량 = 불량영역(카메라);
            불량.ForEach(e => this.표면불량.Remove(e));
        }

        private 결과구분 최종결과(List<결과구분> 결과목록)
        {
            if (결과목록.Contains(결과구분.ER)) return 결과구분.ER;
            if (결과목록.Contains(결과구분.NG)) return 결과구분.NG;
            return 결과구분.OK;
        }

        public 결과구분 결과계산()
        {
            Debug.WriteLine("평면도/운곽도 계산시작");
            this.선윤곽도계산();
            this.바닥평면도계산();
            this.커버윤곽도계산();
            Debug.WriteLine("평면도/운곽도 계산종료");

            //this.측면윤곽도계산();
            //this.부자재검사결과();
            //this.하부표면검사결과();

            //this.SetResult(검사항목.BottomDent, this.표면불량.Where(e => e.검사항목 == 검사항목.BottomDent).Count());
            //this.SetResult(검사항목.BottomScratch, this.표면불량.Where(e => e.검사항목 == 검사항목.BottomScratch).Count());

            List<결과구분> 전체결과 = new List<결과구분>();
            List<결과구분> 품질결과 = new List<결과구분>();
            List<결과구분> 외관결과 = new List<결과구분>();
            //List<결과구분> 마킹전결과목록 = new List<결과구분>();

            foreach (검사정보 정보 in this.검사내역)
            {
                // 임시로 검사중인 항목 완료 처리
                if (정보.측정결과 < 결과구분.PS)
                {

                    this.SetResult(정보, Convert.ToDouble(정보.측정값));
                }

                if (정보.측정결과 == 결과구분.PS) continue;

                if (!전체결과.Contains(정보.측정결과))
                {
                    전체결과.Add(정보.측정결과);

                    //if (정보.검사장치 != 장치구분.QrReader) 마킹전결과목록.Add(정보.측정결과);

                }

                if (정보.검사그룹 == 검사그룹.CTQ) { if (!품질결과.Contains(정보.측정결과)) 품질결과.Add(정보.측정결과); }
                if (정보.검사그룹 == 검사그룹.Surface) {
                    if (!외관결과.Contains(정보.측정결과)) 외관결과.Add(정보.측정결과); 
                }
            }


            this.측정결과 = 최종결과(전체결과);
            //this.마킹전결과 = 최종결과(마킹전결과목록);

            if (this.측정결과 == 결과구분.OK)
            {
                this.CTQ결과 = 결과구분.OK;
                this.외관결과 = 결과구분.OK;
            }
            else
            {
                this.CTQ결과 = 최종결과(품질결과);
                this.외관결과 = 최종결과(외관결과);

                List<검사정보> 불량내역 = this.검사내역.Where(e => e.결과분류 == 결과분류.Summary && (e.측정결과 == 결과구분.NG || e.측정결과 == 결과구분.ER)).ToList();
                
                if (불량내역.Count > 0)
                {
                    foreach (검사정보 정보 in 불량내역)
                        this.불량내역.Add(정보.검사항목.ToString());
                }
                
                this.불량정보 = String.Join(",", this.불량내역.ToArray());
                this.불량내역.Clear();
            }

            Debug.WriteLine($"{this.검사코드} = {this.측정결과}", "검사완료");
            Debug.WriteLine($"{this.검사코드} = {this.마킹전결과}", "마킹전검사완료");
            
            return this.측정결과;
        }

        public void 커버들뜸계산()
        {
            검사정보 M1 = this.GetItem(검사항목.m1);
            검사정보 M2 = this.GetItem(검사항목.m2);
            검사정보 M3 = this.GetItem(검사항목.m3);

            List<Single> 커버들뜸값 = new List<Single>();

            커버들뜸값.Add((Single)(M1.기준값 - M1.결과값));
            커버들뜸값.Add((Single)(M2.기준값 - M2.결과값));
            커버들뜸값.Add((Single)(M3.기준값 - M3.결과값));

            Single[] 커버들뜸편차 = PlaneDistanceCalculator.편차계산(커버들뜸값);
            Single 커버들뜸높이 = PlaneDistanceCalculator.FindAbsMaxDiff2(커버들뜸편차);

            this.SetResult(검사항목.ShapeM1M3, 커버들뜸높이);
        }

        public void 커버윤곽도계산()
        {
            //if(this.GetItem())
            double[,] 기준위치 = {
                    {  76.6f,  200, (Single)this.GetItem(검사항목.A1_R).결과값 },
                    { -76.6f,  200, (Single)this.GetItem(검사항목.A2_R).결과값 },
                    {  76.6f, -230, (Single)this.GetItem(검사항목.A3_R).결과값 },
                    { -76.6f, -230, (Single)this.GetItem(검사항목.A4_R).결과값 },
                };

            double[,] 커버윤곽위치 = {
                    {  26.7f,   74.68f, (Single)this.GetItem(검사항목.k1).결과값 },
                    {  26.7f,  -12.62f, (Single)this.GetItem(검사항목.k2).결과값 },
                    {  26.7f,  -85.92f, (Single)this.GetItem(검사항목.k3).결과값 },
                    {  26.7f, -175.42f, (Single)this.GetItem(검사항목.k4).결과값 },
                    { -26.7f, -175.42f, (Single)this.GetItem(검사항목.k5).결과값 },
                    { -26.7f,  -36.52f, (Single)this.GetItem(검사항목.k6).결과값 },
                    { -26.7f,   33.38f, (Single)this.GetItem(검사항목.k7).결과값 },
                    { -26.7f,   85.48f, (Single)this.GetItem(검사항목.k8).결과값 },
                };

            double[,] 들뜸확인위치 = {
                    {  0f,    40f, (Single)this.GetItem(검사항목.m1).결과값 },
                    {  0f,   -60f, (Single)this.GetItem(검사항목.m2).결과값 },
                    {  0f,  -125f, (Single)this.GetItem(검사항목.m3).결과값 },
                };

            try
            {
                double[] plane = FitPlaneLeastSquares(기준위치);
                Debug.WriteLine($"Plane equation: {plane[0]:E4}x + {plane[1]:E4}y - z + {plane[3]:F4} = 0");

                Debug.WriteLine((Single)this.GetItem(검사항목.k1).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k2).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k3).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k4).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k5).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k6).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k7).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k8).결과값);

                double a = plane[0];
                double b = plane[1];
                double c = plane[2];
                double d = plane[3];


                List<double> dist = new List<double>();

                double distance1 = DistanceFromPointToPlane(커버윤곽위치[0, 0], 커버윤곽위치[0, 1], 커버윤곽위치[0, 2], a, b, c, d);
                double distance2 = DistanceFromPointToPlane(커버윤곽위치[1, 0], 커버윤곽위치[1, 1], 커버윤곽위치[1, 2], a, b, c, d);
                double distance3 = DistanceFromPointToPlane(커버윤곽위치[2, 0], 커버윤곽위치[2, 1], 커버윤곽위치[2, 2], a, b, c, d);
                double distance4 = DistanceFromPointToPlane(커버윤곽위치[3, 0], 커버윤곽위치[3, 1], 커버윤곽위치[3, 2], a, b, c, d);
                double distance5 = DistanceFromPointToPlane(커버윤곽위치[4, 0], 커버윤곽위치[4, 1], 커버윤곽위치[4, 2], a, b, c, d);
                double distance6 = DistanceFromPointToPlane(커버윤곽위치[5, 0], 커버윤곽위치[5, 1], 커버윤곽위치[5, 2], a, b, c, d);
                double distance7 = DistanceFromPointToPlane(커버윤곽위치[6, 0], 커버윤곽위치[6, 1], 커버윤곽위치[6, 2], a, b, c, d);
                double distance8 = DistanceFromPointToPlane(커버윤곽위치[7, 0], 커버윤곽위치[7, 1], 커버윤곽위치[7, 2], a, b, c, d);

                dist.Add(distance1);
                dist.Add(distance2);
                dist.Add(distance3);
                dist.Add(distance4);
                dist.Add(distance5);
                dist.Add(distance6);
                dist.Add(distance7);
                dist.Add(distance8);

                Debug.WriteLine((Single)dist[0]);
                Debug.WriteLine((Single)dist[1]);
                Debug.WriteLine((Single)dist[2]);
                Debug.WriteLine((Single)dist[3]);
                Debug.WriteLine((Single)dist[4]);
                Debug.WriteLine((Single)dist[5]);
                Debug.WriteLine((Single)dist[6]);
                Debug.WriteLine((Single)dist[7]);

                this.SetValue(검사항목.k1, -distance1);
                this.SetValue(검사항목.k2, -distance2);
                this.SetValue(검사항목.k3, -distance3);
                this.SetValue(검사항목.k4, -distance4);
                this.SetValue(검사항목.k5, -distance5);
                this.SetValue(검사항목.k6, -distance6);
                this.SetValue(검사항목.k7, -distance7);
                this.SetValue(검사항목.k8, -distance8);

                Debug.WriteLine((Single)this.GetItem(검사항목.k1).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k2).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k3).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k4).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k5).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k6).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k7).결과값);
                Debug.WriteLine((Single)this.GetItem(검사항목.k8).결과값);

                List<double> dist2 = new List<double>();

                double distance2_1 = DistanceFromPointToPlane(들뜸확인위치[0, 0], 들뜸확인위치[0, 1], 들뜸확인위치[0, 2], a, b, c, d);
                double distance2_2 = DistanceFromPointToPlane(들뜸확인위치[1, 0], 들뜸확인위치[1, 1], 들뜸확인위치[1, 2], a, b, c, d);
                double distance2_3 = DistanceFromPointToPlane(들뜸확인위치[2, 0], 들뜸확인위치[2, 1], 들뜸확인위치[2, 2], a, b, c, d);

                Debug.WriteLine(distance2_1);
                Debug.WriteLine(distance2_2);
                Debug.WriteLine(distance2_3);

                this.SetValue(검사항목.m1, -distance2_1);
                this.SetValue(검사항목.m2, -distance2_2);
                this.SetValue(검사항목.m3, -distance2_3);

                커버들뜸계산();
            }
            catch (Exception e) { Utils.DebugException(e, 0); }
        }

        public void 선윤곽도계산()
        {
            검사정보 H1 = this.GetItem(검사항목.h1);
            검사정보 H2 = this.GetItem(검사항목.h2);
            검사정보 H3 = this.GetItem(검사항목.h3);
            검사정보 H4 = this.GetItem(검사항목.h4);

            검사정보 J1 = this.GetItem(검사항목.j1);
            검사정보 J2 = this.GetItem(검사항목.j2);
            검사정보 J3 = this.GetItem(검사항목.j3);
            검사정보 J4 = this.GetItem(검사항목.j4);

            검사정보 F1 = this.GetItem(검사항목.DistBigNotchL);
            검사정보 F2 = this.GetItem(검사항목.DistBigNotchR);

            this.SetResult(검사항목.ShapeH1H2, PlaneDistanceCalculator.선윤곽도계산(H1, H2));
            this.SetResult(검사항목.ShapeH3H4, PlaneDistanceCalculator.선윤곽도계산(H3, H4));
            this.SetResult(검사항목.ShapeJ1J2, PlaneDistanceCalculator.선윤곽도계산(J1, J2));
            this.SetResult(검사항목.ShapeJ3J4, PlaneDistanceCalculator.선윤곽도계산(J3, J4));
            this.SetResult(검사항목.ShapeBigNotchLR, PlaneDistanceCalculator.선윤곽도계산(F1, F2));
        }

        static double DistanceFromPointToPlane(double x0, double y0, double z0, double a, double b, double c, double d)
        {
            // 직선 거리(방향성 포함) 공식 적용
            double numerator = a * x0 + b * y0 + c * z0 + d;
            double denominator = Math.Sqrt(a * a + b * b + c * c);
            double distance = numerator / denominator;
            return distance;
        }

        public static double[] FitPlaneLeastSquares(double[,] points)
        {
            int numPoints = points.GetLength(0);
            double[,] A = new double[numPoints, 3];
            double[] b = new double[numPoints];

            for (int i = 0; i < numPoints; i++)
            {
                A[i, 0] = points[i, 0];
                A[i, 1] = points[i, 1];
                A[i, 2] = 1;
                b[i] = points[i, 2];
            }

            var matrixA = Matrix.Build.DenseOfArray(A);
            var vectorB = DenseVector.OfArray(b);
            var solution = matrixA.PseudoInverse().Multiply(vectorB);

            double a = solution[0];
            double b_coef = solution[1];
            double d = solution[2];
            double c = -1.0;  // We solve for ax + by - z + d = 0

            return new double[] { a, b_coef, c, d };
        }

        //24.06.03 로직 변경 by LHD
        public void 바닥평면도계산()
        {
            Single[,] 기준위치 = {
                    { 76.6f,  200, (Single)this.GetItem(검사항목.A1_F).결과값},
                    {-76.6f,  200, (Single)this.GetItem(검사항목.A2_F).결과값},
                    { 76.6f, -230, (Single)this.GetItem(검사항목.A3_F).결과값},
                    {-76.6f, -230, (Single)this.GetItem(검사항목.A4_F).결과값},
                };

            Single[,] 검사위치 = {
                    { 105,   200, (Single)this.GetItem(검사항목.a1).결과값},
                    {   0,   200, (Single)this.GetItem(검사항목.a2).결과값},
                    {-105,   200, (Single)this.GetItem(검사항목.a3).결과값},
                    {  90,     0, (Single)this.GetItem(검사항목.a4).결과값},
                    { -90,     0, (Single)this.GetItem(검사항목.a5).결과값},
                    { 105,  -230, (Single)this.GetItem(검사항목.a6).결과값},
                    {   0,  -230, (Single)this.GetItem(검사항목.a7).결과값},
                    { -105, -230, (Single)this.GetItem(검사항목.a8).결과값},
                };

            double[,] points = {
                    { 76.6f,  200, (double)this.GetItem(검사항목.A1_F).결과값},
                    {-76.6f,  200, (double)this.GetItem(검사항목.A2_F).결과값},
                    { 76.6f, -230, (double)this.GetItem(검사항목.A3_F).결과값},
                    {-76.6f, -230, (double)this.GetItem(검사항목.A4_F).결과값},
                };

            double[,] points2 = {
                    { 105,   200, (double)this.GetItem(검사항목.a1).결과값},
                    {   0,   200, (double)this.GetItem(검사항목.a2).결과값},
                    {-105,   200, (double)this.GetItem(검사항목.a3).결과값},
                    {  90,     0, (double)this.GetItem(검사항목.a4).결과값},
                    { -90,     0, (double)this.GetItem(검사항목.a5).결과값},
                    { 105,  -230, (double)this.GetItem(검사항목.a6).결과값},
                    {   0,  -230, (double)this.GetItem(검사항목.a7).결과값},
                    { -105, -230, (double)this.GetItem(검사항목.a8).결과값},
                };

            double[] plane = FitPlaneLeastSquares(points);
            Console.WriteLine($"Plane equation: {plane[0]:E4}x + {plane[1]:E4}y - z + {plane[3]:F4} = 0");

            double a = plane[0];
            double b = plane[1];
            double c = plane[2];
            double d = plane[3];


            List<double> dist = new List<double>();

            double distance1 = DistanceFromPointToPlane(points2[0, 0], points2[0, 1], points2[0, 2], a, b, c, d);
            double distance2 = DistanceFromPointToPlane(points2[1, 0], points2[1, 1], points2[1, 2], a, b, c, d);
            double distance3 = DistanceFromPointToPlane(points2[2, 0], points2[2, 1], points2[2, 2], a, b, c, d);
            double distance4 = DistanceFromPointToPlane(points2[3, 0], points2[3, 1], points2[3, 2], a, b, c, d);
            double distance5 = DistanceFromPointToPlane(points2[4, 0], points2[4, 1], points2[4, 2], a, b, c, d);
            double distance6 = DistanceFromPointToPlane(points2[5, 0], points2[5, 1], points2[5, 2], a, b, c, d);
            double distance7 = DistanceFromPointToPlane(points2[6, 0], points2[6, 1], points2[6, 2], a, b, c, d);
            double distance8 = DistanceFromPointToPlane(points2[7, 0], points2[7, 1], points2[7, 2], a, b, c, d);

            dist.Add(distance1);
            dist.Add(distance2);
            dist.Add(distance3);
            dist.Add(distance4);
            dist.Add(distance5);
            dist.Add(distance6);
            dist.Add(distance7);
            dist.Add(distance8);

            Debug.WriteLine($"위치값A1 : {(Single)this.GetItem(검사항목.A1_F).결과값}");
            Single[] 위치편차 = PlaneDistanceCalculator.CalculateDistances(8, 기준위치, 검사위치);
            Debug.WriteLine($"위치편차_1 : {위치편차[0]}, {위치편차[1]}, {위치편차[2]}, {위치편차[3]}, {위치편차[4]}, {위치편차[5]}, {위치편차[6]}, {위치편차[7]}");
            Single 바닥평면 = PlaneDistanceCalculator.FindMinMaxDiff(위치편차);
            

            Debug.WriteLine($"위치편차_2 : {dist[0]}, {dist[1]}, {dist[2]}, {dist[3]}, {dist[4]}, {dist[5]}, {dist[6]}, {dist[7]}");


            Debug.WriteLine($"평면도1 : {바닥평면}");
            Debug.WriteLine($"평면도2 : {Math.Abs(dist.Max() - dist.Min())}");
            double A1_Dist = DistanceFromPointToPlane(points[0, 0], points[0, 1], points[0, 2], a, b, c, d);
            double A2_Dist = DistanceFromPointToPlane(points[1, 0], points[1, 1], points[1, 2], a, b, c, d);
            double A3_Dist = DistanceFromPointToPlane(points[2, 0], points[2, 1], points[2, 2], a, b, c, d);
            double A4_Dist = DistanceFromPointToPlane(points[3, 0], points[3, 1], points[3, 2], a, b, c, d);

            Single[] 데이텀거리 = PlaneDistanceCalculator.CalculateDistances(4, 기준위치, 기준위치);

            Debug.WriteLine($"데이텀첫번째 : {데이텀거리[0]}, {데이텀거리[1]}, {데이텀거리[2]}, {데이텀거리[3]}");
            Debug.WriteLine($"데이텀두번째 : {A1_Dist}, {A2_Dist}, {A3_Dist}, {A4_Dist}");

            this.SetResult(검사항목.Flatness, Math.Abs(dist.Max() - dist.Min()));


            this.SetValue(검사항목.A1_F, A1_Dist);
            this.SetValue(검사항목.A2_F, A2_Dist);
            this.SetValue(검사항목.A3_F, A3_Dist);
            this.SetValue(검사항목.A4_F, A4_Dist);



            this.SetValue(검사항목.a1, -distance1);
            this.SetValue(검사항목.a2, -distance2);
            this.SetValue(검사항목.a3, -distance3);
            this.SetValue(검사항목.a4, -distance4);
            this.SetValue(검사항목.a5, -distance5);
            this.SetValue(검사항목.a6, -distance6);
            this.SetValue(검사항목.a7, -distance7);
            this.SetValue(검사항목.a8, -distance8);
        }

        public void 큐알정보검사(String 코드, 큐알등급 등급)
        {
            this.큐알내용 = 코드;
            //this.큐알등급 = 등급;
            //this.SetResult(검사항목.QrLegibility, (Int32)this.큐알등급);
            this.큐알정보검사();
        }

        private void 큐알정보검사()
        {
            Boolean r = Global.큐알검증.검증수행(this.큐알내용, out String 오류내용, out Int32[] indexs);

            // 큐알코드 검증, 중복여부 체크
            if (!Global.큐알검증.코드검증 || r) this.SetResult(검사항목.QrValidate, 0);
            else this.SetResult(검사항목.QrValidate, 1);
            if (!String.IsNullOrEmpty(this.큐알내용) && Global.큐알검증.중복체크)
            {
                if (!Global.큐알검증.중복검사(this.큐알내용, indexs, out String 중복오류))
                    this.SetResult(검사항목.QrDuplicated, 0);
                else this.SetResult(검사항목.QrDuplicated, 1);
            }
            else this.SetResult(검사항목.QrDuplicated, 1);
        }

        public void 큐알코드대체(String 코드)
        {
            if (!String.IsNullOrEmpty(this.큐알내용) || String.IsNullOrEmpty(코드)) return;
            this.큐알내용 = 코드;
            //this.SetResult(검사항목.QrLegibility, (Int32)큐알등급.X);
            this.큐알정보검사();
        }

        public 결과구분 큐알결과()
        {
            if (String.IsNullOrEmpty(this.큐알내용)) return 결과구분.NG;
            List<검사항목> 항목들 = new List<검사항목>() { 검사항목.QrValidate, 검사항목.QrDuplicated }; //, 검사항목.QrDistS, 검사항목.QrDistB
            foreach (검사항목 항목 in 항목들)
            {
                검사정보 정보 = this.GetItem(항목);
                if (정보 == null || !정보.검사여부) continue;
                if (정보.측정결과 != 결과구분.OK) return 결과구분.NG;
            }
            return 결과구분.OK;
        }

        public Boolean 카메라검사보기(검사정보 정보)
        {
            try
            {
                if (this.검사코드 >= 9999 || this.검사코드 < 1 || 정보 == null || !CameraAttribute.IsCamera(정보.검사장치)) return false;
                카메라구분 카메라 = (카메라구분)정보.검사장치;
                String file = Global.사진자료.CopyImageFile(this.검사일시, this.검사코드, 카메라, false);
                if (String.IsNullOrEmpty(file) || !File.Exists(file))
                    return Utils.WarningMsg("The image file does not exist.");
                CogToolEdit cogToolEdit = new CogToolEdit() { 사진파일 = file };
                cogToolEdit.Init(Global.비전검사[카메라]);
                cogToolEdit.Show(Global.MainForm);
                return true;
            }
            catch (Exception ex) { Utils.ErrorMsg(ex.Message); }
            return false;
        }
    }
}
