using UnityEngine;
using System.Collections.Generic;

public class HelpFunctions : MonoBehaviour
{
}

public static class Bezier
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);

        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p123 = Vector3.Lerp(p12, p23, t);

        Vector3 p0123 = Vector3.Lerp(p012, p123, t);

        return p0123;
    }

    public static Vector3 GetPoint(float t, params Vector3[] points)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        if(points.Length == 4) {
            return
            oneMinusT * oneMinusT * oneMinusT * points[0] +
            3f * oneMinusT * oneMinusT * t * points[1] +
            3f * oneMinusT * t * t * points[2] +
            t * t * t * points[3];
        }
        else {
            if (points.Length == 0) return new Vector3();

            Vector3[] processPoints = new Vector3[points.Length];
            for (int i = 0; i < processPoints.Length; i++) processPoints[i] = points[i];

            Vector3[] pointsTemp;

            while (processPoints.Length > 1) {
                pointsTemp = new Vector3[processPoints.Length - 1];
                for (int j = 0; j < processPoints.Length - 1; j++) {
                    Vector3 p = Vector3.Lerp(processPoints[j], processPoints[j + 1], t);
                    pointsTemp[j] = p;
                }
                processPoints = new Vector3[pointsTemp.Length];
                for (int i = 0; i < processPoints.Length; i++) processPoints[i] = pointsTemp[i];
            }

            Vector3 resPoint = processPoints[0];
            return resPoint;
        }
    }

    public static float GetParameterT(Vector3 pos, params Vector3[] points3D)
    {
        float t = 0;
        int n = points3D.Length;
        if (n != 4) return t;

        Vector2 pos2D = new Vector2();
        pos2D.x = pos.x;
        pos2D.y = pos.z;
        Vector2[] points = new Vector2[n];
        for (int i = 0; i < n; i++) {
            points[i].x = points3D[i].x;
            points[i].y = points3D[i].z;
        }

        float oneMinusT = 0;
        Vector2 tempPos = new Vector2();
        float dist = 100;

        while (dist > 2f && t < 1f) {
            oneMinusT = 1f - t;
            tempPos = oneMinusT * oneMinusT * oneMinusT * points[0] + 3f * oneMinusT * oneMinusT * t * points[1] + 3f * oneMinusT * t * t * points[2] + t * t * t * points[3];
            dist = Vector3.Distance(pos2D, tempPos);
            t += 0.001f;
        }

        t = t >= 1f ? 0f : t;
        return t;
    }

    public static Vector3 GetPoint3(float t, int count, params Vector3[] points)
    {
        //t = Mathf.Clamp01(t);
        int s = points.Length;

        //if(count == 2) 
        //    return Mathf.Pow(1f-t, s-1) * points[0] + Mathf.Pow(t, s-1) * points[s-1];

        //count--;

        //return 
        //    GetPoint3(t, count, points) + (s-1) * (1f-t) * t * points[count-1];
        if(count == s) t = Mathf.Clamp01(t);


        if (count == 1) {
            return  Mathf.Pow(1f-t, s-1) * points[0];
        }
        else if(count == s) {
            return GetPoint3(t, count-1, points) + Mathf.Pow(t, s-1) * points[s-1];
        }

        //count--;

        return
            GetPoint3(t, count-1, points) + (s-1) * Mathf.Pow(t, count-1) * Mathf.Pow(1f-t, s-count) * points[count-1];
    }

    public static Vector3 GetRotation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return 
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }

    public static Vector3 GetRotation(float t, params Vector3[] points)
    {
        if (points.Length != 4) return new Vector3();

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return
            3f * oneMinusT * oneMinusT * (points[1] - points[0]) +
            6f * oneMinusT * t * (points[2] - points[1]) +
            3f * t * t * (points[3] - points[2]);
    }
}

public static class RotationCounter
{
    public static float PI = 3.1416f;

    //static class Parameters
    //{
    //    public float x1, x2, y1, y2, ang;
    //    public float x3, y3;
    //    public float angFi;
    //    public float b, k;
    //    public float b2, k2;
    //};

