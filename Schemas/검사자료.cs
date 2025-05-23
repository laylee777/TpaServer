﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvUtils;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static IVM.Schemas.장치통신;

namespace IVM.Schemas
{
    public class 검사자료 : BindingList<검사결과>
    {
        public delegate void 검사진행알림(검사결과 결과);
        public delegate void 수동검사수행(카메라구분 카메라, 검사결과 결과);
        public event 검사진행알림 검사완료알림;
        public event 수동검사수행 수동검사알림;

        [JsonIgnore]
        public static TranslationAttribute 로그영역 = new TranslationAttribute("Inspection", "검사내역");
        [JsonIgnore]
        private TranslationAttribute 저장오류 = new TranslationAttribute("An error occurred while saving the data.", "자료 저장중 오류가 발생하였습니다.");
        [JsonIgnore]
        private 검사결과테이블 테이블 = null;
        [JsonIgnore]
        private Dictionary<Int32, 검사결과> 검사스플 = new Dictionary<Int32, 검사결과>();
        [JsonIgnore]
        public 검사결과 수동검사;

        public void Init()
        {
            this.AllowEdit = true;
            this.AllowRemove = true;
            this.테이블 = new 검사결과테이블();
            this.Load();
            this.수동검사초기화();
            Global.환경설정.모델변경알림 += 모델변경알림;
        }

        public Boolean Close()
        {
            if (this.테이블 == null) return true;
            this.테이블.Save();
            this.테이블.자료정리(Global.환경설정.결과보관);
            return true;
        }

        private void 수동검사초기화()
        {
            this.수동검사 = new 검사결과();
            this.수동검사.검사코드 = 9999;
            this.수동검사.Reset();
        }

        public void Save() => this.테이블.Save();
        public void Load() => this.Load(DateTime.Today, DateTime.Today);
        public void Load(DateTime 시작, DateTime 종료)
        {
            this.Clear();
            this.RaiseListChangedEvents = false;
            List<검사결과> 자료 = this.테이블.Select(시작, 종료);

            List<Int32> 대상 = Global.장치통신.검사중인항목();
            자료.ForEach(검사 =>
            {
                this.Add(검사);
                // 검사스플 생성
                if (검사.측정결과 < 결과구분.ER && 대상.Contains(검사.검사코드) && !this.검사스플.ContainsKey(검사.검사코드))
                    this.검사스플.Add(검사.검사코드, 검사);
            });
            this.RaiseListChangedEvents = true;
            this.ResetBindings();
        }

        public 검사결과 GetItem(DateTime 일자, 모델구분 모델, Int32 코드) => this.테이블.Select(일자, 모델, 코드);
        public 검사결과 GetItem(DateTime 시작, DateTime 종료, 모델구분 모델, String 큐알, String serial) => this.테이블.Select(시작, 종료, 모델, 큐알, serial);

        public List<검사결과> GetData(DateTime 시작, DateTime 종료, 모델구분 모델) => this.테이블.Select(시작, 종료, 모델);
        private void 모델변경알림(모델구분 모델코드) => this.수동검사초기화();

        private void 자료추가(검사결과 결과)
        {
            this.Insert(0, 결과);
            if (Global.장치상태.자동수동)
                this.테이블.Add(결과);
            // 저장은 State 에서
        }

        public void 검사항목제거(List<검사정보> 자료) => this.테이블.Remove(자료);
        public Boolean 결과삭제(검사결과 정보)
        {
            this.Remove(정보);
            return this.테이블.Delete(정보) > 0;
        }
        public Boolean 결과삭제(검사결과 결과, 검사정보 정보)
        {
            결과.검사내역.Remove(정보);
            return this.테이블.Delete(정보) > 0;
        }
        public 검사결과 결과조회(DateTime 일자, 모델구분 모델, Int32 코드) => this.테이블.Select(일자, 모델, 코드);

        public void 검사일시추출실행(int numberOfResults, int numberOfProducts) => this.테이블.검사일시추출(numberOfResults, numberOfProducts);

