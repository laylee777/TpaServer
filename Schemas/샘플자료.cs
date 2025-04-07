using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IVM.Schemas
{
    public class 샘플자료 : Dictionary<String, 검사결과>
    {
        public void Init()
        {
            this.Add("MFR01111AA;2311260890A", Init1("MFR01111AA;2311260890A"));
            this.Add("MFR01111AA;2311271111A", Init2("MFR01111AA;2311271111A"));
        }

        private 검사결과 Init1(String 큐알코드)
        {
            검사결과 결과 = new 검사결과() { 큐알내용 = 큐알코드 };

            return 결과;
        }

        private 검사결과 Init2(String 큐알코드)
        {
            검사결과 결과 = new 검사결과() { 큐알내용 = 큐알코드 };
         
            return 결과;
        }

        public 검사정보 SetResult(List<검사정보> 내역, 검사항목 항목, Double 결과값) => SetResult(내역.Where(e => e.검사항목 == 항목).FirstOrDefault(), 결과값);
        public 검사정보 SetResult(검사정보 검사, Double 결과값)
        {
            if (검사 == null) return null;
            if (Double.IsNaN(결과값)) { 검사.측정결과 = 결과구분.ER; return 검사; }
            검사.결과값 = (Decimal)Math.Round(결과값, Global.환경설정.결과자릿수);
            검사.결과계산();
            return 검사;
        }

        private List<검사정보> 검사내역(String 큐알코드)
        {
            if (String.IsNullOrEmpty(큐알코드)) return null;
            if (this.ContainsKey(큐알코드)) return this[큐알코드].검사내역;
            return null;
        }
        public Boolean 검사결과(검사결과 결과)
        {
            List<검사정보> 내역 = 검사내역(결과.큐알내용);
            Debug.WriteLine($"{결과.검사코드} {내역 == null}", 결과.큐알내용);
            if (내역 == null) return false;
            foreach (검사정보 정보 in 내역)
            {
                if (정보.측정결과 <= 결과구분.ER) continue;
                검사정보 검사 = 결과.GetItem(정보.검사항목);
                검사.결과값 = 정보.결과값;
                검사.결과계산();
                Debug.WriteLine($"{검사.결과값} => {정보.결과값}", 정보.검사명칭);
            }
            결과.바닥평면도계산();
            return true;
        }
    }
}

