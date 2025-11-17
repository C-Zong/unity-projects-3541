using UnityEngine;
using System;

public class RayTracer : MonoBehaviour
{
    [Header("Plane Variables")]
    public float distanceToPlane = 10.0f;
    public float pixelWidth = 1;
    public float pixelHeight = 1;
    public int imageWidth = 11;
    public int imageHeight = 11;
    public float maxRayDistance = 100.0f;

    Color[,] p;

    void Start()
    {
        p = new Color[imageWidth, imageHeight];
        Transform cameraTrans = this.transform;
        Vector3 cameraPos = cameraTrans.position;
        for (int i = 0; i < imageWidth; i++)
        {
            for (int j = 0; j < imageHeight; j++)
            {
                float x = (i - (imageWidth - 1) / 2.0f) * pixelWidth;
                float y = (j - (imageHeight - 1) / 2.0f) * pixelHeight;
                Vector3 rayDirection = (cameraTrans.right * x + cameraTrans.up * y + cameraTrans.forward * distanceToPlane).normalized;
                RaycastHit hit;
                Physics.Raycast(cameraPos, rayDirection, out hit, maxRayDistance);
                if (hit.collider == null)
                {
                    p[i, j] = Camera.main.backgroundColor;
                }
                else
                {
                    p[i, j] = hit.collider.GetComponent<Renderer>().material.color;
                }
            }
        }
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string path = Application.dataPath + $"/ZongData/{imageHeight}x{imageWidth}_{timestamp}.ppm";
        PPMWriter.SavePPM(p, imageWidth, imageHeight, path);
    }
}