        #region 검사로직
        // PLC에서 검사번호 요청 시 새 검사 자료를 생성하여 스플에 넣음
        public 검사결과 검사시작(Int32 검사코드)
        {
            if (!Global.장치상태.자동수동)
            {
                this.수동검사.Reset();
                return this.수동검사;
            }
            검사결과 검사 = 검사항목찾기(검사코드, true);
            if (검사 == null)
            {
                검사 = new 검사결과() { 검사코드 = 검사코드 };
                검사.Reset();
                this.자료추가(검사);
                this.검사스플.Add(검사.검사코드, 검사);
                Global.정보로그(로그영역.GetString(), $"검사시작", $"[{(Int32)Global.환경설정.선택모델} - {검사.검사코드}] 신규검사 시작.", false);
            }

            return 검사;
        }

        public 검사결과 하부큐알리딩수행(Int32 검사코드)
        {
            List<String> 하부QR = new List<String>();
            검사결과 검사 = 검사항목찾기(검사코드);
            if (검사 == null) return null;
            하부QR.Add(Global.하부큐알리더.리딩시작(검사));
            하부QR.Add(Global.하부큐알리더2.리딩시작(검사));

            검사.큐알내용 = $"{하부QR[0]}{하부QR[1]}";

            return 검사;
        }

        public void 하부큐알리딩수행종료()
        {
            Global.하부큐알리더.리딩종료();
            Global.하부큐알리더2.리딩종료();
        }

        public 검사결과 상부큐알리딩수행(Int32 검사코드)
        {
            검사결과 검사 = 검사항목찾기(검사코드);
            if (검사 == null) return null;
            Global.상부큐알리더.리딩시작(검사);
            return 검사;
        }

        public void 상부큐알리딩수행종료()
        {
            Global.상부큐알리더.리딩종료();
        }
        public Boolean 커버조립명령(Int32 검사코드)
        {
            검사결과 검사 = 검사항목찾기(검사코드);
            if (검사 == null) return false;
            결과구분 커버조립전결과 = 검사.측정결과;
            if (커버조립전결과 == 결과구분.OK || 커버조립전결과 == 결과구분.PS) return true;
            return false;
        }


        public Dictionary<Int32, Int32> 큐알중복횟수(String 큐알코드, Int32[] indexs) => this.테이블.큐알중복횟수(큐알코드, indexs);

        public 검사결과 평탄검사수행(Int32 검사코드, Dictionary<센서항목, Single> 자료)
        {
            검사결과 검사 = 검사항목찾기(검사코드);
            if (검사 == null || 자료 == null || 자료.Count < 1) return 검사;
            foreach (var s in 자료)
                검사.SetResult(s.Key.ToString(), s.Value);

            return 검사;
        }
        //public 검사결과 외폭검사수행(Int32 검사코드, Dictionary<센서항목, Single> 자료)
        //{
        //    검사결과 검사 = 검사항목찾기(검사코드);
        //    if (검사 == null || 자료.Count < 1) return null;

        //    Random rnd = new Random();
        //    foreach (var s in 자료)
        //    {
        //        Double value = s.Value;
        //        // 임시로 값 생성
        //        value = 108.6 + Math.Round(rnd.NextDouble() / 5, 3);
        //        검사.SetResult(s.Key.ToString(), value);
        //    }
        //    return 검사;
        //}
        public 검사결과 검사결과계산(Int32 검사코드)
        {
            if (검사코드 < 1) return null;
            검사결과 검사 = null;
            if (Global.장치상태.자동수동)
            {
                검사 = this.검사항목찾기(검사코드);
                if (검사 == null)
                {
                    Global.오류로그(로그영역.GetString(), "결과계산", $"[{(Int32)Global.환경설정.선택모델}.{검사코드}] 해당 검사가 없습니다.", false);
                    return null;
                }
                검사.결과계산();
                Debug.WriteLine("수량추가전");
                Global.모델자료.수량추가(검사.모델구분, 검사.측정결과);
                Debug.WriteLine("수량추가후");
                this.검사스플.Remove(검사코드);
            }
            else
            {
                검사 = this.수동검사;
                검사.결과계산();
            }

            Debug.WriteLine("결과계산완료");
            return 검사;
        }

        public void 검사완료알림함수(검사결과 결과)=> this.검사완료알림?.Invoke(결과);

