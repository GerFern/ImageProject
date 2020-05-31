using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ModelBase
{
    public static class PointExtensions
    {
        private static int VectorMul(int ax, int ay, int bx, int by) //векторное произведение
        {
            return ax * by - bx * ay;
        }

        private static float VectorMul(float ax, float ay, float bx, float by) //векторное произведение
        {
            return ax * by - bx * ay;
        }

        public static bool AreCrossing(Point p1, Point p2, Point p3, Point p4)//проверка пересечения
        {
            int v1 = VectorMul(p4.X - p3.X, p4.Y - p3.Y, p1.X - p3.X, p1.Y - p3.Y);
            int v2 = VectorMul(p4.X - p3.X, p4.Y - p3.Y, p2.X - p3.X, p2.Y - p3.Y);
            int v3 = VectorMul(p2.X - p1.X, p2.Y - p1.Y, p3.X - p1.X, p3.Y - p1.Y);
            int v4 = VectorMul(p2.X - p1.X, p2.Y - p1.Y, p4.X - p1.X, p4.Y - p1.Y);
            return (v1 * v2) < 0 && (v3 * v4) < 0;
        }

        public static bool AreCrossing(PointF p1, PointF p2, PointF p3, PointF p4)//проверка пересечения
        {
            var v1 = VectorMul(p4.X - p3.X, p4.Y - p3.Y, p1.X - p3.X, p1.Y - p3.Y);
            var v2 = VectorMul(p4.X - p3.X, p4.Y - p3.Y, p2.X - p3.X, p2.Y - p3.Y);
            var v3 = VectorMul(p2.X - p1.X, p2.Y - p1.Y, p3.X - p1.X, p3.Y - p1.Y);
            var v4 = VectorMul(p2.X - p1.X, p2.Y - p1.Y, p4.X - p1.X, p4.Y - p1.Y);
            return (v1 * v2) < 0 && (v3 * v4) < 0;
        }

        //public static bool Intersect(Point p1, Point p2, Point p3, Point p4)
        //{
        //    if (p2.X < p1.X)
        //    {

        //        Point tmp = p1;
        //        p1 = p2;
        //        p2 = tmp;
        //    }

        //    if (p4.X < p3.X)
        //    {

        //        Point tmp = p3;
        //        p3 = p4;
        //        p4 = tmp;
        //    }

        //    if (p2.X < p3.X)
        //    {
        //        return false;
        //    }

        //    //если оба отрезка вертикальные
        //    if ((p1.X == p2.X) && (p3.X == p4.X))
        //    {

        //        //если они лежат на одном X
        //        if (p1.X == p3.X)
        //        {

        //            //проверим пересекаются ли они, т.е. есть ли у них общий Y
        //            //для этого возьмём отрицание от случая, когда они НЕ пересекаются
        //            if (!((Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y)) ||
        //                    (Math.Min(p1.Y, p2.Y) > Math.Max(p3.Y, p4.Y))))
        //            {

        //                return true;
        //            }
        //        }

        //        return false;
        //    }

        //    //если первый отрезок вертикальный
        //    if (p1.X == p2.X )
        //    {

        //        //найдём Xa, Ya - точки пересечения двух прямых
        //        double Xa = p1.X;
        //        double A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
        //        double b2 = p3.Y - A2 * p3.X;
        //        double Ya = A2 * Xa + b2;

        //        if (p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya &&
        //                Math.Max(p1.Y, p2.Y) >= Ya)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //    //если второй отрезок вертикальный
        //    if (p3.X == p4.X)
        //    {

        //        //найдём Xa, Ya - точки пересечения двух прямых
        //        double Xa = p3.X;
        //        double A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
        //        double b1 = p1.Y - A1 * p1.X;
        //        double Ya = A1 * Xa + b1;

        //        if (p1.X <= Xa && p2.X >= Xa && Math.Min(p3.Y, p4.Y) <= Ya &&
        //                Math.Max(p3.Y, p4.Y) >= Ya)
        //        {
        //            return true;
        //        }

        //        return false;
        //    }


        //    //оба отрезка невертикальные
        //    double A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
        //    double A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
        //    double b1 = p1.Y - A1 * p1.X;
        //    double b2 = p3.Y - A2 * p3.X;

        //    if (A1 == A2)
        //    {
        //        return false; //отрезки параллельны
        //    }

        //    //Xa - абсцисса точки пересечения двух прямых
        //    double Xa = (b2 - b1) / (A1 - A2);

        //    if ((Xa < Math.Max(p1.X, p3.X)) || (Xa > Math.Min(p2.X, p4.X)))
        //    {
        //        return false; //точка Xa находится вне пересечения проекций отрезков на ось X 
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //    //var k1 = line1Start.Angle(line1End);
        //    //var k2 = line2Start.Angle(line2End);
        //    //var b1 = line1Start.Y - k1 * line1Start.X;
        //    //var b2 = line2Start.Y - k2 * line2Start.X;
        //    //var x = (b2 - b1) / (k1 - k2);
        //    //var y = k1
        //}

        public static double Angle(this Point point, Point otherPoint)
        {

            var y1 = point.Y;
            var y2 = otherPoint.Y;
            if (y1 == y2) return 0;
            else
            {
                var x1 = point.X;
                var x2 = otherPoint.X;
                return (y2 - y1) / (double)(x2 - x1);
            }
        }

        public static Rectangle GetRectangleFromTwoPoint(this Point point, Point otherPoint)
        {
            var x = Math.Min(point.X, otherPoint.X);
            var y = Math.Min(point.Y, otherPoint.Y);

            var w = Math.Abs(point.X - otherPoint.X);
            var h = Math.Abs(point.Y - otherPoint.Y);

            return new Rectangle(x, y, w, h);
        }

        public static RectangleF GetRectangleFromTwoPoint(this PointF point, PointF otherPoint)
        {
            var x = Math.Min(point.X, otherPoint.X);
            var y = Math.Min(point.Y, otherPoint.Y);

            var w = Math.Abs(point.X - otherPoint.X);
            var h = Math.Abs(point.Y - otherPoint.Y);

            return new RectangleF(x, y, w, h);
        }

        public static double Distance(this Point point, Point otherPoint) =>
           Math.Sqrt(Math.Pow(point.X - otherPoint.X, 2) + Math.Pow(point.Y - otherPoint.Y, 2));

        public static double Distance(this PointF point, PointF otherPoint) =>
           Math.Sqrt(Math.Pow(point.X - otherPoint.X, 2) + Math.Pow(point.Y - otherPoint.Y, 2));

        public static Rectangle GetRadiusRectangle(this Point point, int radius)
        {
            int dr = radius + radius;
            return new Rectangle(point.X - radius, point.Y - radius, dr, dr);
        }

        public static RectangleF GetRadiusRectangle(this PointF point, float radius)
        {
            float dr = radius + radius;
            return new RectangleF(point.X - radius, point.Y - radius, dr, dr);
        }

        public static Rectangle ToRectangle(this RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }
    }
}
