using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;

namespace DSEV.Schemas
{
    public static class PlaneDistanceCalculator
    {
        public static List<Single> Calculate(List<Point3f> queryPoints) => Calculate(queryPoints, queryPoints);
        public static List<Single> Calculate(List<Point3f> planePoints, List<Point3f> queryPoints)
        {
            // Z 값이 가장 작은 3개의 포인트 선택
            List<Point3f> selectedPoints = planePoints.OrderBy(p => p.Z).Take(3).ToList();

            // 평면 정의
            Point3f point1 = selectedPoints[0];
            Point3f point2 = selectedPoints[1];
            Point3f point3 = selectedPoints[2];

            Single A = (point2.Y - point1.Y) * (point3.Z - point1.Z) - (point2.Z - point1.Z) * (point3.Y - point1.Y);
            Single B = (point2.Z - point1.Z) * (point3.X - point1.X) - (point2.X - point1.X) * (point3.Z - point1.Z);
            Single C = (point2.X - point1.X) * (point3.Y - point1.Y) - (point2.Y - point1.Y) * (point3.X - point1.X);

            Single length = (Single)Math.Sqrt(A * A + B * B + C * C);
            A /= length;
            B /= length;
            C /= length;

            Single D = -(A * point1.X + B * point1.Y + C * point1.Z);

            List<Single> distances = new List<Single>();
            queryPoints.ForEach(p => {
                Single distance = (A * p.X + B * p.Y + C * p.Z + D) / (Single)Math.Sqrt(A * A + B * B + C * C);
                distances.Add(distance);
            });

            return distances;
        }

        public static float[] CalculateDistances(Int32 queryPointCnt, float[,] planePoints, float[,] queryPoints)
        {
            var matrixX = DenseMatrix.OfArray(new float[,] {
                { planePoints[0, 0], planePoints[0, 1], 1 },
                { planePoints[1, 0], planePoints[1, 1], 1 },
                { planePoints[2, 0], planePoints[2, 1], 1 },
                { planePoints[3, 0], planePoints[3, 1], 1 }
            });

            var vectorZ = DenseVector.OfArray(new float[] {
                planePoints[0, 2],
                planePoints[1, 2],
                planePoints[2, 2],
                planePoints[3, 2]
            });

            var result = matrixX.Solve(vectorZ);
            Single A = result[0];
            Single B = result[1];
            Single C = -1;
            Single D = result[2];
            Single[] distances = new Single[queryPointCnt];
            for (int i = 0; i < queryPointCnt; i++)
            {
                Single x = queryPoints[i, 0];
                Single y = queryPoints[i, 1];
                Single z = queryPoints[i, 2];
                Single distance = (A * x + B * y + C * z + D) / (Single)Math.Sqrt(A * A + B * B + C * C);
                distances[i] = distance;
            }

            return distances;
        }

        public static float[] 편차계산(List<Single> 센서값)
        {
            Single[] distances = new Single[센서값.Count];
            for (int lop = 0; lop < 센서값.Count; lop++)
            {
                distances[lop] = 센서값[lop] * 2;
            }

            return distances;
        }

        public static float[] 편차계산(Int32 기준값, List<Single> 센서값)
        {
            Single[] distances = new Single[센서값.Count];
            for (int lop = 0; lop < 센서값.Count; lop++)
            {
                distances[lop] = 기준값 - 센서값[lop];
            }

            return distances;
        }

        public static Single FindMinMaxDiff(List<Single> items) => Math.Abs(items.Max() - items.Min());
        public static Single FindMinMaxDiff(Single[] arr) => Math.Abs(arr.Max() - arr.Min());


        public static Single FindAbsMaxDiff(List<Single> items, Single factor = 2) => Math.Max(Math.Abs(items.Max()), Math.Abs(items.Min())) * factor;
        public static Single FindAbsMaxDiff(Single[] arr) => Math.Max(Math.Abs(arr.Max()), Math.Abs(arr.Min())) * 2;


        public static Single FindAbsMaxDiff2(Single[] arr) => Math.Max(Math.Abs(arr.Max()), Math.Abs(arr.Min()));

        public static Single 선윤곽도계산(검사정보 정보1, 검사정보 정보2) => (Single)(Math.Max(Math.Abs(정보1.기준값 - 정보1.결과값), Math.Abs(정보2.기준값 - 정보2.결과값)) * 2);
    }
}