        public void 검사수행알림(검사결과 검사) => this.검사완료알림?.Invoke(검사);
        public void 수동검사결과(카메라구분 카메라, 검사결과 검사)
        {
            this.검사완료알림?.Invoke(검사);
            this.수동검사알림?.Invoke(카메라, 검사);
        }

        // 현재 검사중인 정보를 검색
        public 검사결과 검사항목찾기(Int32 검사코드, Boolean 신규여부 = false)
        {
            if (!Global.장치상태.자동수동) return this.수동검사;
            검사결과 검사 = null;
            if (검사코드 > 0 && this.검사스플.ContainsKey(검사코드))
                검사 = this.검사스플[검사코드];
            if (검사 == null && !신규여부)
                Global.오류로그(로그영역.GetString(), "제품검사", $"[{검사코드}] Index가 없습니다.", true);
            return 검사;
        }

        public 검사결과 현재검사찾기()
        {
            if (!Global.장치상태.자동수동) return this.수동검사;
            if (this.검사스플.Count < 1) return this.수동검사;
            return this.검사스플.Last().Value;
        }

        public void ResetItem(검사결과 검사)
        {
            if (검사 == null) return;
            this.ResetItem(this.IndexOf(검사));
        }
        #endregion
    }


    public class 검사결과테이블 : Data.BaseTable
    {
        private TranslationAttribute 로그영역 = new TranslationAttribute("Inspection Data", "검사자료");
        private TranslationAttribute 삭제오류 = new TranslationAttribute("An error occurred while deleting data.", "자료 삭제중 오류가 발생하였습니다.");
        private DbSet<검사결과> 검사결과 { get; set; }
        private DbSet<검사정보> 검사정보 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<검사결과>().Property(e => e.모델구분).HasConversion(new EnumToNumberConverter<모델구분, Int32>());
            modelBuilder.Entity<검사결과>().Property(e => e.측정결과).HasConversion(new EnumToNumberConverter<결과구분, Int32>());
            modelBuilder.Entity<검사결과>().Property(e => e.CTQ결과).HasConversion(new EnumToNumberConverter<결과구분, Int32>());
            modelBuilder.Entity<검사결과>().Property(e => e.외관결과).HasConversion(new EnumToNumberConverter<결과구분, Int32>());

            modelBuilder.Entity<검사정보>().HasKey(e => new { e.검사일시, e.검사항목 });
            modelBuilder.Entity<검사정보>().Property(e => e.검사그룹).HasConversion(new EnumToNumberConverter<검사그룹, Int32>());
            modelBuilder.Entity<검사정보>().Property(e => e.검사항목).HasConversion(new EnumToNumberConverter<검사항목, Int32>());
            modelBuilder.Entity<검사정보>().Property(e => e.검사장치).HasConversion(new EnumToNumberConverter<장치구분, Int32>());
            modelBuilder.Entity<검사정보>().Property(e => e.결과분류).HasConversion(new EnumToNumberConverter<결과분류, Int32>());
            modelBuilder.Entity<검사정보>().Property(e => e.측정단위).HasConversion(new EnumToNumberConverter<단위구분, Int32>());
            modelBuilder.Entity<검사정보>().Property(e => e.측정결과).HasConversion(new EnumToNumberConverter<결과구분, Int32>());
            base.OnModelCreating(modelBuilder);
        }

        public void Save()
        {
            try { this.SaveChanges(); }
            catch (Exception ex) { Debug.WriteLine(ex.ToString(), "자료저장"); }
        }

        public void SaveAsync()
        {
            try { this.SaveChangesAsync(); }
            catch (Exception ex) { Debug.WriteLine(ex.ToString(), "자료저장"); }
        }

        public void Add(검사결과 정보)
        {
            this.검사결과.Add(정보);
            this.검사정보.AddRange(정보.검사내역);
        }

        public void Remove(List<검사정보> 자료) => this.검사정보.RemoveRange(자료);

