using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public BossHealth bHealth_;
    public playermovement playerMovement;

    public GameObject player_;
    public GameObject prefabToInstantiate;
    public GameObject specialPrefab; // The special obstacle prefab
    public float followSpeed = 2.0f;
    public float instantiateInterval = 5.0f;
    private float instantiateTimer = 0.0f;
    private float specialObstacleTimer = 0.0f; // Timer for spawning special obstacles

    private bool isMovingToZero = false;
    private Vector3 targetPosition;

    private bool inSpecialPhase = false;
    private bool specialObstaclesSpawned = false;
    private bool inSecondPhase = false;
    private float specialPhaseTimer = 0.0f;
    private float specialPhaseDuration = 2.0f;
    private float timeBetweenSpecialPhases = 10.0f;

    private int obstacleCount = 0; // Count the number of obstacles

    void Start()
    {
        instantiateTimer = instantiateInterval;
        StartCoroutine(CyclicSpecialPhases());
    }

    void Update()
    {
        if(bHealth_.GetHealth() <= 50)
        {
            inSecondPhase = true;
            playerMovement.SetSecondPhase(inSecondPhase);
        }

        if (!inSpecialPhase)
        {
            BossMovement();
            InstantiatePrefab();
        }
        else
        {
            HandleSpecialPhase();
        }

        if(bHealth_.GetHealth() >= 50)
            SpawnSpecialObstacle(); // Check for special obstacle spawning       
    }

    void BossMovement()
    {
        if (player_ != null)
        {
            Vector3 bossPosition = transform.position;
            Vector3 playerPosition = player_.transform.position;
            float newZ = Mathf.Lerp(bossPosition.z, playerPosition.z, Time.deltaTime * followSpeed);
            transform.position = new Vector3(bossPosition.x, bossPosition.y, newZ);
        }
    }

    void InstantiatePrefab()
    {
        instantiateTimer -= Time.deltaTime;

        // Wait for 10 seconds before starting instantiation
        if (instantiateTimer <= 0 && player_ != null && player_.transform.position.x < 56)
        {
            if (prefabToInstantiate != null)
            {
                // Determine how many objects to instantiate (either 1 or 2)
                int numToInstantiate = Random.Range(1, 3);

                // Create a list to store the instantiated objects
                List<GameObject> instantiatedObstacles = new List<GameObject>();

                // Define the Z positions between which the objects can spawn
                float minZ = -4f;
                float maxZ = 4f;

                // Instantiate objects
                for (int i = 0; i < numToInstantiate; i++)
                {
                    Vector3 playerPosition = player_.transform.position;
                    float xOffset = -5; // Adjust the range as needed
                    float zOffset = Random.Range(minZ, maxZ); // Randomize between -4 and 4

                    // If more than one obstacle is being instantiated at the same time
                    if (numToInstantiate > 1)
                    {
                        // Ensure they spawn at the same X position
                        xOffset = xOffset; // You can set this to a fixed value for the same X position
                    }

                    GameObject obstacle = Instantiate(prefabToInstantiate, playerPosition + new Vector3(xOffset, 0, zOffset), Quaternion.identity);

                    // Check if the obstacle is outside the z-axis boundary (-4 and 4)
                    if (obstacle.transform.position.z < minZ || obstacle.transform.position.z > maxZ)
                    {
                        Destroy(obstacle);
                    }
                    else
                    {
                        instantiatedObstacles.Add(obstacle);
                    }
                }

                for (int i = 0; i < instantiatedObstacles.Count; i++)
                {
                    for (int j = 0; j < instantiatedObstacles.Count; j++)
                    {
                        if (i != j && Mathf.Approximately(instantiatedObstacles[i].transform.position.z, instantiatedObstacles[j].transform.position.z))
                        {
                            // Reposition the obstacle to have a different Z position within the limits of -4 and 4
                            float newZOffset = Random.Range(minZ, maxZ);
                            Vector3 newPosition = new Vector3(instantiatedObstacles[i].transform.position.x, instantiatedObstacles[i].transform.position.y, newZOffset);
                            instantiatedObstacles[i].transform.position = newPosition;
                        }
                    }
                }

                // Reset the timer for the next instantiation
                instantiateTimer = instantiateInterval;
            }
        }
        
    }

    void HandleSpecialPhase()
    {
        if (specialPhaseTimer <= 0 && !specialObstaclesSpawned)
        {
            if (!isMovingToZero)
            {
                // Delay the obstacle instantiation by 0.5 seconds
                StartCoroutine(DelayedObstacleInstantiation(0.5f));
            }
            else
            {
                targetPosition = transform.position;
                targetPosition.z = 0f;
            }
            isMovingToZero = true;
        }

        if (specialPhaseTimer <= -specialPhaseDuration)
        {
            isMovingToZero = false;
            Debug.Log("Special Phase Ended");
            inSpecialPhase = false;
            specialObstaclesSpawned = false;
        }
        else
        {
            specialPhaseTimer -= Time.deltaTime;
        }

        if (isMovingToZero)
        {
            float moveSpeed = 2.0f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }

    void SpawnSpecialObstacle()
    {
        // Check if it's time to spawn a special obstacle
        if (specialObstacleTimer <= 0 && !specialObstaclesSpawned)
        {
            float zPosition = Random.Range(-4f, 4f); // Random Z-axis position

            // Spawn the special obstacle
            if(bHealth_.GetHealth() >= 50)
                InstantiateSpecialObstacle(zPosition);

            specialObstacleTimer = Random.Range(5f, 10f); // Set the timer for the next special obstacle
        }
        else
        {
            specialObstacleTimer -= Time.deltaTime;
        }
    }

    void InstantiateSpecialObstacle(float zPosition)
    {
        if (specialPrefab != null && (bHealth_.GetHealth() >= 50))
        {
            Vector3 bossPosition = transform.position;
            float xOffset = -5;

            GameObject obstacle = Instantiate(specialPrefab, bossPosition + new Vector3(xOffset, 0, zPosition), Quaternion.identity);
        }
    }

    IEnumerator StartSpecialPhaseDelay()
    {
        yield return new WaitForSeconds(15f);
        Debug.Log("Special Phase Started");
        specialPhaseTimer = 0.0f;
        inSpecialPhase = true;
    }

    IEnumerator DelayedObstacleInstantiation(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Spawn the special obstacles only if they haven't been spawned in this special phase.
        InstantiateSpecialObstacle(4f);
        InstantiateSpecialObstacle(0f);
        InstantiateSpecialObstacle(-4f);

        specialObstaclesSpawned = true; // Set the flag to true to indicate that special obstacles have been spawned

        Debug.Log("Moving to Z = 0");

        targetPosition = transform.position;
        targetPosition.z = 0f;
    }

    IEnumerator CyclicSpecialPhases()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpecialPhases);
            StartCoroutine(StartSpecialPhaseDelay());
        }
    }
}