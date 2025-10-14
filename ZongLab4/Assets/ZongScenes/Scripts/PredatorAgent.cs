using UnityEngine;
using System.Collections.Generic;

// Predator agent that alternates between wandering and chasing prey, with wall avoidance in both modes
public class PredatorAgent : MonoBehaviour
{
    public static GameObject linePrefab;
    public static float visionRange;
    public static float visionAngle;
    public static int rayCount;
    public static float speed;
    public static float rotationSpeed;
    public static float wallAvoidanceThreshold;
    public static float minDistanceToWall;

    public static bool visionVisualization = true;
    bool currentVisualization;

    GameObject[] lines;
    float minDistanceToPrey;
    Transform targetPrey;
    Vector3 avoidWallDirection = Vector3.zero;
    List<Vector3> alongWalls = new List<Vector3>();

    // Initialize vision rays
    void Start()
    {
        lines = new GameObject[rayCount];
        GameObject vision = new GameObject("Vision");
        vision.transform.parent = transform;
        for (int i = 0; i < rayCount; i++)
        {
            lines[i] = Instantiate(linePrefab, vision.transform);
            LineRenderer lineRenderer = lines[i].GetComponent<LineRenderer>();
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        currentVisualization = true;
    }

    void Update()
    {
        PositionCorrection();
        VisionUpdate();
        PositionUpdate();
        VisualizationUpdate();
    }

    // Reset position if out of bounds
    void PositionCorrection()
    {
        if (transform.position.x < 0 || transform.position.z < 0 || transform.position.x > 40 || transform.position.z > 40)
        {
            transform.position = PreyAgent.spawnPoint;
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }

    // Update vision rays and detect prey and walls
    void VisionUpdate()
    {
        targetPrey = null;
        minDistanceToPrey = float.MaxValue;
        avoidWallDirection = Vector3.zero;
        alongWalls.Clear();

        float angleStep = visionAngle / (rayCount - 1);
        float startAngle = -visionAngle / 2f;
        Vector3 eyePos = transform.position + Vector3.up * 0.4f;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 endPoint = eyePos + dir * visionRange;

            RaycastHit[] hits = Physics.RaycastAll(eyePos, dir, visionRange);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Predator"))
                {
                    endPoint = hit.point;
                    float distanceToWall = Vector3.Distance(eyePos, hit.point);

                    // If too close to wall, align along the wall
                    if (distanceToWall < minDistanceToWall)
                    {
                        Vector3 wallNormal = hit.normal;
                        Vector3 alongWall = Vector3.Cross(wallNormal, Vector3.up);
                        if (Vector3.Dot(alongWall, transform.forward) < 0)
                            alongWall = -alongWall;
                        alongWalls.Add(alongWall);
                    }
                    // If within avoidance threshold, steer away from wall
                    else if (distanceToWall < wallAvoidanceThreshold)
                    {
                        avoidWallDirection += (eyePos - hit.point) / distanceToWall * (wallAvoidanceThreshold - distanceToWall);
                    }
                    break;
                }
                else if (hit.collider.CompareTag("Prey"))
                {
                    endPoint = hit.point;
                    float distanceToPrey = Vector3.Distance(eyePos, hit.point);
                    if (distanceToPrey <= 1)
                    {
                        hit.collider.transform.position = PreyAgent.spawnPoint;
                        hit.collider.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    }
                    else if (distanceToPrey < minDistanceToPrey)
                    {
                        minDistanceToPrey = distanceToPrey;
                        targetPrey = hit.collider.transform;
                    }
                    break;
                }
            }

            LineRenderer lineRenderer = lines[i].GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, eyePos);
            lineRenderer.SetPosition(1, endPoint);
        }
    }

    // Update position and orientation based on prey and wall avoidance
    void PositionUpdate()
    {
        // If aligning along walls, change direction accordingly
        if (alongWalls.Count > 0)
        {
            bool isSameDirection = true;
            Vector3 avgAlongWall = Vector3.zero;
            foreach (var alongWall in alongWalls)
            {
                if (Vector3.Dot(alongWall, alongWalls[0]) <= 0)
                    isSameDirection = false;
                avgAlongWall += alongWall;
            }
            // If along walls have different direction, add a random direction to avoid getting stuck
            if (!isSameDirection)
            {
                switch (Random.Range(0, 4))
                {
                    case 0:
                        avgAlongWall += transform.right;
                        break;
                    case 1:
                        avgAlongWall -= transform.right;
                        break;
                    case 2:
                        avgAlongWall += transform.forward;
                        break;
                    case 3:
                        avgAlongWall -= transform.forward;
                        break;
                }
            }
            avgAlongWall.Normalize();
            if (!isSameDirection)
                avgAlongWall = -avgAlongWall;
            transform.rotation = Quaternion.LookRotation(avgAlongWall);
        }
        // If not aligning, steer away from walls and towards prey if seen
        else
        {
            Vector3 targetDirection = transform.forward;
            if (targetPrey != null)
            {
                targetDirection = (targetPrey.position - transform.position).normalized;
            }
            else if (avoidWallDirection != Vector3.zero)
            {
                targetDirection = avoidWallDirection.normalized;
            }
            targetDirection = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Update visualization of vision rays
    void VisualizationUpdate()
    {
        if (visionVisualization != currentVisualization)
        {
            foreach (var line in lines)
            {
                line.SetActive(visionVisualization);
            }
            currentVisualization = visionVisualization;
        }
    }
}
