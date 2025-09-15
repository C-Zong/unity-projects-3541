using UnityEngine;
using System.Collections.Generic;

// Randomized Depth-First Search Maze Generator with BFS to Find the Farthest Point.
// Maze generation and farthest point algorithm mostly generated with AI.
// Hierarchical structure and meaningful names added for clarity.
// Some variable values slightly adjusted for better visual appearance.
// Certain variables made public for easier tweaking in the Unity Inspector.
public class RandomMazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public Vector2Int startPos = new Vector2Int(1, 1);
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public float cubeSize = 1.0f;
    public float lightSpawnChance = 0.1f;
    int[,] maze;
    private GameManager gameManager;

    // Generate and draw the maze on start
    public void GenerateAndDrawMaze()
    {
        gameManager = GetComponent<GameManager>();
        if (width % 2 == 0) width += 1;
        if (height % 2 == 0) height += 1;
        GenerateMaze();
        DrawMaze();
    }

    // Randomized depth-first search maze generation
    void GenerateMaze()
    {
        // initialize maze with walls
        maze = new int[height, width];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                maze[z, x] = 1;
            }
        }

        // Randomized depth-first search
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = startPos;
        maze[current.y, current.x] = 0;
        stack.Push(current);

        // Random directions
        System.Random rand = new System.Random();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(0, 2),
            new Vector2Int(0, -2)
        };

        while (stack.Count > 0)
        {
            current = stack.Pop();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // Find unvisited neighbors
            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x > 0 && neighbor.x < width - 1 &&
                    neighbor.y > 0 && neighbor.y < height - 1 &&
                    maze[neighbor.y, neighbor.x] == 1)
                {
                    neighbors.Add(neighbor);
                }
            }

            // If there are unvisited neighbors, choose one randomly
            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[rand.Next(neighbors.Count)];
                Vector2Int wall = (current + chosen) / 2;
                maze[wall.y, wall.x] = 0;
                maze[chosen.y, chosen.x] = 0;
                stack.Push(chosen);
            }
        }

        // Find farthest point from start
        Vector2Int endPos = FindFarthestPoint(startPos);
        gameManager.SetMazeData(cubeSize, maze, startPos, endPos);
    }

    void DrawMaze()
    {
        // Draw floor
        GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity, this.transform);
        floor.transform.localScale = new Vector3(width * cubeSize / 8f, 1, height * cubeSize / 8f);
        floor.transform.position = new Vector3((width - 1) * cubeSize / 2f, -0.5f, (height - 1) * cubeSize / 2f);
        floor.name = "Floor";

        GameObject walls = new GameObject("Walls");
        walls.transform.parent = this.transform;
        // Draw walls
        for (int z = 0; z < height; z++)
        {
            GameObject rowParent = new GameObject($"Wall_{z}");
            rowParent.transform.parent = walls.transform;
            for (int x = 0; x < width; x++)
            {
                if (maze[z, x] == 1)
                {
                    Vector3 pos = new Vector3(x * cubeSize, 0, z * cubeSize);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, rowParent.transform);
                    wall.name = $"Wall_{z}_{x}";
                }
                // Randomly place lights in open spaces
                else if (maze[z, x] == 0 && Random.value < lightSpawnChance && (x != startPos.x || z != startPos.y))
                {
                    GameObject lightObj = new GameObject($"Light_{z}_{x}");
                    lightObj.transform.position = new Vector3(x * cubeSize, cubeSize * 1.2f, z * cubeSize);
                    lightObj.transform.parent = rowParent.transform;
                    Light lightComp = lightObj.AddComponent<Light>();
                    lightComp.type = LightType.Point;
                    lightComp.range = cubeSize * 3f;
                    lightComp.intensity = 1.5f;
                    lightComp.color = Color.yellow;
                }
            }
        }
    }

    // Breadth-first search to find the farthest point from start
    Vector2Int FindFarthestPoint(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distance[start] = 0;

        Vector2Int[] dirs = new Vector2Int[]
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };

        Vector2Int farthest = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (var d in dirs)
            {
                Vector2Int next = current + d;
                if (next.x > 0 && next.x < width &&
                    next.y > 0 && next.y < height &&
                    maze[next.y, next.x] == 0 && !distance.ContainsKey(next))
                {
                    distance[next] = distance[current] + 1;
                    queue.Enqueue(next);

                    if (distance[next] > distance[farthest])
                        farthest = next;
                }
            }
        }

        return farthest;
    }
}
