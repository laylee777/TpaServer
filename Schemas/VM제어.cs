using DevExpress.Utils;
using GraphicsSetModuleCs;
using ImageSourceModuleCs;
using MvUtils;
using OpenCvSharp;
using ShellModuleCs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using VM.Core;
using VM.PlatformSDKCS;

namespace DSEV.Schemas
{
    public enum Flow구분
    {
        표면검사 = 1,
    }
    public class VM제어 : List<VM플로우>
    {
        private static String 로그영역 = "검사도구";
        public delegate void 현재결과상태갱신(결과구분 구분);
        public event 현재결과상태갱신 결과상태갱신알림;
        public 모델구분 모델구분 = 모델구분.VDA590TPA;
        public String 도구명칭 { get => this.모델구분.ToString(); }
        //private String 도구파일 { get => Path.Combine(Global.환경설정.VM도구경로, $"{Utils.GetDescription(Global.환경설정.선택모델)}.sol"); }
        public String 도구파일 { get => Path.Combine(Global.환경설정.도구경로, ((Int32)모델구분).ToString("d2"), $"{도구명칭}.sol"); }

        public Boolean Init() => Load();

        public Boolean Load()
        {
            try
            {
                base.Clear();
                //VM Solution 불러오기
                if (File.Exists(도구파일))
                {
                    VmSolution.Load(도구파일, null);
                    Global.정보로그(로그영역, 로그영역, $"[ {Global.환경설정.선택모델} ] VmSolution파일 로드 완료.", false);
                }
                else
                {
                    Global.경고로그(로그영역, "솔루션 로드", $"솔루션 파일이 없습니다.", true);
                    return false;
                }
                VmSolution.Instance.DisableModulesCallback();

                foreach (Flow구분 플로우 in typeof(Flow구분).GetValues()) base.Add(new VM플로우(플로우));
                return true;
            }
            catch (Exception e)
            {
                Global.오류로그(로그영역, "솔루션 로드", $"솔루션을 로드하는 중 오류가 발생하였습니다. / {e.Message}", true);
                return false;
            }
        }

        public VM플로우 GetItem(Flow구분 구분)
        {
            foreach (VM플로우 플로우 in this)
                if (플로우.플로우 == 구분) return 플로우;
            return null;
        }
        public VM플로우 GetItem(카메라구분 구분)
        {
            foreach (VM플로우 플로우 in this)
                if ((int)플로우.플로우 == (int)구분) return 플로우;
            return null;
        }

        public void Close() => VmSolution.Instance.CloseSolution();
    }

    public class VM플로우
    {
        public Flow구분 플로우;
        public Boolean 결과;
        public String 로그영역 { get => $"플로우{플로우}"; }
        public VmProcedure Procedure;
        public ImageSourceModuleTool 입력이미지;
        public GraphicsSetModuleTool 출력이미지;
        public ShellModuleTool 플로우출력값;

        public VM플로우(Flow구분 플로우)
        {
            this.플로우 = 플로우;
            this.결과 = false;
            this.Init();
            if (출력이미지 != null)
                this.출력이미지.EnableResultCallback();
        }

        public void Init()
        {
            this.Procedure = VmSolution.Instance[this.플로우.ToString()] as VmProcedure;

            if (Procedure != null)
            {
                this.입력이미지 = this.Procedure["InputImage"] as ImageSourceModuleTool;
                this.출력이미지 = this.Procedure["OutputImage"] as GraphicsSetModuleTool;
                this.플로우출력값 = this.Procedure["Resulte"] as ShellModuleTool;

                if (this.입력이미지 != null) 
                    this.입력이미지.ModuParams.ImageSourceType = ImageSourceParam.ImageSourceTypeEnum.SDK;
            }
        }

        public Boolean Run(Mat mat, ImageBaseData imageBaseData)
        {
            try
            {
                this.결과 = false;
                if (this.입력이미지 == null)
                {
                    Global.경고로그(로그영역, "검사오류", $"[{this.플로우}] VM 검사 모델이 없습니다.", true);
                    return false;
                }

                imageBaseData = mat == null ? imageBaseData : MatToImageBaseData(mat);
                if (imageBaseData != null)
                    this.입력이미지.SetImageData(imageBaseData);

                this.Procedure.Run();

                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} : 표면검사 끝");

                return this.결과;
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역, "검사오류", $"[{this.플로우}] 검사 오류 / {ex.Message}", true);
                return false;
            }
        }

        private Boolean GetResult(Flow구분 구분)
        {
            ShellModuleTool shell = Global.VM제어.GetItem(구분).플로우출력값;
            String str = "";
            List<VmIO> t = shell.Outputs[6].GetAllIO();
            String name = t[0].UniqueName.Split('%')[1];
            if (t[0].Value != null)
            {
                str = ((ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[])t[0].Value)[0].strValue;
            }

            Boolean resBool = str == "0" ? true : false;

            return resBool;
        }

        private void SetResult(Flow구분 플로우) //0이면 Front, 1이면 Rear 
        {
            ShellModuleTool shell = Global.VM제어.GetItem(플로우).플로우출력값;

            Int32 startIndex = 6;
            for (int lop = startIndex; lop < shell.Outputs.Count; lop++)
            {
                List<VmIO> t = shell.Outputs[lop].GetAllIO();
                String name = t[0].UniqueName.Split('%')[1];
                if (t[0].Value != null)
                {
                    String str = ((ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[])t[0].Value)[0].strValue;
                    if (str == null) return;
                    try
                    {
                        String[] vals = str.Split(';');
                        Boolean ok = false;
                        Single val = Single.NaN;
                        if (!String.IsNullOrEmpty(vals[0])) val = Convert.ToSingle(vals[0]);
                        if (vals.Length > 1) ok = MvUtils.Utils.IntValue(vals[1]) == 1;
                        //Global.검사자료.항목검사(this.플로우, name, val);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message, name);
                    }
                }
            }
        }
        private ImageBaseData MatToImageBaseData(Mat mat)
        {
            if (mat.Channels() != 1) return null;
            ImageBaseData imageBaseData;
            uint dataLen = (uint)(mat.Width * mat.Height * mat.Channels());
            byte[] buffer = new byte[dataLen];
            Marshal.Copy(mat.Ptr(0), buffer, 0, buffer.Length);
            imageBaseData = new ImageBaseData(buffer, dataLen, mat.Width, mat.Height, (int)VMPixelFormat.VM_PIXEL_MONO_08);
            return imageBaseData;
        }
    }
}
