using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace IVM.Schemas
{
    public class VDA590TPA3D : Viewport
    {
        #region 초기화
        public override String StlPath => Global.환경설정.기본경로;
        public override String StlFile => Path.Combine(StlPath, "VDA590TPA.stl");
        public override Double Scale => 1;
        internal override void LoadStl()
        {
            if (!File.Exists(StlFile)) return;
            StLReader reader = new StLReader();
            Model3DGroup groups = reader.Read(StlFile);
            MainModel = groups.Children[0] as GeometryModel3D;
            //Debug.WriteLine(groups.Children.Count, "Groups Count");

            Point3D p = Center3D();
            Transform3DGroup transform = new Transform3DGroup();
            transform.Children.Add(new TranslateTransform3D(p.X * Scale, p.Y * Scale, 0 * Scale));
            transform.Children.Add(new ScaleTransform3D(Scale, Scale, Scale));
            MainModel.Transform = transform;
            //MainModel.Transform = new TranslateTransform3D(p.X, p.Y, 0);
            //Debug.WriteLine(p.ToString(), "p");
            //Debug.WriteLine(MainModel.Transform.Value.ToString(), "Transform");

            MainModel.SetName(nameof(MainModel));
            MainModel.Material = FrontMaterial;
            MainModel.BackMaterial = BackMaterial;
            ModelGroup.Children.Add(MainModel);
        }
        #endregion

        #region 기본 설정
        List<Base3D> InspItems = new List<Base3D>();
        internal String InspectionName(검사항목 항목)
        {
            검사정보 정보 = Global.모델자료.선택모델.검사설정.GetItem(항목);
            if (정보 == null) return String.Empty;
            return 정보.검사명칭;
        }
        internal override void InitModel()
        {
            if (MainModel == null) return;
            //Children.Add(new GridLinesVisual3D
            //{
            //    MajorDistance = 10, // 주 그리드 간격
            //    MinorDistance = 5,  // 보조 그리드 간격
            //    Thickness = 1, // Scale,    // 그리드 두께
            //    Center = new Point3D(0, 0, 0),
            //    Material = GridMaterial,
            //});

            //Rect3D r = MainModel.Bounds;
            //Debug.WriteLine($"{r.SizeY}, {r.SizeX}, {r.SizeZ}", "Rectangle3D"); // 225, 564.40, 29.5
            //Double hx = r.SizeX / 2;
            //Double hy = r.SizeY / 2;
            //Double hz = r.SizeZ / 2;
            //Double zz = r.SizeZ;
            //Double tz = 32;

            //AddText3D(new Point3D(-hx - 60, 0, 0), "R", 48, MajorColors.FrameColor);
            //AddText3D(new Point3D(+hx + 60, 0, 0), "F", 48, MajorColors.FrameColor);
            //AddArrowLine(new Point3D(-hx, 0, tz), new Point3D(hx, 0, tz), MajorColors.FrameColor); // Front ~ Rear Center
            //AddArrowLine(new Point3D(0, -hy, tz), new Point3D(0, hy, tz), MajorColors.FrameColor); // Width Center
            //AddArrowLine(new Point3D(0, -hy - 0.5,  0), new Point3D(0, -hy - 0.5, zz), MajorColors.FrameColor); // Front ~ Rear Center
            //AddArrowLine(new Point3D(0, +hy + 0.5,  0), new Point3D(0, +hy + 0.5, zz), MajorColors.FrameColor); // Width Center

            Rect3D r = MainModel.Bounds;
            Debug.WriteLine($"{r.SizeY}, {r.SizeX}, {r.SizeZ}", "Rectangle3D"); // 217, 562.16
            Double hx = r.SizeX / 2;
            Double hy = r.SizeY / 2;
            Double tz = 108.2;
            Double offset = 5;

            AddText3D(new Point3D(-hx - 60, 0, 55), "R", 48, MajorColors.FrameColor);
            AddText3D(new Point3D(+hx + 60, 0, 55), "F", 48, MajorColors.FrameColor);
            AddArrowLine(new Point3D(-hx, 0, tz + offset), new Point3D(hx, 0, tz + offset * 2), MajorColors.FrameColor); // Front ~ Rear Center
            AddArrowLine(new Point3D(0, -hy, tz + offset), new Point3D(0, hy, tz + offset * 2), MajorColors.FrameColor); // Width Center

            InspItems.Add(new Label3D(검사항목.f1) { Point = new Point3D(-228.50, +108.5, tz), Origin = new Point3D(-228.50, +108.5 + 20, tz), Name = "f01", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f2) { Point = new Point3D(-183.75, +108.5, tz), Origin = new Point3D(-183.75, +108.5 + 20, tz), Name = "f02", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f3) { Point = new Point3D(-93.75, +108.5, tz), Origin = new Point3D(-93.75, +108.5 + 20, tz), Name = "f03", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f4) { Point = new Point3D(-46.15, +108.5, tz), Origin = new Point3D(-46.15, +108.5 + 20, tz), Name = "f04", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f5) { Point = new Point3D(+46.15, +108.5, tz), Origin = new Point3D(+46.15, +108.5 + 20, tz), Name = "f05", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f6) { Point = new Point3D(+93.75, +108.5, tz), Origin = new Point3D(+93.75, +108.5 + 20, tz), Name = "f06", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f7) { Point = new Point3D(+183.75, +108.5, tz), Origin = new Point3D(+183.75, +108.5 + 20, tz), Name = "f07", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f8) { Point = new Point3D(+228.50, +108.5, tz), Origin = new Point3D(+228.50, +108.5 + 20, tz), Name = "f08", LabelStyle = NamePrintType.Up });
            InspItems.Add(new Label3D(검사항목.f9) { Point = new Point3D(-228.50, -108.5, tz), Origin = new Point3D(-228.50, -108.5 - 20, tz), Name = "f09", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f10) { Point = new Point3D(-183.75, -108.5, tz), Origin = new Point3D(-183.75, -108.5 - 20, tz), Name = "f10", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f11) { Point = new Point3D(-93.75, -108.5, tz), Origin = new Point3D(-93.75, -108.5 - 20, tz), Name = "f11", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f12) { Point = new Point3D(-46.15, -108.5, tz), Origin = new Point3D(-46.15, -108.5 - 20, tz), Name = "f12", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f13) { Point = new Point3D(+46.15, -108.5, tz), Origin = new Point3D(+46.15, -108.5 - 20, tz), Name = "f13", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f14) { Point = new Point3D(+93.75, -108.5, tz), Origin = new Point3D(+93.75, -108.5 - 20, tz), Name = "f14", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f15) { Point = new Point3D(+183.75, -108.5, tz), Origin = new Point3D(+183.75, -108.5 - 20, tz), Name = "f15", LabelStyle = NamePrintType.Down });
            InspItems.Add(new Label3D(검사항목.f16) { Point = new Point3D(+228.50, -108.5, tz), Origin = new Point3D(+228.50, -108.5 - 20, tz), Name = "f16", LabelStyle = NamePrintType.Down });

            InspItems.Add(new Width3D(검사항목.C1C5) { Point = new Point3D(-200, -38, tz + offset), PointS = new Point3D(-200, -hy, tz + offset), PointE = new Point3D(-200, hy, tz + offset), Name = "C1C5", LabelS = "C1", LabelE = "C5", LabelMargin = 6, LabelStyle = NamePrintType.Up });
            InspItems.Add(new Width3D(검사항목.C2C6) { Point = new Point3D(-75, -38, tz + offset), PointS = new Point3D(-75, -hy, tz + offset), PointE = new Point3D(-75, hy, tz + offset), Name = "C2C6", LabelS = "C2", LabelE = "C6", LabelMargin = 6, LabelStyle = NamePrintType.Up });
            InspItems.Add(new Width3D(검사항목.C3C7) { Point = new Point3D(+75, -39, tz + offset), PointS = new Point3D(+75, -hy, tz + offset), PointE = new Point3D(+75, hy, tz + offset), Name = "C3C7", LabelS = "C3", LabelE = "C7", LabelMargin = 6, LabelStyle = NamePrintType.Up });
            InspItems.Add(new Width3D(검사항목.C4C8) { Point = new Point3D(+200, -39, tz + offset), PointS = new Point3D(+200, -hy, tz + offset), PointE = new Point3D(+200, hy, tz + offset), Name = "C4C8", LabelS = "C4", LabelE = "C8", LabelMargin = 6, LabelStyle = NamePrintType.Up });

            //InspItems.Add(new Circle3D(검사항목.a1) { Point = new Point3D(-200, +105, tz), Name = "a1", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a2) { Point = new Point3D(0, 200, tz), Name = "a2", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a3) { Point = new Point3D(-105, 200, tz), Name = "a3", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a4) { Point = new Point3D(+90, 0, tz), Name = "a4", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a5) { Point = new Point3D(-90, 0, tz), Name = "a5", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a6) { Point = new Point3D(+105, -230, tz), Name = "a6", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a7) { Point = new Point3D(0, -230, tz), Name = "a7", LabelStyle = NamePrintType.Up });
            //InspItems.Add(new Circle3D(검사항목.a8) { Point = new Point3D(-105, -230, tz), Name = "a8", LabelStyle = NamePrintType.Up });

            InspItems.ForEach(e => e.Create(Children));
        }
        #endregion

        public virtual Color GetColor(결과구분 결과) => 결과 == 결과구분.OK ? MajorColors.GoodColor : MajorColors.BadColor;
        public void SetResults(검사결과 결과)
        {
            foreach (Base3D 항목 in InspItems)
            {
                검사정보 정보 = 결과.GetItem(항목.Type);
                if (정보 == null) continue;
                else 항목.Draw(정보.결과값, 정보.측정결과);
            }
        }
    }
}
