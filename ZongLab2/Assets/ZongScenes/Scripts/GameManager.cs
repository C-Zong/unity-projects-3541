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
    bool isSceneRotating = false;
    Vector3 pivotPoint;

    void Start()
    {
        // Get components
        RandomMazeGenerator mazeGenerator = GetComponent<RandomMazeGenerator>();
        GameSetup gameSetup = GetComponent<GameSetup>();

        // Generate maze and setup game
        mazeGenerator.GenerateAndDrawMaze();
        pivotPoint = new Vector3((mazeGenerator.width / 2) * cubeSize, 0, (mazeGenerator.height / 2) * cubeSize);
        gameSetup.SetupGame();

        // Initialize cameras
        MainCamera.enabled = false;
        isMainCameraActive = MainCamera.enabled;
        PlayerCamera.enabled = true;

        // Set player controller's maze data
        playerController = player.GetComponent<PlayerController>();
        playerController.SetupInitialGame(maze, startPos);
    }

    void FixedUpdate()
    {
        if (isMainCameraActive && isSceneRotating)
        {
            this.transform.RotateAround(pivotPoint, Vector3.up, 20 * Time.deltaTime);
        }
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

    // Change scene rotation state if main camera is active
    private void OnSceneRotation()
    {
        if (isMainCameraActive)
        {
            isSceneRotating = !isSceneRotating;
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
