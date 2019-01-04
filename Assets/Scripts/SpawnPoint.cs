using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public enum Direction { Up, Down, Left, Right };

    public Direction carDirection;

    public Vector2 GetDirection()
    {
        switch (carDirection)
        {
            case Direction.Down:
                return Vector2.down;
            case Direction.Up:
                return Vector2.up;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
}
