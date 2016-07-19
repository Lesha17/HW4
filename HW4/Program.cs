using System;
using System.IO;

namespace HW4
{
    class Program
    {
        const uint Q = 26681;
        const long A = -2;
        const long B = 0;
        static readonly Point ZeroPoint = new Point(0, 0);
        static readonly Point P = new Point(26597, 13742);

        static void Main(string[] args)
        {
            uint i = 0;
            using (StreamWriter sw = new StreamWriter( File.Create("../../70.txt")))
            {
                Point current = ZeroPoint;
                do
                {
                    current = current + P;
                    sw.WriteLine("({0}, {1})", current.X.Value, current.Y.Value);    
                    ++i;
                } while (!current.IsZero);
                sw.WriteLine("Порядок P: {0}", i);
            }

            uint a = 228;
            var A_Point = a * P;
            Console.WriteLine("A = ({0} {1})", A_Point.X.Value, A_Point.Y.Value);

            var M = 666 * P;
            Console.WriteLine("M = ({0} {1})", M.X.Value, M.Y.Value);

            uint k = 1488;
            var B1_Point = k * P;
            var B2_Point = M + k * A_Point;

            Console.WriteLine("B1 = ({0} {1})", B1_Point.X.Value, B1_Point.Y.Value);
            Console.WriteLine("B2 = ({0} {1})", B2_Point.X.Value, B2_Point.Y.Value);
            Console.WriteLine("Cryptogram = (({0} {1}), ({2} {3}))", B1_Point.X.Value, B1_Point.Y.Value, B2_Point.X.Value, B2_Point.Y.Value);

            var decrypt_M = B2_Point + (i-a) * B1_Point;
            Console.WriteLine("Alice Received Point M: ({0} {1})", decrypt_M.X.Value, decrypt_M.Y.Value);
        }

        struct GroupElement
        {
            public long Value { get; set; }

            public static GroupElement operator +(GroupElement a, GroupElement b)
            {
                return a.Value + b.Value;
            }

            public static GroupElement operator -(GroupElement a)
            {
                return (Q - 1) * a;
            }

            public static GroupElement operator -(GroupElement a, GroupElement b)
            {
                return a + (-b);
            }

            public static GroupElement operator *(GroupElement a, GroupElement b)
            {
                return a.Value * b.Value;
            }

            public static bool operator ==(GroupElement a, GroupElement b)
            {
                return a.Value == b.Value;
            }

            public static bool operator !=(GroupElement a, GroupElement b)
            {
                return !(a == b);
            }

            public static implicit operator long (GroupElement g)
            {
                return g.Value;
            }

            public static implicit operator GroupElement(long n)
            {
                return new GroupElement { Value = (n % Q + Q) % Q };
            }
        }

        struct Point
        {
            public Point(GroupElement x, GroupElement y)
            {
                X = x;
                Y = y;
            }

            public GroupElement X { get; set; }
            public GroupElement Y { get; set; }

            public bool IsZero { get { return this == ZeroPoint; } }

            public static bool operator ==(Point p1, Point p2)
            {
                return p1.X == p2.X && p1.Y == p2.Y;
            }

            public static bool operator !=(Point p1, Point p2)
            {
                return !(p1 == p2);
            }

            public static Point operator +(Point p1, Point p2)
            {
                if (p1.IsZero)
                {
                    return p2;
                }

                if (p2.IsZero)
                {
                    return p1;
                }

                if (p1 == p2 && p1.Y != 0)
                {
                    long a, b;
                    long g = gcd(2 * p1.Y, Q, out a, out b);

                    GroupElement lambda = (3 * p1.X * p1.X + A) * a;
                    GroupElement x = lambda * lambda - 2 * p1.X;
                    GroupElement y = lambda * (p1.X - x) - p1.Y;

                    return new Point(x, y);
                }
                if (p1.X != p2.X)
                {
                    GroupElement dy = p2.Y - p1.Y;
                    GroupElement dx = p2.X - p1.X;

                    long a, b;
                    long g = gcd(dx, Q, out a, out b);

                    if (g != 1)
                    {
                        Console.WriteLine("Не существует!");
                    }

                    GroupElement lambda = dy * a;
                    GroupElement x = lambda * lambda - p1.X - p2.X;
                    GroupElement y = lambda * (p1.X - x) - p1.Y;

                    return new Point(x, y);
                }
                if (p1.X == p2.X && p2.Y == -(p1.Y))
                {
                    return new Point(0, 0);
                }
                return new Point(0, 0);
            }

            public static Point operator *(uint n, Point p)
            {
                var ans = p;
                for (uint i = 1; i < n; ++i)
                {
                    ans += p;
                }
                return ans;
            }

            static long gcd(long a, long b, out long x, out long y)
            {
                if (a == 0)
                {
                    x = 0; y = 1;
                    return b;
                }
                long x1, y1;
                long d = gcd(b % a, a, out x1, out y1);
                x = y1 - (b / a) * x1;
                y = x1;
                return d;
            }
        }
    }
}
