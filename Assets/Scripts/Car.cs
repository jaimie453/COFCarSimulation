using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public enum Turning { Left, Right, None }; // none means the car continues straight

    public float maxSpeed = 5f;              
    public Turning turningDirection;         
    public float slowDownDistance = 3f;        // the distance the car starts deaccelerating for a turn
    public float deaccelerationRate = 0.01f;   //  should be small
    public float accelerationRate = 0.01f;

    [HideInInspector] public Vector2 currentDirection;

    private bool isTurning = false, isDeaccelerating = false, isAccelerating = false;
    private Vector3 movement;
    private Rigidbody2D rb2D;
    private ParabolaController parabolaController;
    private Vector2 nextDirection = Vector2.zero;
    private int turningMask;
    private float currentSpeed;

    void Start()
    {
        parabolaController = GetComponent<ParabolaController>();
        rb2D = GetComponent<Rigidbody2D>();
        turningMask = 1 << 8;  // only includes colliders in the 8th layer mask (Turning Point)
        currentSpeed = maxSpeed;
    }

	// Update is called once per frame
	void FixedUpdate () {

        // move the car
        if (!isTurning) 
        {
            movement.Set(currentDirection.x, currentDirection.y, 0f);

            if (isDeaccelerating)
            {
                currentSpeed -= deaccelerationRate;
            } else if (isAccelerating)
            {
                currentSpeed += accelerationRate;
                if(currentSpeed >= maxSpeed)
                {
                    isAccelerating = false;
                    currentSpeed = maxSpeed;
                }
            }

            //Debug.Log("currentSpeed: " + currentSpeed.ToString());
            movement = movement * currentSpeed * Time.deltaTime;
            rb2D.MovePosition(transform.position + movement);
        }

        // calculate the distance between the car and its turning point
        if(turningDirection != Turning.None && !isTurning && !isDeaccelerating)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, Mathf.Infinity, turningMask);
            if(hit.collider != null)
            {
                Vector2 diff = transform.position - hit.transform.position;
                float distance;

                // currentDirection is horizontal
                if (currentDirection == Vector2.right || currentDirection == Vector2.left) 
                    distance = Mathf.Abs(diff.x);
                // currentDirection is vertical
                else
                    distance = Mathf.Abs(diff.y);

                //Debug.Log("distance " + distance.ToString());
                if(distance <= slowDownDistance)
                    isDeaccelerating = true;
            } 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // remove cars once they leave the intersection
        if (other.gameObject.CompareTag("End Point"))
            Destroy(gameObject);

        else if(other.gameObject.CompareTag("Turning Point") && turningDirection != Turning.None)
        {
            isDeaccelerating = false;
            isTurning = true;
            TurningPoint turningPoint = other.gameObject.GetComponent<TurningPoint>();

            GameObject path;

            if (turningDirection == Turning.Left)
            {
                path = turningPoint.leftPath;
                nextDirection = turningPoint.leftExitDirection;
            }
            else
            {
                path = turningPoint.rightPath;
                nextDirection = turningPoint.rightExitDirection;
            }

            parabolaController.ParabolaRoot = path;
            parabolaController.Speed = currentSpeed;
            parabolaController.FollowParabola();
        }
    }

    // stop the car if a crash occurs
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            enabled = false;                     // turns off movement/turning
            parabolaController.enabled = false;
        }
    }

    // called from ParabolaController
    public void EndOfTurn()
    {
        isTurning = false;
        currentDirection = nextDirection;
        nextDirection = Vector2.zero;

        // recenter car's rotation after the turn
        if (currentDirection == Vector2.up || currentDirection == Vector2.down)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        turningDirection = Turning.None;
        isAccelerating = true;
    }

    // used for CarSpawner
    public static Turning GetRandomTurning()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
            return Turning.Left;
        else if (rand == 1)
            return Turning.Right;
        else
            return Turning.None;
    }

    // rotates object to face target in 2D
    public void LookAt2D(Vector3 target)
    {
        Vector3 diff = target - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}