        public List<검사결과> Select() => this.Select(DateTime.Today);
        public List<검사결과> Select(DateTime 날짜)
        {
            DateTime 시작 = new DateTime(날짜.Year, 날짜.Month, 날짜.Day);
            return this.Select(시작, 시작);
        }
        public List<검사결과> Select(DateTime 시작, DateTime 종료, 모델구분 모델 = 모델구분.None, Int32 코드 = 0, String 큐알 = null, String serial = null)
        {
            IQueryable<검사결과> query1 =
                from l in 검사결과
                where l.검사일시 >= 시작 && l.검사일시 < 종료.AddDays(1)
                where (코드 <= 0 || l.검사코드 == 코드)
                where (모델 == 모델구분.None || l.모델구분 == 모델)
                where (String.IsNullOrEmpty(큐알) || l.큐알내용 == 큐알)
                where (String.IsNullOrEmpty(serial) || l.큐알내용.Contains(serial))
                orderby l.검사일시 descending
                select l;
            List<검사결과> 자료 = query1.AsNoTracking().ToList();

            IQueryable<검사정보> query2 =
                from d in 검사정보
                join l in 검사결과 on d.검사일시 equals l.검사일시
                where l.검사일시 >= 시작 && l.검사일시 < 종료.AddDays(1)
                where (코드 <= 0 || l.검사코드 == 코드)
                where (모델 == 모델구분.None || l.모델구분 == 모델)
                where (String.IsNullOrEmpty(큐알) || l.큐알내용 == 큐알)
                where (String.IsNullOrEmpty(serial) || l.큐알내용.Contains(serial))
                orderby d.검사일시 descending
                orderby d.검사항목 ascending
                select d;
            List<검사정보> 정보 = query2.AsNoTracking().ToList();

            자료.ForEach(l => {
                l.AddRange(정보.Where(d => d.검사일시 == l.검사일시).ToList());
            });
            return 자료;
        }
        public 검사결과 Select(DateTime 일자, 모델구분 모델, Int32 코드) => this.Select(일자, 일자, 모델, 코드).FirstOrDefault();
        public 검사결과 Select(DateTime 시작, DateTime 종료, 모델구분 모델, String 큐알, String serial) => this.Select(시작, 종료, 모델, 0, 큐알, serial).FirstOrDefault();

