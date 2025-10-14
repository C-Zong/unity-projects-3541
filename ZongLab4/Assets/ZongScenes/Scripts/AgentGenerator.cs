using UnityEngine;
using System.Collections.Generic;

// Generates predator and prey agents in the scene
public class AgentGenerator : MonoBehaviour
{
    Color predatorColor = Color.red;
    Color preyColor = Color.green;
    HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

    // Generate agents at random empty positions in the grid
    public void GenerateAgent(bool[,] grid, int size, int numOfPreys, int numOfPredators, GameObject agentPrefab, Transform parent)
    {
        for (int i = 0; i < numOfPreys; i++)
        {
            Vector3 position = FindRandomEmptyPosition(grid, size);
            if (position != Vector3.negativeInfinity)
            {
                float randomY = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(0, randomY, 0);
                GameObject agent = Instantiate(agentPrefab, position, randomRotation, parent);
                agent.name = $"Prey_{i}";
                agent.tag = "Prey";
                agent.GetComponent<Renderer>().material.color = preyColor;
                agent.AddComponent<PreyAgent>();
                occupied.Add(new Vector2Int((int)position.x, (int)position.z));
            }
        }

        for (int i = 0; i < numOfPredators; i++)
        {
            Vector3 position = FindRandomEmptyPosition(grid, size);
            if (position != Vector3.negativeInfinity)
            {
                float randomY = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(0, randomY, 0);
                GameObject agent = Instantiate(agentPrefab, position, randomRotation, parent);
                agent.name = $"Predator_{i}";
                agent.tag = "Predator";
                agent.GetComponent<Renderer>().material.color = predatorColor;
                agent.AddComponent<PredatorAgent>();
                occupied.Add(new Vector2Int((int)position.x, (int)position.z));
            }
        }
    }

    // Find a random empty position in the grid
    Vector3 FindRandomEmptyPosition(bool[,] grid, int size)
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = Random.Range(1, size - 1);
            int y = Random.Range(1, size - 1);
            if (!grid[x, y] && !occupied.Contains(new Vector2Int(x, y)))
            {
                occupied.Add(new Vector2Int(x, y));
                return new Vector3(x, 0.5f, y);
            }
        }
        Debug.LogWarning("Failed to find empty position for agent.");
        return Vector3.negativeInfinity;
    }
}
