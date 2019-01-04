using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningPoint : MonoBehaviour {

    // determines the path the car takes when it is turning left/right

    [Header("Left")]
    public GameObject leftPath;          // has to be 3 points
    public Vector2 leftExitDirection; 

    [Header("Right")]
    public GameObject rightPath;         // has to be 3 points
    public Vector2 rightExitDirection;
}
