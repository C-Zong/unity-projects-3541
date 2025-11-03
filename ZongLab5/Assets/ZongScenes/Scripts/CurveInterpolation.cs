using UnityEngine;

public interface CurveInterpolation
{
    public Vector3[] GetGeometricInformation(Vector3[] controlPoints, int segmentNumber, int numberOfPoints);
}
