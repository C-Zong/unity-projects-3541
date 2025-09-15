using UnityEngine;

// Sets up the game by placing the player and a light at the end position.
public class GameSetup : MonoBehaviour
{
    public GameObject playerPrefab;
    Vector2Int startPos;
    Vector2Int endPos;
    float cubeSize;
    private GameManager gameManager;
    Camera playerCamera;

    // Initializes the game setup
    public void SetupGame()
    {
        gameManager = GetComponent<GameManager>();
        (startPos, endPos) = gameManager.GetPositions();
        cubeSize = gameManager.GetCubeSize();
        playerCamera = gameManager.PlayerCamera;
        gameManager.SetPlayer(SetupPlayer());
        PlaceLightAtEnd();
    }

    // Sets up the player at the starting position and configures the player camera
    GameObject SetupPlayer()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(startPos.x, -0.5f, startPos.y), Quaternion.identity, this.transform);
        player.name = "Player";
        playerCamera.transform.SetParent(player.transform);
        playerCamera.transform.localPosition = new Vector3(0, cubeSize * 0.3f, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        return player;
    }

    // Places a light at the end position
    void PlaceLightAtEnd()
    {
        GameObject endMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        endMarker.name = "EndMarker";
        endMarker.transform.position = new Vector3(endPos.x * cubeSize, 0.3f, endPos.y * cubeSize);
        endMarker.transform.localScale = Vector3.one * (cubeSize * 0.6f);
        endMarker.GetComponent<Renderer>().material.color = Color.red;
        endMarker.transform.SetParent(this.transform);
    }
}
