using UnityEngine;

public class BezierCurveInterpolation : CurveInterpolation
{
    public BezierCurveInterpolation()
    {
        CurveManager.coefficientMatrix = new Matrix4x4(
                    new Vector4(-1, 3, -3, 1),
                    new Vector4(3, -6, 3, 0),
                    new Vector4(-3, 3, 0, 0),
                    new Vector4(1, 0, 0, 0)
                );
    }

    // Returns the four control points needed for Bezier interpolation
    public Vector3[] GetGeometricInformation(Vector3[] controlPoints, int segmentNumber, int numberOfPoints)
    {
        Vector3 p0 = controlPoints[segmentNumber * 3];
        Vector3 p1 = controlPoints[segmentNumber * 3 + 1];
        Vector3 p2 = controlPoints[segmentNumber * 3 + 2];
        Vector3 p3 = segmentNumber * 3 + 3 != controlPoints.Length ? controlPoints[(segmentNumber * 3 + 3)] : controlPoints[0];

        return new Vector3[] { p0, p1, p2, p3 };
    }
}
