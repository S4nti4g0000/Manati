using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerator : MonoBehaviour
{
    public Autoscrollmovement amovScript;
    public List<GameObject> objects = new List<GameObject>();
    public GameObject player_;
    public GameObject checkr_;

    int time = 5;
    int distance = 40;
    bool isSpawning = false;

    // Store references to the spawned objects.
    List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if (objects.Count == 0)
        {
            Debug.Log("List is empty");
        }

        StartCoroutine(Spawner());
    }

    IEnumerator Spawner()
    {
        if (isSpawning)
            yield break;

        isSpawning = true;

        while (true)
        {
            distance += 15;
            Vector3 spawnPos = player_.transform.position + new Vector3(distance, 0, 0);

            yield return new WaitForSeconds(time);
            // Pass the desired Y position (adjust the value as needed)
            float desiredYPosition = 5.60f;
            SpawnAndDestroyLast(spawnPos, desiredYPosition);
        }

        isSpawning = false;
    }

    private void SpawnAndDestroyLast(Vector3 spawnPos, float desiredYPosition)
    {
        int numberOfObjectsToSpawn = Random.Range(1, 1);

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            float zOffset = Random.Range(-3f, 3f);
            GameObject prefabTS = objects[Random.Range(0, objects.Count)];
            GameObject newObject = Instantiate(prefabTS, new Vector3(spawnPos.x, desiredYPosition, spawnPos.z + zOffset), Quaternion.identity);

            spawnedObjects.Add(newObject);
        }

        Instantiate(checkr_, new Vector3(spawnPos.x, desiredYPosition, spawnPos.z), Quaternion.identity);
    }

    void Update()
    {
        if (amovScript.GetScore() >= 5)
        {
            DestroyObstaclesOutOfCamera();
            GameObject[] checkers = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == "checker(Clone)").ToArray();
            foreach (var checker in checkers)
            {
                Destroy(checker);
            }
        }

        
    }

    private void DestroyObstaclesOutOfCamera()
    {
        // Get the camera's boundaries.
        float cameraLeftX = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        float cameraRightX = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;

        // Create a list to store obstacles to destroy.
        List<GameObject> obstaclesToDestroy = new List<GameObject>();

        // Check which obstacles are out of the camera's range.
        foreach (var obstacle in spawnedObjects)
        {
            float obstacleRightX = obstacle.transform.position.x + obstacle.transform.localScale.x;
            float obstacleLeftX = obstacle.transform.position.x;

            if (obstacleRightX < cameraLeftX || obstacleLeftX > cameraRightX)
            {
                // This obstacle is out of the camera range; mark it for destruction.
                obstaclesToDestroy.Add(obstacle);
            }
        }

        // Destroy the marked obstacles and remove them from the list.
        foreach (var obstacle in obstaclesToDestroy)
        {
            Destroy(obstacle);
            spawnedObjects.Remove(obstacle);
        }

        this.gameObject.SetActive(false);
    }
}
