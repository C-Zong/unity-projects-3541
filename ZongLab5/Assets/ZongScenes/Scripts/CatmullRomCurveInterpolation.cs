using UnityEngine;

public class CatmullRomCurveInterpolation : CurveInterpolation
{
	const float Tau = 0.5f;
	public CatmullRomCurveInterpolation()
	{
		CurveManager.coefficientMatrix = new Matrix4x4(
										new Vector4(-Tau, 2 * Tau, -Tau, 0),
										new Vector4(2 - Tau, Tau - 3, 0, 1),
										new Vector4(Tau - 2, 3 - 2 * Tau, Tau, 0),
										new Vector4(Tau, -Tau, 0, 0)
								 );

	}

	// Returns the four control points needed for Catmull-Rom interpolation
	public Vector3[] GetGeometricInformation(Vector3[] controlPoints, int segmentNumber, int numberOfPoints)
	{
		Vector3 p0 = controlPoints[(segmentNumber - 1 + numberOfPoints) % numberOfPoints];
		Vector3 p1 = controlPoints[segmentNumber % numberOfPoints];
		Vector3 p2 = controlPoints[(segmentNumber + 1) % numberOfPoints];
		Vector3 p3 = controlPoints[(segmentNumber + 2) % numberOfPoints];

		return new Vector3[] { p0, p1, p2, p3 };
	}
}
