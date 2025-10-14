using UnityEngine;
using System.Collections.Generic;

// Manages the overall game setup, including scene generation and agent instantiation
public class GameManager : MonoBehaviour
{
    const int SIZE = 40;
    bool[,] grid = new bool[SIZE, SIZE];

    [Header("Room Settings")]
    public int minRoomSize = 5;
    public int maxRoomSize = 10;
    public int minDoorSize = 3;
    public int maxDoorSize = 5;

    [Header("General Agent Settings")]
    public float wallAvoidanceThreshold = 5f;
    public float minDistanceToWall = 3f;

    [Header("Prey Agent Settings")]
    public int numOfPreys = 5;
    public float preyVisionRange = 5f;
    public float preyVisionAngle = 330f;
    public int preyRayCount = 165;
    public float preySpeed = 3f;
    public float preyWallDetectionHalfAngle = 15f;
    public float preyRotationSpeed = 1f;

    [Header("Flocking Settings")]
    public float flockingRadius = 5f;
    public float separationDistance = 2f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;

    [Header("Predator Agent Settings")]
    public int numOfPredators = 2;
    public float predatorVisionRange = 10f;
    public float predatorVisionAngle = 30f;
    public int predatorRayCount = 30;
    public float predatorSpeed = 3f;
    public float predatorRotationSpeed = 5f;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject agentPrefab;
    public GameObject linePrefab;

    void Start()
    {
        // Generate the scene
        SceneGenerator sceneGenerator = gameObject.AddComponent<SceneGenerator>();
        GameObject walls = new GameObject("Walls");
        walls.transform.parent = transform;
        sceneGenerator.GenerateScene(grid, SIZE, minRoomSize, maxRoomSize, minDoorSize, maxDoorSize, wallPrefab, walls.transform);

        // Instantiate the agent at a random empty position
        AgentSettingsUpdate();
        AgentGenerator agentGenerator = gameObject.AddComponent<AgentGenerator>();
        GameObject agents = new GameObject("Agents");
        agents.transform.parent = transform;
        agentGenerator.GenerateAgent(grid, SIZE, numOfPreys, numOfPredators, agentPrefab, agents.transform);
    }

    void AgentSettingsUpdate()
    {
        // Update static variables in PreyAgent
        PreyAgent.linePrefab = linePrefab;
        PreyAgent.visionRange = preyVisionRange;
        PreyAgent.visionAngle = preyVisionAngle;
        PreyAgent.speed = preySpeed;
        PreyAgent.rotationSpeed = preyRotationSpeed;
        PreyAgent.rayCount = preyRayCount;
        PreyAgent.wallAvoidanceThreshold = wallAvoidanceThreshold;
        PreyAgent.minDistanceToWall = minDistanceToWall;
        PreyAgent.wallDetectionHalfAngle = preyWallDetectionHalfAngle;
        PreyAgent.flockingRadius = flockingRadius;
        PreyAgent.separationDistance = separationDistance;
        PreyAgent.cohesionWeight = cohesionWeight;
        PreyAgent.separationWeight = separationWeight;
        PreyAgent.alignmentWeight = alignmentWeight;

        // Update static variables in PredatorAgent
        PredatorAgent.linePrefab = linePrefab;
        PredatorAgent.visionRange = predatorVisionRange;
        PredatorAgent.visionAngle = predatorVisionAngle;
        PredatorAgent.speed = predatorSpeed;
        PredatorAgent.rotationSpeed = predatorRotationSpeed;
        PredatorAgent.rayCount = predatorRayCount;
        PredatorAgent.wallAvoidanceThreshold = wallAvoidanceThreshold;
        PredatorAgent.minDistanceToWall = minDistanceToWall;
    }

    // Toggle visualization of vision rays for both Prey and Predator agents
    void OnVisualize()
    {
        PreyAgent.visionVisualization = !PreyAgent.visionVisualization;
        PredatorAgent.visionVisualization = !PredatorAgent.visionVisualization;
    }
}
