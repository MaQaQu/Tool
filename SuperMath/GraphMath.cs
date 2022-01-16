using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMath
{
    public static class GraphMath
    {
        public const float PI = 3.1415926535897931f;

        /// <summary>
        /// 判断圆上一点是否在圆上的一段弧内
        /// </summary>
        /// <param name="startAngle">弧的起始角度（x轴正方向为0）</param>
        /// <param name="deltaAngle">弧的圆周角</param>
        /// <param name="Angle">圆上一点与圆心连线的角度（x轴正方向为0）</param>
        /// <returns>结果</returns>
        public static bool IsAngleInRange(double startAngle, double deltaAngle, double Angle)
        {
            startAngle = DegreeTo360(startAngle) + 720;
            Angle = DegreeTo360(Angle) + 720;

            if (deltaAngle > 0)
            {
                if (Angle < startAngle) Angle += 360;
                if (startAngle + deltaAngle > Angle && startAngle < Angle)
                {
                    return true;
                }
            }
            else
            {
                if (Angle > startAngle) Angle -= 360;
                if (startAngle + deltaAngle < Angle && startAngle > Angle)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断两个点是否接近
        /// </summary>
        /// <param name="X1">点1X</param>
        /// <param name="Y1">点1Y</param>
        /// <param name="X2">点2X</param>
        /// <param name="Y2">点2Y</param>
        /// <param name="offset">偏差</param>
        /// <returns>结果</returns>
        public static bool IsPointApproach(double X1, double Y1, double X2, double Y2, double offset)
        {
            if (Math.Abs(X1 - X2) <= offset && Math.Abs(Y1 - Y2) <= offset)
            {
                if (DistancePointToPoint(X1, Y1, X2, Y2) <= offset)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断两个点是否接近
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <param name="offset">偏差</param>
        /// <returns>结果</returns>
        public static bool IsPointApproach(GraphPoint P1, GraphPoint P2, double offset)
        {
            return IsPointApproach(P1.X, P1.Y, P2.X, P2.Y, offset);
        }

        /// <summary>
        /// 两点间线段的长度
        /// </summary>
        /// <param name="X1">点1X</param>
        /// <param name="Y1">点1Y</param>
        /// <param name="X2">点2X</param>
        /// <param name="Y2">点2Y</param>
        /// <returns>线段的长度</returns>
        public static double DistancePointToPoint(double X1, double Y1, double X2, double Y2)
        {
            double length = Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));
            return length;
        }

        /// <summary>
        /// 两点间线段的长度的平方
        /// </summary>
        /// <param name="X1">点1X</param>
        /// <param name="Y1">点1Y</param>
        /// <param name="X2">点2X</param>
        /// <param name="Y2">点2Y</param>
        /// <returns>线段的长度的平方</returns>
        public static double DistancePointToPoint2(double X1, double Y1, double X2, double Y2)
        {
            double length = Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2);
            return length;
        }

        /// <summary>
        /// 两点间线段的长度
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <returns>线段的长度</returns>
        public static double DistancePointToPoint(GraphPoint P1, GraphPoint P2)
        {
            return DistancePointToPoint(P1.X, P1.Y, P2.X, P2.Y);
        }

        /// <summary>
        /// 两点间线段的长度的平方
        /// </summary>
        /// <param name="P1">点1</param>
        /// <param name="P2">点2</param>
        /// <returns>线段的长度的平方</returns>
        public static double DistancePointToPoint2(GraphPoint P1, GraphPoint P2)
        {
            return DistancePointToPoint2(P1.X, P1.Y, P2.X, P2.Y);
        }

        /// <summary>
        /// 计算线段ab和线段ac的夹角（小于π）
        /// </summary>
        /// <param name="Xa">点a的X坐标</param>
        /// <param name="Ya">点a的Y坐标</param>
        /// <param name="Xb">点b的X坐标</param>
        /// <param name="Yb">点b的Y坐标</param>
        /// <param name="Xc">点c的X坐标</param>
        /// <param name="Yc">点c的Y坐标</param>
        /// <returns>夹角（弧度）</returns>
        public static double IntersectionRadian(double Xa, double Ya, double Xb, double Yb, double Xc, double Yc)
        {
            double Lab = DistancePointToPoint(Xa, Ya, Xb, Yb);
            double Lac = DistancePointToPoint(Xa, Ya, Xc, Yc);
            double Lbc = DistancePointToPoint(Xb, Yb, Xc, Yc);

            double angle = Math.Acos((Lab * Lab + Lac * Lac - Lbc * Lbc) / (2 * Lac * Lab));
            return angle;
        }

        /// <summary>
        /// 计算线段ab和线段ac的夹角（小于π）
        /// </summary>
        /// <param name="Pa">点a</param>
        /// <param name="Pb">点b</param>
        /// <param name="Pc">点c</param>
        /// <returns>夹角（弧度）</returns>
        public static double IntersectionRadian(GraphPoint Pa, GraphPoint Pb, GraphPoint Pc)
        {
            return IntersectionRadian(Pa.X, Pa.Y, Pb.X, Pb.Y, Pc.X, Pc.Y);
        }

        /// <summary>
        /// 计算线段ab和线段ac的夹角（小于180）
        /// </summary>
        /// <param name="Xa">点a的X坐标</param>
        /// <param name="Ya">点a的Y坐标</param>
        /// <param name="Xb">点b的X坐标</param>
        /// <param name="Yb">点b的Y坐标</param>
        /// <param name="Xc">点c的X坐标</param>
        /// <param name="Yc">点c的Y坐标</param>
        /// <returns>夹角（角度）</returns>
        public static double IntersectionDegree(double Xa, double Ya, double Xb, double Yb, double Xc, double Yc)
        {
            double angle = IntersectionRadian(Xa, Ya, Xb, Yb, Xc, Yc);
            return DegreeOf(angle);
        }

        /// <summary>
        /// 计算线段ab和线段ac的夹角（小于180）
        /// </summary>
        /// <param name="Pa">点a</param>
        /// <param name="Pb">点b</param>
        /// <param name="Pc">点c</param>
        /// <returns>夹角（角度）</returns>
        public static double IntersectionDegree(GraphPoint Pa, GraphPoint Pb, GraphPoint Pc)
        {
            return IntersectionDegree(Pa.X, Pa.Y, Pb.X, Pb.Y, Pc.X, Pc.Y);
        }

        /// <summary>
        /// 点到由点1和点2确定的直线的距离
        /// </summary>
        /// <param name="X1">点1的X坐标</param>
        /// <param name="Y1">点1的Y坐标</param>
        /// <param name="X2">点2的X坐标</param>
        /// <param name="Y2">点2的Y坐标</param>
        /// <param name="X">点的X坐标</param>
        /// <param name="Y">点的Y坐标</param>
        /// <returns>距离</returns>
        public static double DistancePointToLine(double X1, double Y1, double X2, double Y2, double X, double Y)
        {
            double d12 = DistancePointToPoint(X1, Y1, X2, Y2);
            double d1p = DistancePointToPoint(X1, Y1, X, Y);
            double d2p = DistancePointToPoint(X, Y, X2, Y2);

            double a1 = Math.Acos((d12 * d12 + d1p * d1p - d2p * d2p) / (2 * d12 * d1p));
            double h = d1p * Math.Sin(a1);

            return h;
        }

        /// <summary>
        /// 点到由点1和点2确定的直线的距离
        /// </summary>
        /// <param name="P1">点2</param>
        /// <param name="P2">点1</param>
        /// <param name="P">点</param>
        /// <returns>距离</returns>
        public static double DistancePointToLine(GraphPoint P1, GraphPoint P2, GraphPoint P)
        {
            return DistancePointToLine(P1.X, P1.Y, P2.X, P2.Y, P.X, P.Y);
        }

        /// <summary>
        /// 点在由点1和点2确定的直线上的投影和点1的距离
        /// </summary>
        /// <param name="X1">点1的X坐标</param>
        /// <param name="Y1">点1的Y坐标</param>
        /// <param name="X2">点2的X坐标</param>
        /// <param name="Y2">点2的Y坐标</param>
        /// <param name="X">点的X坐标</param>
        /// <param name="Y">点的Y坐标</param>
        /// <returns>距离</returns>
        public static double DistancePointToOrigin(double X1, double Y1, double X2, double Y2, double X, double Y)
        {
            double d12 = DistancePointToPoint(X1, Y1, X2, Y2);
            double d1p = DistancePointToPoint(X1, Y1, X, Y);
            double d2p = DistancePointToPoint(X, Y, X2, Y2);

            double h = (d12 * d12 + d1p * d1p - d2p * d2p) / (2 * d12);

            return h;
        }

        /// <summary>
        /// 点在点1指定方向的直线上的投影和点1的距离
        /// </summary>
        /// <param name="X1">点1的X坐标</param>
        /// <param name="Y1">点1的Y坐标</param>
        /// <param name="rad">指定方向</param>
        /// <param name="X">点的X坐标</param>
        /// <param name="Y">点的Y坐标</param>
        /// <returns>距离</returns>
        public static double DistancePointToOrigin(double X1, double Y1, double rad, double X, double Y)
        {
            double slope = SlopeRadian(X1, Y1, X, Y);
            double inAng = IntersectionRadian(rad, slope);
            double dis = DistancePointToPoint(X1, Y1, X, Y) * Math.Cos(inAng);

            return dis;
        }

        /// <summary>
        /// 点在点1指定方向的直线上的投影点的距离
        /// </summary>
        /// <param name="X1">点1的X坐标</param>
        /// <param name="Y1">点1的Y坐标</param>
        /// <param name="rad">指定方向</param>
        /// <param name="X">点的X坐标</param>
        /// <param name="Y">点的Y坐标</param>
        /// <returns>距离</returns>
        public static double DistancePointToLine(double X1, double Y1, double rad, double X, double Y)
        {
            double slope = SlopeRadian(X1, Y1, X, Y);
            double inAng = DifferentRadian(rad, slope);
            double dis = DistancePointToPoint(X1, Y1, X, Y) * Math.Sin(inAng);

            return dis;
        }

        /// <summary>
        /// 点在由点1和点2确定的直线上的投影和点1的距离
        /// </summary>
        /// <param name="P1">点2</param>
        /// <param name="P2">点1</param>
        /// <param name="P">点</param>
        /// <returns>距离</returns>
        public static double DistancePointToOrigin(GraphPoint P1, GraphPoint P2, GraphPoint P)
        {
            return DistancePointToOrigin(P1.X, P1.Y, P2.X, P2.Y, P.X, P.Y);
        }

        /// <summary>
        /// 计算点在由点1和点2确定的射线的左边还是右边（若射线向y轴负方向，-1：在射线左边；0：在射线上；1：在射线右边）
        /// </summary>
        /// <param name="X1">点1的X坐标</param>
        /// <param name="Y1">点1的Y坐标</param>
        /// <param name="X2">点2的X坐标</param>
        /// <param name="Y2">点2的Y坐标</param>
        /// <param name="X">点的X坐标</param>
        /// <param name="Y">点的Y坐标</param>
        /// <returns>结果</returns>
        public static int IsPointOnLine(double X1, double Y1, double X2, double Y2, double X, double Y)
        {
            double angle = SlopeRadian(X1, Y1, X2, Y2);
            GraphPoint UpCenter = GetEndPoint(new GraphPoint(X1, Y1), RadianTo2PI(angle + Math.PI / 2), 100);

            double distance = DistancePointToOrigin(X1, Y1, UpCenter.X, UpCenter.Y, X, Y);

            if (distance > 0)
                return 1;
            else if (distance == 0)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// 计算点在由点1和点2确定的射线的左边还是右边（-1：在射线左边；0：在射线上；1：在射线右边）
        /// </summary>
        /// <param name="P1">点2</param>
        /// <param name="P2">点1</param>
        /// <param name="P">点</param>
        /// <returns>结果</returns>
        public static int IsPointOnLine(GraphPoint P1, GraphPoint P2, GraphPoint P)
        {
            return IsPointOnLine(P1.X, P1.Y, P2.X, P2.Y, P.X, P.Y);
        }

        /// <summary>
        /// 计算两个角度的夹角（小于180度）
        /// </summary>
        /// <param name="degree1">角度1</param>
        /// <param name="degree2">角度2</param>
        /// <returns>结果</returns>
        public static double IntersectionDegree(double degree1, double degree2)
        {
            double dela = degree1 - degree2;
            return Math.Abs(DegreeTo180(dela));
        }

        /// <summary>
        /// 计算从角度1逆时针旋转到角度2的度数
        /// </summary>
        /// <param name="degree1">角度1</param>
        /// <param name="degree2">角度2</param>
        /// <returns>结果</returns>
        public static double DifferentDegree(double degree1, double degree2)
        {
            double delta = DegreeTo360(degree2) - DegreeTo360(degree1);
            if (delta < 0)
            {
                delta += 360;
            }
            return delta;
        }

        /// <summary>
        /// 计算两个弧度的夹角（小于π）
        /// </summary>
        /// <param name="radian1">弧度1</param>
        /// <param name="radian2">弧度2</param>
        /// <returns>结果</returns>
        public static double IntersectionRadian(double radian1, double radian2)
        {
            double dela = radian1 - radian2;
            return Math.Abs(RadianToPI(dela));
        }

        /// <summary>
        /// 计算从弧度1逆时针旋转到弧度2的度数
        /// </summary>
        /// <param name="radian1">弧度1</param>
        /// <param name="radian2">弧度2</param>
        /// <returns></returns>
        public static double DifferentRadian(double radian1, double radian2)
        {
            double delta = RadianTo2PI(radian2) - RadianTo2PI(radian1);
            if(delta < 0)
            {
                delta += 2 * Math.PI;
            }
            return delta;
        }

        /// <summary>
        /// 将任意角度转为弧度
        /// </summary>
        /// <param name="degree">角度</param>
        /// <returns>弧度</returns>
        public static double RadianOf(double degree)
        {
            double radians = (Math.PI / 180) * degree;
            return radians;
        }

        /// <summary>
        /// 将任意角度转为弧度
        /// </summary>
        /// <param name="degree">角度</param>
        /// <returns>弧度</returns>
        public static float RadianOfF(float degree)
        {
            float radians = (PI / 180f) * degree;
            return radians;
        }

        /// <summary>
        /// 将任意弧度转为角度
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>角度</returns>
        public static double DegreeOf(double radian)
        {
            double degree = (radian / Math.PI) * 180;
            return degree;
        }

        /// <summary>
        /// 将任意弧度转为角度
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>角度</returns>
        public static float DegreeOfF(float radian)
        {
            float degree = (radian / PI) * 180f;
            return degree;
        }

        /// <summary>
        /// 把任意角度转换为-180~+180度之间的角度
        /// </summary>
        /// <param name="degree">任意角度</param>
        /// <returns>角度</returns>
        public static double DegreeTo180(double degree)
        {
            double mydegree = 0;
            double degree360 = DegreeTo360(degree);
            if (degree360 <= 180)
            {
                mydegree = degree360;
            }
            else
            {
                mydegree = (-360) + degree360;
            }
            return mydegree;
        }

        /// <summary>
        /// 把任意角度转换为0~360度之间的角度
        /// </summary>
        /// <param name="degree">任意角度</param>
        /// <returns>角度</returns>
        public static double DegreeTo360(double degree)
        {
            int n;
            if (degree >= 0)
            {
                n = (int)(degree / 360);
            }
            else
            {
                n = (int)(degree / 360) - 1;
            }
            return degree - 360 * n;
        }

        /// <summary>
        /// 把任意角度转换为0~360度之间的角度（带符号）
        /// </summary>
        /// <param name="degree">任意角度</param>
        /// <returns>角度</returns>
        public static double DegreeToSigned360(double degree)
        {
            double output = DegreeTo360(Math.Abs(degree));
            if (degree > 0)
            {
                return output;
            }
            else
            {
                return -output;
            }
        }

        /// <summary>
        /// 把任意弧度转换为(+ -)π之间的弧度
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>弧度</returns>
        public static double RadianToPI(double radian)
        {
            double myradian = 0;
            double radian2PI = RadianTo2PI(radian);
            if (radian2PI <= Math.PI)
            {
                myradian = radian2PI;
            }
            else
            {
                myradian = (-(2 * Math.PI)) + radian2PI;
            }
            return myradian;
        }

        /// <summary>
        /// 计算返回指定弧度与x轴正方向的弧度（0 ~ 2*PI）
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>弧度</returns>
        public static double RadianTo2PI(double radian)
        {
            int n;

            if(radian >= 0 && radian < 2 * Math.PI)
            {
                return radian;
            }

            if (radian >= 0)
            {
                n = (int)(radian / (2 * Math.PI));
            }
            else
            {
                n = (int)(radian / (2 * Math.PI)) - 1;
            }
            return radian - 2 * Math.PI * n;
        }

        /// <summary>
        /// 计算返回指定弧度与x轴正方向的弧度（带符号的 0 ~ 2*PI）
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>弧度</returns>
        public static double RadianToSigned2PI(double radian)
        {
            double output = RadianTo2PI(Math.Abs(radian));
            if (radian > 0)
            {
                return output;
            }
            else
            {
                return -output;
            }
        }

        /// <summary>
        /// 返回两点的连线与x轴正方向的夹角（弧度）
        /// </summary>
        /// <param name="aX">点a X坐标</param>
        /// <param name="aY">点a Y坐标</param>
        /// <param name="bX">点b X坐标</param>
        /// <param name="bY">点b Y坐标</param>
        /// <returns>夹角（弧度）</returns>
        public static double SlopeRadian(double aX, double aY, double bX, double bY)
        {
            double output;
            output = RadianTo2PI(Math.Atan2((bY - aY), (bX - aX)));
            return output;
        }

        /// <summary>
        /// 返回两点的连线与x轴正方向的夹角（角度）
        /// </summary>
        /// <param name="aX">点a X坐标</param>
        /// <param name="aY">点a Y坐标</param>
        /// <param name="bX">点b X坐标</param>
        /// <param name="bY">点b Y坐标</param>
        /// <returns>夹角（角度）</returns>
        public static double SlopeDegree(double aX, double aY, double bX, double bY)
        {
            return DegreeOf(SlopeRadian(aX, aY, bX, bY));
        }

        /// <summary>
        /// 计算并返回起始点指定方位长度上的点的坐标
        /// </summary>
        /// <param name="StartX">起始点X</param>
        /// <param name="StartY">起始点Y</param>
        /// <param name="Radian">方位角（x轴正方向为零度）（弧度）</param>
        /// <param name="length">距离</param>
        /// <returns>坐标点</returns>
        public static GraphPoint GetEndPoint(double StartX, double StartY, double Radian, double length)
        {
            return new GraphPoint(Math.Round(StartX + length * Math.Cos(Radian), 6), Math.Round(StartY + length * Math.Sin(Radian), 6));
        }

        /// <summary>
        /// 计算并返回起始点指定方位长度上的点的坐标
        /// </summary>
        /// <param name="Start">起始点</param>
        /// <param name="Radian">方位角（x轴正方向为零度）（弧度）</param>
        /// <param name="length">距离</param>
        /// <returns>坐标点</returns>
        public static GraphPoint GetEndPoint(GraphPoint Start, double Radian, double length)
        {
            return GetEndPoint(Start.X, Start.Y, Radian, length);
        }
    }
}
