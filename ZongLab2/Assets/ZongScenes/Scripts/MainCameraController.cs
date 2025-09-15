using UnityEngine;

// Main camera controller to position and orient the camera above the maze.
public class MainCameraController : MonoBehaviour
{
    public RandomMazeGenerator mazeGenerator;
    public Vector3 cameraRotation = new Vector3(90f, 0f, 0f);

    void Start()
    {
        // Get maze dimensions and cube size from the maze generator
        float cubeSize = mazeGenerator.cubeSize;
        int width = mazeGenerator.width;
        int height = mazeGenerator.height;

        // Position the camera above the center of the maze
        int max = width > height ? width : height;
        transform.position = new Vector3(width * cubeSize / 2, max * cubeSize * 2, height * cubeSize / 2);
        transform.rotation = Quaternion.Euler(cameraRotation);
    }
}
