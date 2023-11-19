using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstMovement : MonoBehaviour
{
    [SerializeField] int health = 10;
    public GameObject boss; // Reference to the Boss GameObject
    [SerializeField] float xOffset = -5f; // Fixed offset to spawn obstacles ahead of the Boss

    private float initialSpeed = -0.1f; // The initial speed of the obstacle
    private float speedIncrease = 0.05f; // The speed increase every 5 seconds
    private float speed; // The current speed of the obstacle

    int damage_;

    void Awake()
    {
        // Find the Boss object named "Truck"
        boss = GameObject.Find("Truck");
        if (boss == null)
            Debug.LogError("Boss (Truck) object not found. Make sure it has the correct name in the scene.");
    }

    // Use this for initialization
    void Start()
    {
        // Set the initial position of the obstacle with the fixed x-offset
        if (boss != null)
        {
            Vector3 initialPosition = new Vector3(boss.transform.position.x + xOffset, 6.63f, transform.position.z);
            transform.position = initialPosition;
        }
        speed = initialSpeed; // Set the initial speed
        // Schedule autodestruction
        Invoke("Autodestruction", health);

        // Start a repeating method to increase speed every 5 seconds
        InvokeRepeating("IncreaseSpeed", 5.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Move forward
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            GameObject player = collision.collider.gameObject;
            HealthPlayer healthPlayer = player.GetComponent<HealthPlayer>();

            if (healthPlayer != null)
            {
                Debug.Log("Found script!");
                // Access healthPlayer's methods or properties as needed.
                healthPlayer.ReduceHealth(1);
            }
            else
            {
                Debug.Log("HealthPlayer script not found on the player GameObject.");
            }
        }
    }

    void Autodestruction()
    {
        Destroy(gameObject);
    }

    void IncreaseSpeed()
    {
        speed += speedIncrease; // Increase the speed
    }
}
