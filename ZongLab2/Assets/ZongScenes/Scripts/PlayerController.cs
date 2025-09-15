using UnityEngine;

// Controls player movement and direction in the maze
public class PlayerController : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }
    private Direction currentDirection = Direction.Up;
    private Vector2Int currentPosition;
    int[,] maze;

    // Move the player based on input
    public void Move(Vector2 input)
    {
        if (input.y > 0)
        {
            Vector2Int directionVec;

            switch (currentDirection)
            {
                case Direction.Up: directionVec = new Vector2Int(1, 0); break;
                case Direction.Left: directionVec = new Vector2Int(0, -1); break;
                case Direction.Down: directionVec = new Vector2Int(-1, 0); break;
                case Direction.Right: directionVec = new Vector2Int(0, 1); break;
                default: directionVec = Vector2Int.zero; break;
            }

            Vector2Int nextPos = currentPosition + directionVec;
            if (nextPos.x >= 0 && nextPos.x < maze.GetLength(0) &&
                nextPos.y >= 0 && nextPos.y < maze.GetLength(1) &&
                maze[nextPos.x, nextPos.y] == 0)
            {
                transform.Translate(Vector3.forward, Space.Self);
                currentPosition = nextPos;
            }
        }
        else
        {
            transform.Rotate(0, input.x * 90 + input.y * 180, 0);
            currentDirection = (Direction)(((int)currentDirection - input.x - input.y * 2 + 4) % 4);
        }
    }

    // Set the maze data and starting position
    public void SetupInitialGame(int[,] mazeData, Vector2Int startPos)
    {
        maze = mazeData;
        currentPosition = startPos;
    }
}
