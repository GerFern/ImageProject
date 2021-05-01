using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace ModelBase
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Line
    {
        public class LineIDEqualityComparer : EqualityComparer<Line>
        {
            public static LineIDEqualityComparer Instance { get; } = new LineIDEqualityComparer();

            public override bool Equals(Line x, Line y)
            {
                return x.ID.Equals(y.ID);
            }

            public override int GetHashCode(Line obj)
            {
                return obj.ID;
            }
        }

        [Browsable(false)]
        public LineContainer Container { get; }

        public int ID { get; internal set; }

        public Line(Dot first, Dot second, LineSet owner, LineContainer container)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            first.Lines.Add(this);
            second.Lines.Add(this);
            container.Add(this);
            RectangleF = first.Point.GetRectangleFromTwoPoint(second.Point);
            first.PointChanged += DotPointChanged;
            second.PointChanged += DotPointChanged;
            float firstY = first.Point.Y;
            float secondY = second.Point.Y;
            Direction = firstY < secondY ? Direction.Up
                        : firstY > secondY ? Direction.Down
                        : Direction.None;
        }

        ~Line()
        {
            First.PointChanged -= DotPointChanged;
            Second.PointChanged -= DotPointChanged;
        }

        private void DotPointChanged(object sender, EventArgs e)
        {
            RectangleF = First.Point.GetRectangleFromTwoPoint(Second.Point);
        }

        public bool IsIntersect(Line otherLine)
        {
            return PointExtensions.AreCrossing(First.Point, Second.Point, otherLine.First.Point, otherLine.Second.Point);
        }

        [DisplayName("Пересечения")]
        public List<Line> Intersected { get; }

        [DisplayName("Начало")]
        public Dot First { get; }

        [DisplayName("Конец")]
        public Dot Second { get; }

        [Browsable(false)]
        public LineSet Owner { get; }

        [Browsable(false)]
        public RectangleF RectangleF { get; private set; }

        [DisplayName("Направление")]
        public Direction Direction { get; }

        public override string ToString() =>
            $"{First.Point} -> {Second.Point} : {First.Point.Distance(Second.Point)}";
    }

    public struct LineVector
    {
        public float X { get; }
        public float Y { get; }
        public float Length { get; }

        public LineVector(float x, float y)
        {
            X = x;
            Y = y;
            Length = (float)Math.Sqrt(x * x + y * y);
        }

        public LineVector(PointF point) : this(point.X, point.Y)
        {
        }

        private LineVector(float x, float y, float length)
        {
            X = x;
            Y = y;
            Length = length;
        }

        public LineVector GetReverse()
        {
            return new LineVector(-X, -Y, Length);
        }

        public static LineVector FindVector(PointF first, PointF second)
        {
            return new LineVector(second.X - first.X, second.Y - first.Y);
        }

        public float FindAngle(LineVector otherVecror)
        {
            var s1 = this * otherVecror;
            var s2 = Length * otherVecror.Length;
            return (float)Math.Acos(s1 / s2);
        }

        public static LineVector operator +(LineVector left, LineVector right)
        {
            return new LineVector(left.X + right.X, left.Y + right.Y);
        }

        public static LineVector operator -(LineVector left, LineVector right)
        {
            return new LineVector(left.X - right.X, left.Y - right.Y);
        }

        public static LineVector operator -(LineVector lineVector)
        {
            return lineVector.GetReverse();
        }

        public static float operator *(LineVector left, LineVector right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
    }
}