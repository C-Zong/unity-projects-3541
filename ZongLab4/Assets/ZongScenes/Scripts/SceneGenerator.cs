using UnityEngine;

// Generates a scene with rooms on top and bottom mirrored along the center.
public class SceneGenerator : MonoBehaviour
{
    // Main function to generate the entire scene
    public void GenerateScene(bool[,] grid, int size, int minRoomSize, int maxRoomSize, int minDoorSize, int maxDoorSize, GameObject wallPrefab, Transform parent)
    {
        GenerateFrontier(grid, size);
        GenerateRooms(grid, size, minRoomSize, maxRoomSize, minDoorSize, maxDoorSize);
        GenerateWalls(grid, size, wallPrefab, parent);
    }

    // Generates the outer walls of the grid
    void GenerateFrontier(bool[,] grid, int size)
    {
        for (int i = 0; i < size; i++)
        {
            grid[i, 0] = true;
            grid[i, size - 1] = true;
            grid[0, i] = true;
            grid[size - 1, i] = true;
        }
    }

    // Generates rooms with walls and doors, mirrored along the center
    void GenerateRooms(bool[,] grid, int size, int minRoomSize, int maxRoomSize, int minDoorSize, int maxDoorSize)
    {
        int currentX = 1;
        int currentY = 1;
        int x = Random.Range(currentX + minRoomSize + 1, currentX + maxRoomSize + 2);
        int y = Random.Range(currentY + minRoomSize + 1, currentY + maxRoomSize + 2);
        while (x < size)
        {
            GenerateWallWithDoor(grid, currentX, currentY, x, y, Random.Range(minDoorSize, maxDoorSize + 1));
            currentX = x + 1;
            x = Random.Range(currentX + minRoomSize + 1, currentX + maxRoomSize + 2);
            y = Random.Range(currentY + minRoomSize + 1, currentY + maxRoomSize + 2);
        }
        MirrorGridCenter(grid, 0, (size + 1) / 2, size - 1, size - 1, size);
    }

    // Generates a wall with a door between two points
    void GenerateWallWithDoor(bool[,] grid, int x1, int y1, int x2, int y2, int doorSize)
    {
        int doorPosition = Random.Range(x1, x2 - doorSize + 1);
        int x = x1;
        while (x <= x2)
        {
            if (x == doorPosition)
            {
                x += doorSize;
            }
            else
            {
                grid[x, y2] = true;
                x++;
            }
        }
        doorPosition = Random.Range(y1, y2 - doorSize + 1);
        int y = y1;
        while (y <= y2)
        {
            if (y == doorPosition)
            {
                y += doorSize;
            }
            else
            {
                grid[x2, y] = true;
                y++;
            }
        }
    }

    // Mirrors the grid along the center line
    void MirrorGridCenter(bool[,] grid, int x1, int y1, int x2, int y2, int size)
    {
        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                grid[x, y] = grid[size - 1 - x, size - 1 - y];
            }
        }
    }

    // Instantiates wall prefabs at the positions marked in the grid
    void GenerateWalls(bool[,] grid, int size, GameObject wallPrefab, Transform parent)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y])
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, parent);
                    wall.name = $"Wall_{x}_{y}";
                    wall.tag = "Wall";
                }
            }
        }
    }
}
