using UnityEngine;
using System.Collections.Generic;

// Prey agent that avoids predators, walls, and flocks with other preys, and heads to a goal point when safe
public class PreyAgent : MonoBehaviour
{
    public static GameObject linePrefab;
    public static float visionRange;
    public static float visionAngle;
    public static int rayCount;
    public static float speed;
    public static float rotationSpeed;
    public static float wallAvoidanceThreshold;
    public static float minDistanceToWall;
    public static float wallDetectionHalfAngle;

    public static float flockingRadius;
    public static float separationDistance;
    public static float cohesionWeight;
    public static float separationWeight;
    public static float alignmentWeight;

    public static Vector3 spawnPoint = new Vector3(1.5f, 0.5f, 1.5f);

    public static bool visionVisualization = true;
    bool currentVisualization;


    GameObject[] lines;
    Transform predator;
    Vector3 avoidWallDirection = Vector3.zero;
    List<Vector3> alongWalls = new List<Vector3>();
    Vector3 goalPoint = new Vector3(38.5f, 0.5f, 38.5f);

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
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
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
            transform.position = spawnPoint;
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }

    // Update vision rays and detect prey and walls
    void VisionUpdate()
    {
        predator = null;
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
                if (hit.collider.CompareTag("Wall"))
                {
                    endPoint = hit.point;

                    if (Mathf.Abs(currentAngle) < wallDetectionHalfAngle)
                    {
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
                    }

                    break;
                }
                else if (hit.collider.CompareTag("Predator"))
                {
                    predator = hit.collider.transform;
                    endPoint = hit.point;
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
        // If not aligning, steer away from predator and walls, or flock with other preys, or head to goal
        else
        {
            Vector3 targetDirection = transform.forward;
            if (predator != null)
            {
                targetDirection = (transform.position - predator.position).normalized;
            }
            else if (avoidWallDirection != Vector3.zero)
            {
                targetDirection = avoidWallDirection.normalized;
            }
            else
            {
                targetDirection = CalculateFlockingDirection();
                if (targetDirection != Vector3.zero)
                    targetDirection = targetDirection.normalized;
                else
                    targetDirection = (goalPoint - transform.position).normalized;
            }
            targetDirection = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Calculate flocking direction based on nearby preys
    Vector3 CalculateFlockingDirection()
    {
        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockingRadius);
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        int neighborCount = 0;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.CompareTag("Prey"))
            {
                Vector3 toNeighbor = neighbor.transform.position - transform.position;
                float distance = toNeighbor.magnitude;

                // Cohesion: Move towards the average position of neighbors
                cohesion += neighbor.transform.position;

                // Separation: Avoid getting too close to neighbors
                if (distance < separationDistance)
                {
                    separation -= toNeighbor.normalized / distance;
                }

                // Alignment: Match the average direction of neighbors
                alignment += neighbor.transform.forward;

                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            cohesion = (cohesion / neighborCount - transform.position).normalized * cohesionWeight;
            separation = separation.normalized * separationWeight;
            alignment = alignment.normalized * alignmentWeight;
        }

        return cohesion + separation + alignment;
    }

    // Update visualization of vision rays
    void VisualizationUpdate()
    {
        if (currentVisualization != visionVisualization)
        {
            foreach (var line in lines)
            {
                line.SetActive(visionVisualization);
            }
            currentVisualization = visionVisualization;
        }
    }
}
