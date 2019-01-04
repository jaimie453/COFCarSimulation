using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

    public GameObject carPrefab;
    public float spawnRate = 1f;
    public Transform carParent; // for organizing cars in hierarchy

    private Color[] carColors = { Color.blue, Color.red, Color.green, Color.yellow };
    private SpawnPoint[] spawnPoints;

	void Start () {
        SetSpawnPoints();
        InvokeRepeating("SpawnCar", 0f, spawnRate);        
	}

    void SetSpawnPoints()
    {
        spawnPoints = new SpawnPoint[transform.childCount];
        int i = 0;
        foreach (Transform spawnPoint in transform)
        {
            spawnPoints[i] = spawnPoint.gameObject.GetComponent<SpawnPoint>();
            i++;
        }
    }

    // spawns a car at one of the spawn points w/ a random color
    void SpawnCar()
    {
        SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 direction = spawnPoint.GetDirection();

        Quaternion rotation = Quaternion.identity;
        if (direction == Vector2.up || direction == Vector2.down)
            rotation.eulerAngles = new Vector3(0, 0, 90);

        GameObject car = Instantiate(carPrefab, spawnPoint.transform.position, rotation, carParent);
        Color randColor = carColors[Random.Range(0, carColors.Length)];

        car.GetComponent<SpriteRenderer>().color = randColor;
        Car carScript = car.GetComponent<Car>();
        carScript.turningDirection = Car.GetRandomTurning();
        carScript.currentDirection = direction;
    }
}
