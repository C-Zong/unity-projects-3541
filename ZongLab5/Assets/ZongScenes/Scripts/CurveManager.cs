using UnityEngine;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour
{
    public enum CurveType
    {
        [InspectorName("Catmull–Rom Splines")]
        CatmullRom,

        [InspectorName("Bezier Curves")]
        Bezier
    }

    [Header("Curve Settings")]
    public CurveType curveType = CurveType.CatmullRom;
    public int numberOfPoints = 10;
    public float deltaTime = 0.001f;
    [Range(0f, 0.49f)]
    public float easingDuration = 0.125f;

    // Control point generation bounds
    Vector3[] controlPoints;
    const int MinX = -5;
    const int MinY = -5;
    const int MinZ = 0;
    const int MaxX = 5;
    const int MaxY = 5;
    const int MaxZ = 5;

    const int SamplesPerSegment = 100;
    List<float[]> segmentArcLengths = new List<float[]>();
    List<float> segmentTotalLengths = new List<float>();
    float time = 0;
    float a = 0;
    CurveInterpolation curveInterpolation;

    public static Matrix4x4 coefficientMatrix;

    // Generate random control points and create visual representations
    void GenerateControlPointGeometry()
    {
        if (curveType == CurveType.Bezier)
            numberOfPoints = numberOfPoints + (3 - numberOfPoints % 3) % 3;

        controlPoints = new Vector3[numberOfPoints];

        controlPoints[0] = new Vector3(0, 0, 0);
        for (int i = 1; i < numberOfPoints; i++)
        {
            controlPoints[i] = new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY), Random.Range(MinZ, MaxZ));
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tempcube.transform.localScale -= new Vector3(0.8f, 0.8f, 0.8f);
            tempcube.transform.position = controlPoints[i];
        }
    }

    // Precompute arc lengths for each segment using supersampling
    void PrecomputeArcLengths()
    {
        float totalCurveLength = 0f;

        segmentArcLengths.Clear();
        segmentTotalLengths.Clear();

        if (curveType == CurveType.Bezier)
            numberOfPoints = numberOfPoints / 3;
        for (int s = 0; s < numberOfPoints; s++)
        {
            float[] arcLength = new float[SamplesPerSegment + 1];
            arcLength[0] = 0f;
            Vector3 prev = ComputePointOnCurve(0f, s);

            for (int i = 1; i <= SamplesPerSegment; i++)
            {
                float u = i / (float)SamplesPerSegment;
                Vector3 curr = ComputePointOnCurve(u, s);
                arcLength[i] = arcLength[i - 1] + Vector3.Distance(prev, curr);
                prev = curr;
            }

            totalCurveLength += arcLength[SamplesPerSegment];
            segmentArcLengths.Add(arcLength);
            segmentTotalLengths.Add(arcLength[SamplesPerSegment]);
        }

        float cumulative = 0f;
        for (int i = 0; i < segmentTotalLengths.Count; i++)
        {
            float normalizedSegmentLength = segmentTotalLengths[i] / totalCurveLength;
            cumulative += normalizedSegmentLength;
            segmentTotalLengths[i] = cumulative;
        }
    }

    // Compute a point on the curve given parameter u and segment number
    public Vector3 ComputePointOnCurve(float u, int segmentNumber)
    {
        Vector3[] B = curveInterpolation.GetGeometricInformation(controlPoints, segmentNumber, numberOfPoints);

        Vector4 U = new Vector4(u * u * u, u * u, u, 1);

        Vector4 Px = new Vector4(B[0].x, B[1].x, B[2].x, B[3].x);
        Vector4 Py = new Vector4(B[0].y, B[1].y, B[2].y, B[3].y);
        Vector4 Pz = new Vector4(B[0].z, B[1].z, B[2].z, B[3].z);

        float x = Vector4.Dot(U, coefficientMatrix * Px);
        float y = Vector4.Dot(U, coefficientMatrix * Py);
        float z = Vector4.Dot(U, coefficientMatrix * Pz);

        return new Vector3(x, y, z);
    }

    // Easing function to compute eased s value
    float ComputeEasedS(float t)
    {
        if (t < easingDuration)
        {
            return 0.5f * a * t * t;
        }
        else if (t < 1 - easingDuration)
        {
            return a * easingDuration * (t - 0.5f * easingDuration);
        }
        else
        {
            return 1 - 0.5f * a * (1 - t) * (1 - t);
        }
    }

    // Find segment number for a given target arc length
    int FindSegmentNumber(float target)
    {
        for (int i = 0; i < segmentTotalLengths.Count; i++)
        {
            if (target < segmentTotalLengths[i])
                return i;
        }
        return segmentTotalLengths.Count - 1;
    }

    // Find target length within a segment
    float FindTargetLength(float s, int segmentNumber)
    {
        float start = (segmentNumber == 0) ? 0f : segmentTotalLengths[segmentNumber - 1];
        float end = segmentTotalLengths[segmentNumber];
        float localS = (s - start) / (end - start);
        float segmentLength = segmentArcLengths[segmentNumber][SamplesPerSegment];
        return localS * segmentLength;
    }

    // Find parameter u corresponding to a target arc length using binary search
    float FindUFromArcLength(float targetLength, float[] arcLength)
    {
        int low = 0, high = arcLength.Length - 1;
        while (low < high)
        {
            int mid = (low + high) / 2;
            if (arcLength[mid] < targetLength)
                low = mid + 1;
            else
                high = mid;
        }

        int i = Mathf.Max(low - 1, 0);
        float t = (targetLength - arcLength[i]) / (arcLength[i + 1] - arcLength[i]);
        return (i + t) / (arcLength.Length - 1);
    }

    void Start()
    {
        GenerateControlPointGeometry();

        switch (curveType)
        {
            case CurveType.CatmullRom:
                curveInterpolation = new CatmullRomCurveInterpolation();
                break;
            case CurveType.Bezier:
                curveInterpolation = new BezierCurveInterpolation();
                break;
        }

        PrecomputeArcLengths();
        a = 1 / (easingDuration * (1 - easingDuration));
    }

    void Update()
    {
        time += deltaTime;
        if (time > 1)
            time -= 1;
        float easedS = ComputeEasedS(time);

        int segmentNumber = FindSegmentNumber(easedS);
        float u = FindUFromArcLength(FindTargetLength(easedS, segmentNumber), segmentArcLengths[segmentNumber]);
        Vector3 temp = ComputePointOnCurve(u, segmentNumber);

        Vector3 previousPos = transform.position;
        transform.position = temp;

        Vector3 direction = (transform.position - previousPos).normalized;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }
}
