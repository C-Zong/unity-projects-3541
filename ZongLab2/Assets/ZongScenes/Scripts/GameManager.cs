using UnityEngine;
using UnityEngine.InputSystem;

// Central game manager that stores maze data and key positions.
// Also responsible for initializing maze generation and overall game setup.
public class GameManager : MonoBehaviour
{
    public Camera MainCamera;
    public Camera PlayerCamera;
    Vector2Int startPos;
    Vector2Int endPos;
    float cubeSize;
    int[,] maze;
    bool isMainCameraActive;
    GameObject player;
    PlayerController playerController;

    void Start()
    {
        // Get components
        RandomMazeGenerator mazeGenerator = GetComponent<RandomMazeGenerator>();
        GameSetup gameSetup = GetComponent<GameSetup>();

        // Generate maze and setup game
        mazeGenerator.GenerateAndDrawMaze();
        gameSetup.SetupGame();

        // Initialize cameras
        MainCamera.enabled = false;
        isMainCameraActive = MainCamera.enabled;
        PlayerCamera.enabled = true;

        // Set player controller's maze data
        playerController = player.GetComponent<PlayerController>();
        playerController.SetupInitialGame(maze, startPos);
    }

    // Switch between main camera and player camera
    private void OnSwitchView()
    {
        isMainCameraActive = !isMainCameraActive;
        MainCamera.enabled = isMainCameraActive;
        PlayerCamera.enabled = !isMainCameraActive;
    }

    // Handle player movement input
    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        if (player != null)
        {
            playerController.Move(input);
        }
    }

    public void SetMazeData(float cubeSize, int[,] mazeData, Vector2Int startPosition, Vector2Int endPosition)
    {
        this.cubeSize = cubeSize;
        maze = mazeData;
        startPos = startPosition;
        endPos = endPosition;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public (Vector2Int start, Vector2Int end) GetPositions()
    {
        return (startPos, endPos);
    }

    public float GetCubeSize()
    {
        return cubeSize;
    }

    public int[,] GetMaze()
    {
        return maze;
    }
}
