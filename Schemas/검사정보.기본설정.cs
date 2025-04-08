using Cognex.VisionPro;
using IVM.Schemas;
using MvUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace IVM.Schemas
{
    public enum 카메라구분
    {
        [ListBindable(false)]
        None = 0,

        [Description("Cam1(Top Camera)")]
        Cam01 = 1,

        [Description("Cam2(Side Left)")]
        Cam02 = 2,
        [Description("Cam3(Side Right)")]
        Cam03 = 3,

        [Description("Cam4(Bottom Left)")]
        Cam04 = 4,
        [Description("Cam5(Bottom Right)")]
        Cam05 = 5,

        [Description("Cam6(Connector Left)")]
        Cam06 = 6,
        [Description("Cam7(Connector Right)")]
        Cam07 = 7,
    }

    // 카메라구분 과 번호 맞춤
    public enum 장치구분
    {
        [Description("None"), Camera(false)]
        None = 0,
        [Description("Cam01"), Camera(true)]
        Cam01 = 카메라구분.Cam01,
        [Description("Cam02"), Camera(true)]
        Cam02 = 카메라구분.Cam02,
        [Description("Cam03"), Camera(true)]
        Cam03 = 카메라구분.Cam03,
        [Description("Cam04"), Camera(true)]
        Cam04 = 카메라구분.Cam04,
        [Description("Cam05"), Camera(true)]
        Cam05 = 카메라구분.Cam05,
        [Description("Cam06"), Camera(true)]
        Cam06 = 카메라구분.Cam06,
        [Description("Cam07"), Camera(true)]
        Cam07 = 카메라구분.Cam07,


        [Description("Flatness"), Camera(false)]
        Flatness = 9,
        [Description("Spacing"), Camera(false)]
        Spacing = 10,
        [Description("QRCode"), Camera(false)]
        QRCode = 11,
        [Description("QRMark"), Camera(false)]
        QRMark = 12,
    }

    public enum 결과분류
    {
        None,
        Summary,
        Detail,
    }

    public enum 검사그룹
    {
        [Description("None"), Translation("None", "없음")]
        None,
        [Description("CTQ"), Translation("CTQ")]
        CTQ,
        [Description("Surface"), Translation("Surface", "외관검사")]
        Surface,
        [Description("Etc"), Translation("Etc", "기타")]
        Etc,
    }


    public enum 검사항목 : Int32
    {
        [Result(), ListBindable(false)]
        None,

        //제품 폭
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "C1C5")]
        C1C5 = 101,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "C2C6")]
        C2C6 = 102,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "C3C7")]
        C3C7 = 103,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "C4C8")]
        C4C8 = 104,


        //H1H2 윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "ShapeH1H2")]
        ShapeH1H2 = 201,

        //H1H2 윤곽도하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeH1H2, "h1")]
        h1 = 221,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeH1H2, "h2")]
        h2 = 222,

        //H3H4 윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "ShapeH3H4")]
        ShapeH3H4 = 301,

        //H3H4 윤곽도하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeH3H4, "h3")]
        h3 = 321,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeH3H4, "h4")]
        h4 = 322,


        //M1M3, K1K8 윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Flatness, None)]
        ShapeM1M3 = 401,
        //[Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Flatness, None)]
        //ShapeK1K8 = 402,

        //M1M3, K1K8 윤곽도하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, ShapeM1M3)]
        m1 = 421,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, ShapeM1M3)]
        m2 = 422,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, ShapeM1M3)]
        m3 = 423,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k1 = 424,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k2 = 425,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k3 = 426,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k4 = 427,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k5 = 428,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k6 = 429,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k7 = 430,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        k8 = 431,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        A1_R = 432,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        A2_R = 433,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        A3_R = 434,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness)]
        A4_R = 435,


        //가공부 높이
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f1")]
        f1 = 501,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f2")]
        f2 = 502,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f3")]
        f3 = 503,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f4")]
        f4 = 504,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f5")]
        f5 = 505,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f6")]
        f6 = 506,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f7")]
        f7 = 507,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam02, None, "f8")]
        f8 = 508,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f9")]
        f9 = 509,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f10")]
        f10 = 510,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f11")]
        f11 = 511,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f12")]
        f12 = 512,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f13")]
        f13 = 513,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f14")]
        f14 = 514,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f15")]
        f15 = 515,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam03, None, "f16")]
        f16 = 516,


        //가공부 폭
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g1")]
        g1 = 601,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g2")]
        g2 = 602,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g3")]
        g3 = 603,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g4")]
        g4 = 604,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g5")]
        g5 = 605,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g6")]
        g6 = 606,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g7")]
        g7 = 607,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam04, None, "g8")]
        g8 = 608,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g9")]
        g9 = 609,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g10")]
        g10 = 610,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g11")]
        g11 = 611,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g12")]
        g12 = 612,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g13")]
        g13 = 613,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g14")]
        g14 = 614,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g15")]
        g15 = 615,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam05, None, "g16")]
        g16 = 616,


        //바닥평면도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Flatness, None)]
        Flatness = 701,

        //바닥평면도 하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        A1_F = 721,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        A2_F = 722,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        A3_F = 723,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        A4_F = 724,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a1 = 725,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a2 = 726,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a3 = 727,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a4 = 728,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a5 = 729,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a6 = 730,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a7 = 731,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Flatness, Flatness)]
        a8 = 732,


        //하단부 큰 노치 윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "ShapeBigNotchLR")]
        ShapeBigNotchLR = 801,

        //하단부 큰 노치 윤곽도하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeBigNotchLR, "DistBigNotchR")]
        DistBigNotchR = 821,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeBigNotchLR, "DistBigNotchL")]
        DistBigNotchL = 822,


        //J1J2윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "ShapeJ1J2")]
        ShapeJ1J2 = 901,

        //J1J2윤곽도 하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeJ1J2, "j1")]
        j1 = 921,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeJ1J2, "j2")]
        j2 = 922,

        //J3J4윤곽도
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "ShapeJ3J4")]
        ShapeJ3J4 = 1001,
        //J3J4윤곽도 하위
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeJ3J4, "j3")]
        j3 = 1021,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Detail, 장치구분.Cam01, ShapeJ3J4, "j4")]
        j4 = 1022,


        //사이드노치 위치
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchPosFR")]
        NotchPosFR = 1101,//d1
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchPosRR")]
        NotchPosRR = 1102,//d3
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchPosFL")]
        NotchPosFL = 1103,//d4
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchPosRL")]
        NotchPosRL = 1104,//d6


        //중심 노치높이
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightCR")]
        NotchHeightCR = 1201, //d2
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightCL")]
        NotchHeightCL = 1202, //d5


        //중심 노치반폭
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthCR1")]
        NotchWidthCR1 = 1301, //d2
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthCR2")]
        NotchWidthCR2 = 1302,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthCL1")]
        NotchWidthCL1 = 1303, //d5
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthCL2")]
        NotchWidthCL2 = 1304,


        //사이드 노치높이
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthFR")]
        NotchWidthFR = 1401, //d1
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthRR")]
        NotchWidthRR = 1402, //d3
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthFL")]
        NotchWidthFL = 1403, //d4
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchWidthRL")]
        NotchWidthRL = 1404, //d6


        //사이드 노치반폭
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightFR1")]
        NotchHeightFR1 = 1501,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightFR2")]
        NotchHeightFR2 = 1502,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightRR1")]
        NotchHeightRR1 = 1503,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightRR2")]
        NotchHeightRR2 = 1504,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightFL1")]
        NotchHeightFL1 = 1505,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightFL2")]
        NotchHeightFL2 = 1506,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightRL1")]
        NotchHeightRL1 = 1507,
        [Result(피씨구분.Server, 검사그룹.CTQ, 결과분류.Summary, 장치구분.Cam01, None, "NotchHeightRL2")]
        NotchHeightRL2 = 1508,


        //커넥터상부
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.Cam06, None)]
        CntT = 1801,

        //커넥터상부 하위
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Detail, 장치구분.Cam06, CntT, "CntTL")]
        CntTL = 1821,
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Detail, 장치구분.Cam06, CntT, "CntTR")]
        CntTR = 1822,


        //커넥터하부
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.Cam07, None)]
        CntB = 1901,

        //커넥터하부 하위
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Detail, 장치구분.Cam07, CntB, "CntBL")]
        CntBL = 1921,
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Detail, 장치구분.Cam07, CntB, "CntBR")]
        CntBR = 1922,


        //하부QR검사
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        BottomQRCode1 = 2001,
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        BottomQRCode2 = 2002,

        //상부QR검사
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        TopQRCode1 = 2101,
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        TopQRCode2 = 2102,

        //QR검증
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        QrValidate = 2201,
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.QRCode, None)]
        QrDuplicated = 2202,


        // 기타

        //투입방향
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.Cam01, None, "InputDirection")]
        InputDirection = 2301,

        //노말미러 각인확인
        [Result(피씨구분.Server, 검사그룹.Etc, 결과분류.Summary, 장치구분.Cam01, None, "NomalMirror")]
        NomalMirror = 2303,


        // 상부 외관검사 라인 //

        //브라켓각인 검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None, None)]
        BraketMarking = 3401,

        //브라켓각인 검사 하위(위치)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos1")]
        BraketMarkingPos1 = 3421,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos2")]
        BraketMarkingPos2 = 3422,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos3")]
        BraketMarkingPos3 = 3423,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos4")]
        BraketMarkingPos4 = 3424,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos5")]
        BraketMarkingPos5 = 3425,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingPos6")]
        BraketMarkingPos6 = 3426,

        //브라켓각인 검사 하위(유무)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi1")]
        BraketMarkingExi1 = 3427,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi2")]
        BraketMarkingExi2 = 3428,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi3")]
        BraketMarkingExi3 = 3429,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi4")]
        BraketMarkingExi4 = 3430,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi5")]
        BraketMarkingExi5 = 3431,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, BraketMarking, "BraketMarkingExi6")]
        BraketMarkingExi6 = 3432,


        //직선각인 검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None, None)]
        LineMarking = 3501,

        //직선각인 검사 하위(위치)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, LineMarking, "LineMarkingPos1")]
        LineMarkingPos1 = 3521,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, LineMarking, "LineMarkingPos2")]
        LineMarkingPos2 = 3522,

        //직선각인 검사 하위(유무)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, LineMarking, "LineMarkingExi1")]
        LineMarkingExi1 = 3523,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, LineMarking, "LineMarkingExi2")]
        LineMarkingExi2 = 3524,


        //부호각인 검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None, None)]
        SignMarking = 3601,

        //직선각인 검사 하위(위치)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, SignMarking, "SignMarkingPos1")]
        SignMarkingPos1 = 3621,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, SignMarking, "SignMarkingPos2")]
        SignMarkingPos2 = 3622,

        //직선각인 검사 하위(유무)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, SignMarking, "SignMarkingExi1")]
        SignMarkingExi1 = 3623,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, SignMarking, "SignMarkingExi2")]
        SignMarkingExi2 = 3624,


        //이형지 검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None, None)]
        Sticker = 3701,

        //이형지 검사 하위(유무)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, StickerExi1, "StickerExi1")]
        StickerExi1 = 3721,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, StickerExi2, "StickerExi2")]
        StickerExi2 = 3722,



        //하부 외관검사 라인//

        //인슐레이션 검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None, None)]
        Insulation = 4001,

        //인슐레이션 검사 하위(유무)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationExi")]
        InsulationExi = 4021,

        //인슐레이션 검사 하위(뜯김)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationConditionL")]
        InsulationConditionL = 4022,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, Insulation, "InsulationConditionR")]
        InsulationConditionR = 4023,

        //인슐레이션 검사 하위(각인이탈)
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationPos1")]
        InsulationPos1 = 4024,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationPos2")]
        InsulationPos2 = 4025,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationPos3")]
        InsulationPos3 = 4026,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, Insulation, "InsulationPos4")]
        InsulationPos4 = 4027,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, Insulation, "InsulationPos5")]
        InsulationPos5 = 4028,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, Insulation, "InsulationPos6")]
        InsulationPos6 = 4029,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, Insulation, "InsulationPos7")]
        InsulationPos7 = 4030,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, Insulation, "InsulationPos8")]
        InsulationPos8 = 4031,



        // 표면검사 라인 //

        //상부표면검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None)]
        TopSurface = 6001,

        //상부표면검사 하위
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, TopSurface, "TopDent")]
        TopDent = 6021,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, TopSurface, "TopScratch")]
        TopScratch = 6022,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam01, TopSurface, "TopPollution")]
        TopPollution = 6023,


        //측면표면검사
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None)]
        SideSurface = 6101,

        //측면표면검사 하위
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam02, SideSurface, "SideDentL")]
        SideDentL = 6121,
        [Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam03, SideSurface, "SideDentR")]
        SideDentR = 6122,


        ////하부표면검사
        //[Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Summary, 장치구분.None)]
        //BottomSurface = 6001,

        ////하부표면검사 하위
        //[Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam04, BottomSurface, "BottomDentL")]
        //BottomDentL = 6021,
        //[Result(피씨구분.Server, 검사그룹.Surface, 결과분류.Detail, 장치구분.Cam05, BottomSurface, "BottomDentR")]
        //BottomDentR = 6022,
    }

    public enum 단위구분
    {
        [Description("mm")]
        mm = 0,
        [Description("OK/NG")]
        ON = 1,
        [Description("EA")]
        EA = 2,
        [Description("Grade")]
        GA = 3,
    }

    public enum 큐알등급
    {
        [Description("-")]
        X = 0,
        [Description("A")]
        A = 1,
        [Description("B")]
        B = 2,
        [Description("C")]
        C = 3,
        [Description("D")]
        D = 4,
        [Description("E")]
        E = 5,
        [Description("F")]
        F = 6,
    }

    public enum 결과구분
    {
        [Description("Waiting"), Translation("Waiting", "대기중")]
        WA = 0,
        //[Description("Measuring"), Translation("Measuring", "검사중")]
        //ME = 1,
        [Description("PS"), Translation("Pass", "통과")]
        PS = 2,
        [Description("ER"), Translation("Error", "오류")]
        ER = 3,
        [Description("NG"), Translation("NG", "불량")]
        NG = 5,
        [Description("OK"), Translation("OK", "양품")]
        OK = 7,
    }

    [Table("inspd")]
    public class 검사정보
    {
        [Column("idwdt", Order = 0), Required, Key, JsonProperty("idwdt"), Translation("Time", "검사일시")]
        public DateTime 검사일시 { get; set; } = DateTime.Now;
        [NotMapped, JsonProperty("idnam"), Translation("Name", "명칭")]
        public String 검사명칭 { get; set; } = String.Empty;
        [Column("iditm", Order = 1), Required, Key, JsonProperty("iditm"), Translation("Item", "검사항목")]
        public 검사항목 검사항목 { get; set; } = 검사항목.None;
        [Column("idgrp"), JsonProperty("idgrp"), Translation("Group", "검사그룹")]
        public 검사그룹 검사그룹 { get; set; } = 검사그룹.None;
        [Column("iddev"), JsonProperty("iddev"), Translation("Device", "검사장치")]
        public 장치구분 검사장치 { get; set; } = 장치구분.None;
        [Column("idcat"), JsonProperty("idcat"), Translation("Category", "결과유형")]
        public 결과분류 결과분류 { get; set; } = 결과분류.None;
        [Column("iduni"), JsonProperty("iduni"), Translation("Unit", "단위"), BatchEdit(true)]
        public 단위구분 측정단위 { get; set; } = 단위구분.mm;
        [Column("idstd"), JsonProperty("idstd"), Translation("Norminal", "기준값"), BatchEdit(true)]
        public Decimal 기준값 { get; set; } = 0m;
        [Column("idmin"), JsonProperty("idmin"), Translation("Min", "최소값"), BatchEdit(true)]
        public Decimal 최소값 { get; set; } = 0m;
        [Column("idmax"), JsonProperty("idmax"), Translation("Max", "최대값"), BatchEdit(true)]
        public Decimal 최대값 { get; set; } = 0m;
        [Column("idoff"), JsonProperty("idoff"), Translation("Offset", "보정값"), BatchEdit(true)]
        public Decimal 보정값 { get; set; } = 0m;
        [Column("idcal"), JsonProperty("idcal"), Translation("Calib(µm)", "교정(µm)"), BatchEdit(true)]
        public Decimal 교정값 { get; set; } = 0m;
        [Column("idmes"), JsonProperty("idmes"), Translation("Measure", "측정값")]
        public Decimal 측정값 { get; set; } = 0m;
        [Column("idval"), JsonProperty("idval"), Translation("Value", "결과값")]
        public Decimal 결과값 { get; set; } = 0m;
        [NotMapped, JsonProperty("idrel"), Translation("Real", "실측값")]
        public Decimal 실측값 { get; set; } = 0m;
        [Column("idres"), JsonProperty("idres"), Translation("Result", "판정")]
        public 결과구분 측정결과 { get; set; } = 결과구분.WA;
        [NotMapped, JsonProperty("idmag"), Translation("Margin"), BatchEdit(true)]
        public Decimal 마진값 { get; set; } = 0m;
        [NotMapped, JsonProperty("iduse"), Translation("Used", "검사"), BatchEdit(true)]
        public Boolean 검사여부 { get; set; } = true;

        [NotMapped, JsonIgnore]
        public Double 검사시간 = 0;
        [NotMapped, JsonIgnore]
        public String 변수명칭 = String.Empty;
        [NotMapped, JsonIgnore]
        public Boolean 카메라여부 = false;
        [NotMapped, JsonIgnore]
        public 검사항목 결과항목 = 검사항목.None;
        [NotMapped, JsonIgnore]
        public Int32 결과부호 = 1;

        public 검사정보() { }
        public 검사정보(검사정보 정보) { this.Set(정보); }

        public void Init()
        {
            this.카메라여부 = CameraAttribute.IsCamera(this.검사장치);
            ResultAttribute a = Utils.GetAttribute<ResultAttribute>(this.검사항목);
            this.변수명칭 = a.변수명칭;
            this.결과항목 = a.결과항목;
            this.결과부호 = a.결과부호;
        }

        public void Reset(DateTime? 일시 = null)
        {
            if (일시 != null) this.검사일시 = (DateTime)일시;
            this.측정값 = 0;
            this.결과값 = 0;
            this.측정결과 = 결과구분.WA;
        }
        public void Set(검사정보 정보)
        {
            if (정보 == null) return;
            foreach (PropertyInfo p in typeof(검사정보).GetProperties())
            {
                if (!p.CanWrite) continue;
                Object v = p.GetValue(정보);
                if (v == null) continue;
                p.SetValue(this, v);
            }
            this.Reset(null);
            this.Init();
        }

        public Boolean 교정계산()
        {
            if (this.측정값 <= 0) return false;
            this.교정값 = Convert.ToDecimal(Math.Round((this.실측값 - this.보정값) / this.측정값 * 1000, 9));
            return true;
        }

        public Boolean 교정계산2()
        {
            if (this.실측값 <= 0) return false;
            if (this.측정값 <= 0) return false;
            if (this.검사여부) this.교정값 = Convert.ToDecimal(Math.Round((this.실측값 / this.측정값 * 1000), 9));
            return true;
        }

        public 결과구분 결과계산()
        {
            Boolean ok = this.결과값 >= this.최소값 && this.결과값 <= this.최대값;
            this.측정결과 = ok ? 결과구분.OK : 결과구분.NG;
            return this.측정결과;
        }

        public String DisplayText(Decimal value)
        {
            if (this.측정단위 == 단위구분.EA) return Utils.FormatNumeric(value);
            if (this.측정단위 == 단위구분.ON) return value == 1 ? "OK" : "NG";
            return String.Empty;
        }

        private String[] AppearanceFields = new String[] { nameof(측정결과), nameof(최소값), nameof(최대값), nameof(기준값), nameof(결과값) };
        public void SetAppearance(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e == null || !AppearanceFields.Contains(e.Column.FieldName)) return;
            PropertyInfo p = typeof(검사정보).GetProperty(e.Column.FieldName);
            if (p.Name == nameof(결과값) || p.Name == nameof(측정결과))
                e.Appearance.ForeColor = 환경설정.ResultColor(this.측정결과);
            if (p.PropertyType != typeof(Decimal)) return;
            Object v = p.GetValue(this);
            if (v == null) return;
            String display = DisplayText((Decimal)v);
            if (!String.IsNullOrEmpty(display)) e.DisplayText = display;
        }
    }

    [Table("insuf")]
    public class 불량영역
    {
        [Column("iswdt"), Required, Key, JsonProperty("iswdt"), Translation("Time", "일시")]
        public DateTime 검사일시 { get; set; } = DateTime.Now;
        [Column("isdev"), JsonProperty("isdev"), Translation("Model", "모델")]
        public 장치구분 장치구분 { get; set; } = 장치구분.None;
        [Column("isitm"), JsonProperty("isitm"), Translation("Item", "검사항목")]
        public 검사항목 검사항목 { get; set; } = 검사항목.None;
        [Column("islef"), JsonProperty("islef"), Translation("X", "X")]
        public Double 가로중심 { get; set; } = 0;
        [Column("istop"), JsonProperty("istop"), Translation("Y", "Y")]
        public Double 세로중심 { get; set; } = 0;
        [Column("iswid"), JsonProperty("iswid"), Translation("Width", "Width")]
        public Double 가로길이 { get; set; } = 0;
        [Column("ishei"), JsonProperty("ishei"), Translation("Height", "Height")]
        public Double 세로길이 { get; set; } = 0;
        [Column("isrot"), JsonProperty("isrot"), Translation("Rotation", "Rotation")]
        public Double 회전각도 { get; set; } = 0;
        [Column("isske"), JsonProperty("isske"), Translation("Skew", "Skew")]
        public Double 기울임 { get; set; } = 0;

        public 불량영역() { }
        public 불량영역(카메라구분 카메라, 검사항목 항목, CogRectangleAffine 영역)
        {
            장치구분 = (장치구분)카메라;
            검사항목 = 항목;
            가로중심 = 영역.CenterX;
            세로중심 = 영역.CenterY;
            가로길이 = 영역.SideXLength;
            세로길이 = 영역.SideYLength;
            회전각도 = 영역.Rotation;
            기울임 = 영역.Skew;
        }
        public 불량영역(카메라구분 카메라, 검사항목 항목, List<Double> r)
        {
            장치구분 = (장치구분)카메라;
            검사항목 = 항목;
            가로중심 = r[0];
            세로중심 = r[1];
            가로길이 = r[2];
            세로길이 = r[3];
            회전각도 = r[4];
            기울임 = r[5];
        }
        public CogRectangleAffine GetRectangleAffine() => new CogRectangleAffine() { CenterX = 가로중심, CenterY = 세로중심, SideXLength = 가로길이, SideYLength = 세로길이, Rotation = 회전각도 };
        public CogRectangleAffine GetRectangleAffine(CogColorConstants color) { var r = GetRectangleAffine(); r.Color = color; return r; }
        public CogRectangleAffine GetRectangleAffine(CogColorConstants color, Int32 lineWidth) { var r = GetRectangleAffine(color); r.LineWidthInScreenPixels = lineWidth; return r; }
    }

    #region Attributes
    public class CameraAttribute : Attribute
    {
        public Boolean Whether = true;
        public CameraAttribute(Boolean cam) { Whether = cam; }

        public static Boolean IsCamera(장치구분 구분)
        {
            CameraAttribute a = Utils.GetAttribute<CameraAttribute>(구분);
            if (a == null) return false;
            return a.Whether;
        }
    }

    public class ResultAttribute : Attribute
    {
        public 피씨구분 피씨구분 = 피씨구분.Server;
        public 검사그룹 검사그룹 = 검사그룹.None;
        public 결과분류 결과분류 = 결과분류.None;
        public 장치구분 장치구분 = 장치구분.None;
        public 검사항목 결과항목 = 검사항목.None;
        public String 변수명칭 = String.Empty;
        public Int32 결과부호 = 1;
        public ResultAttribute() { }
        public ResultAttribute(피씨구분 피씨, 검사그룹 그룹, 결과분류 결과) { 피씨구분 = 피씨; 검사그룹 = 그룹; 결과분류 = 결과; }
        public ResultAttribute(피씨구분 피씨, 검사그룹 그룹, 결과분류 결과, 장치구분 장치) { 피씨구분 = 피씨; 검사그룹 = 그룹; 결과분류 = 결과; 장치구분 = 장치; }
        public ResultAttribute(피씨구분 피씨, 검사그룹 그룹, 결과분류 결과, 장치구분 장치, 검사항목 항목) { 피씨구분 = 피씨; 검사그룹 = 그룹; 결과분류 = 결과; 장치구분 = 장치; 결과항목 = 항목; }
        public ResultAttribute(피씨구분 피씨, 검사그룹 그룹, 결과분류 결과, 장치구분 장치, 검사항목 항목, String 변수) { 피씨구분 = 피씨; 검사그룹 = 그룹; 결과분류 = 결과; 장치구분 = 장치; 결과항목 = 항목; 변수명칭 = 변수; }
        public ResultAttribute(피씨구분 피씨, 검사그룹 그룹, 결과분류 결과, 장치구분 장치, 검사항목 항목, String 변수, Int32 부호) { 피씨구분 = 피씨; 검사그룹 = 그룹; 결과분류 = 결과; 장치구분 = 장치; 결과항목 = 항목; 변수명칭 = 변수; 결과부호 = 부호; }

        public static String VarName(검사항목 항목)
        {
            ResultAttribute a = Utils.GetAttribute<ResultAttribute>(항목);
            if (a == null) return String.Empty;
            return a.변수명칭;
        }
        public static Int32 ValueFactor(검사항목 항목)
        {
            ResultAttribute a = Utils.GetAttribute<ResultAttribute>(항목);
            if (a == null) return 1;
            return a.결과부호;
        }
    }
    #endregion
}