        public Int32 Delete(검사결과 정보)
        {
            String Sql = $"DELETE FROM inspd WHERE idwdt = @idwdt;\nDELETE FROM inspl WHERE ilwdt = @ilwdt;";
            try
            {
                int AffectedRows = 0;
                using (NpgsqlCommand cmd = new NpgsqlCommand(Sql, this.DbConn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@idwdt", 정보.검사일시));
                    cmd.Parameters.Add(new NpgsqlParameter("@ilwdt", 정보.검사일시));
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    AffectedRows = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
                return AffectedRows;
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역.GetString(), Localization.삭제.GetString(), $"{삭제오류.GetString()}\r\n{ex.Message}", true);
            }
            return 0;
        }

        public Int32 Delete(검사정보 정보)
        {
            String Sql = $"DELETE FROM inspd WHERE idwdt = @idwdt AND idnum = @idnum";
            try
            {
                Int32 AffectedRows = 0;
                using (NpgsqlCommand cmd = new NpgsqlCommand(Sql, this.DbConn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@idwdt", 정보.검사일시));
                    cmd.Parameters.Add(new NpgsqlParameter("@idnum", 정보.검사항목));
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    AffectedRows = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
                return AffectedRows;
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역.GetString(), Localization.삭제.GetString(), $"{삭제오류.GetString()}\r\n{ex.Message}", true);
            }
            return 0;
        }

        public Int32 자료정리(Int32 일수)
        {
            DateTime 일자 = DateTime.Today.AddDays(-일수);
            String day = Utils.FormatDate(일자, "{0:yyyy-MM-dd}");
            String sql = $"DELETE FROM inspd WHERE idwdt < DATE('{day}');\nDELETE FROM inspl WHERE ilwdt < DATE('{day}');";
            try
            {
                int AffectedRows = 0;
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, this.DbConn))
                {
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    AffectedRows = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
                return AffectedRows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Global.오류로그(로그영역.GetString(), "Remove Datas", ex.Message, false);
            }
            return -1;
        }

        public Dictionary<Int32, Int32> 큐알중복횟수(String qrcode, Int32[] indexs)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>();
            if (!Global.큐알검증.중복체크 || indexs.Length < 1) return result;
            DateTime 시작 = DateTime.Today.AddDays(-Global.큐알검증.중복일수);
            String Sql = $"SELECT * FROM qr_duplicated('{qrcode}', ARRAY[{String.Join(",", indexs)}]::integer[], '{시작.ToString("yyyy-MM-dd")}');";
            try
            {
                DateTime sday = new DateTime(시작.Year, 시작.Month, 시작.Day);
                using (NpgsqlCommand cmd = new NpgsqlCommand(Sql, this.DbConn))
                {
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(reader.GetInt32(0), reader.GetInt32(1));
                    }
                    cmd.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역.GetString(), "중복검사", $"{ex.Message}", true);
            }
            return result;
        }

        //반복작업을 피하기 위해 추가 For R&R

        public void 검사일시추출(int numberOfResults, int numberOfProducts)
        {
            Debug.WriteLine("추출시작");


            DateTime today = DateTime.Today;


            // 오늘 날짜 기준으로 최신 데이터부터 필터링
            var filteredResults = this.검사결과
                .Where(x => x.검사일시 >= today)
                .OrderByDescending(x => x.검사일시)
                .ToList(); // 메모리로 로드하여 인덱스를 사용할 수 있도록 변환


            int sheetnum = numberOfProducts;
            for (int i = 0; i < numberOfProducts; i++)
            {
                List<List<decimal>> result = new List<List<decimal>>();
                var groupedResults = new List<DateTime>();
                var group = filteredResults
                    .Where((x, index) => (index % numberOfProducts) == i)
                    .Take(numberOfResults);

                groupedResults.AddRange(group.Select(x => x.검사일시));

                foreach (var 검사일시 in groupedResults)
                {

                    Console.WriteLine($"검사일시: {검사일시} {sheetnum}");

                    // 해당 검사일시에 대한 inspd 데이터 조회
                    var inspdData = this.검사정보
                        .Where(x => x.검사일시 == 검사일시)
                        .OrderBy(x => x.검사항목)
                        .ToList();
                    result.Add(inspdData.Select(x => x.결과값).ToList());
                    foreach (var data in inspdData)
                    {
                        Console.WriteLine($"검사일시: {data.검사일시}, 결과값: {data.결과값}");
                    }


                    // 행과 열을 전치하여 새로운 데이터 구조 생성
                    var transposedResults = TransposeList(result);

                    // CSV 파일 경로

                    string csvFileFolder = "C:\\IVM\\RandR";
                    string csvFileName =$"GageR&R_{sheetnum}_{DateTime.Now.ToString("yyMMddHHmmss")}.csv";


                    //var csvFilePath = $"C:\\IVM\\RandR\\GageR&R_{sheetnum}_{DateTime.Now.ToString("yyMMddHHmmss")}.csv";


                    // 폴더 없으면 만들고
                    if (!Directory.Exists(csvFileFolder)) Directory.CreateDirectory(csvFileFolder);

                    

                    // CSV 파일 경로
                    var csvFilePath = Path.Combine(csvFileFolder, csvFileName);

                    // CSV 파일 쓰기
                    using (var writer = new StreamWriter(csvFilePath, false, System.Text.Encoding.UTF8))
                    {
                        foreach (var row in transposedResults)
                        {
                            writer.WriteLine(string.Join(",", row));
                        }
                    }


                }
                sheetnum--;
            }

            Debug.WriteLine("추출끝");
            return;
        }


        // 리스트 전치 함수(행과열바꾸기)
        public static List<List<decimal>> TransposeList(List<List<decimal>> original)
        {
            var transposed = new List<List<decimal>>();

            // 열 개수 결정
            int columns = original.Count;
            if (columns == 0)
                return transposed;

            // 행 개수 결정
            int rows = original[0].Count;

            // 전치
            for (int row = 0; row < rows; row++)
            {
                var newRow = new List<decimal>();
                for (int col = 0; col < columns; col++)
                {
                    newRow.Add(original[col][row]);
                }
                transposed.Add(newRow);
            }

            return transposed;
        }
    }
}