    public static float getNewDirection(Vector3 carPos, Vector3 markerPos, float ang)
    {
        Vector2 point1 = new Vector2(carPos.x, carPos.z);   //ќпредел€ем первую точку на координатной оси
        Vector2 point2 = new Vector2(markerPos.x, markerPos.z);  //ќпредел€ем вторую точку на координатной оси
        //ѕолучаем два вектора на координатной оси: 1 - по точкам point1 и point2, 2 - по точке point1 и углу ang к оси y.
        //Ќужно найти результирующую точку пересечени€ вектора 2 и перпендикул€ра опущенного из точки point2 на эту пр€мую.
        Vector2 resPoint = countLines(point1, point2, ang);   //Ќаходим нужную точку и получаем пр€моугольный треугольник point1, point2, resPoint
        Vector3 result = getPathAtributes(point1, point2, resPoint);   //ѕо формулам пр€моугольного треугольника находим необходимые
                                                                       //направление от точке point3 к point2,
                                                                       //рассто€ние от точки point2 до point3 (по одному катету) и
                                                                       //рассто€ние от point1 до point2 (по гипотенузе)

        float dir = result.x;
        float distance = result.z;
        float shift = result.y;
        float angRes = Mathf.Asin(shift / distance)  * 180 / PI;
        angRes = dir > 0 ? -angRes : angRes;                       //результирующий угол с направлением по знаку

        return angRes;
    }

    static Vector3 getPathAtributes(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        float l1, l2;
        float x1 = point1.x;
        float z1 = point1.y;
        float x2 = point2.x;
        float z2 = point2.y;
        float x3 = point3.x;
        float z3 = point3.y;

        if (x3 > x1 && z3 > z1) l1 = Mathf.Sqrt((x3 - x1) * (x3 - x1) + (z3 - z1) * (z3 - z1));
        else if (x3 > x1 && z3 < z1) l1 = Mathf.Sqrt((x3 - x1) * (x3 - x1) + (z1 - z3) * (z1 - z3));
        else if (x3 < x1 && z3 > z1) l1 = Mathf.Sqrt((x1 - x3) * (x1 - x3) + (z3 - z1) * (z3 - z1));
        else l1 = Mathf.Sqrt((x1 - x3) * (x1 - x3) + (z1 - z3) * (z1 - z3));

        if (x3 > x2 && z3 > z2) l2 = Mathf.Sqrt((x3 - x2) * (x3 - x2) + (z3 - z2) * (z3 - z2));
        else if (x3 > x2 && z3 < z2) l2 = Mathf.Sqrt((x3 - x2) * (x3 - x2) + (z2 - z3) * (z2 - z3));
        else if (x3 < x2 && z3 > z2) l2 = Mathf.Sqrt((x2 - x3) * (x2 - x3) + (z3 - z2) * (z3 - z2));
        else l2 = Mathf.Sqrt((x2 - x3) * (x2 - x3) + (z2 - z3) * (z2 - z3));

        float l3 = Mathf.Sqrt(l1 * l1 + l2 * l2);

        float dir = (x3 - x1) * (z2 - z1) - (z3 - z1) * (x2 - x1);

        Vector3 result = new Vector3(dir, l2, l3);
        return result;
    }

    static Vector2 countLines(Vector2 point1, Vector2 point2, float angle)
    {
        float x1 = point1.x, z1 = point1.y, x2 = point2.x, z2 = point2.y;
        float angFi = 90 - angle;
        float angFiRad = angFi * PI / 180;  //Ќайден угол к оси X в радианах
        //ƒалее определ€ем переменные k, k2, b, b2 в формулах дл€ пр€мых на координатной оси:
        //y = k*x + b, y2 = k2*x2 + b2
        float k = Mathf.Tan(angFiRad);
        float b = z1 - k * x1;
        //k2 дл€ пр€мой перпендукул€рной к пр€мой с k равен: k2 = -1/k
        float k2 = -1 / k;
        float b2 = x2 / k + z2;
        //Ќаходим точку пересечени€ пр€мых y = k*x + b, y2 = k2*x2 + b2
        float y3 = (b - k * b2 / k2) / (1 - k / k2);
        float x3 = (y3 - b2) / k2;

        return new Vector2(x3, y3);
    }

    public static Vector3 getShiftMarker(Vector3 marker, float ang, float s, float d)
    {
        //Ќаходим точку смещенную от точки marker по перпендикул€ру к вектору опрелеленному аргументом ang (угол к оси y)
        //в сторону определенную аргументом d (по знаку) и на росто€нии s от нее 
        float x1 = marker.x;
        float z1 = marker.z;
        float angRad = ang * PI / 180;

        Vector3 newMarker = new Vector3();
        newMarker.x = x1 + s * d * Mathf.Cos(angRad);
        newMarker.y = marker.y;
        newMarker.z = z1 - s * d * Mathf.Sin(angRad);

        //string strDir = d > 0 ? "right" : "left";
        //Debug.Log("Counting new point for marker: " + marker + ". Ang: " + ang + ". Dist: " + s + ". Dir: " + strDir + ". Sign dir: " + d + ". New point: " + newMarker);

        return newMarker;
    }
}
