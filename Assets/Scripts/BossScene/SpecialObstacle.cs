using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialObstacle : MonoBehaviour
{
    [SerializeField] int initialHealth = 5; // Initial health of the obstacle
    int health; // Current health of the obstacle
    public GameObject boss; // Reference to the Boss GameObject
    [SerializeField] float xOffset = -5f; // Fixed offset to spawn obstacles ahead of the Boss

    private float initialSpeed = -0.15f; // The initial speed of the obstacle
    private float speedIncrease = 0.05f; // The speed increase every 5 seconds
    private float speed; // The current speed of the obstacle

    private TMP_Text ui_Interact;
    private bool playerInsideCollider = false;
    private bool directionChanged = false; // Add this flag

    void Awake()
    {
        // Find the Boss object named "Truck"
        boss = GameObject.Find("Truck");
        if (boss == null)
            Debug.LogError("Boss (Truck) object not found. Make sure it has the correct name in the scene.");

        ui_Interact = GameObject.Find("Interaction").GetComponent<TMP_Text>();
        if (ui_Interact == null)
            Debug.LogError("UI Text (Interaction) not found. Make sure it has the correct name in the scene.");
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
        health = initialHealth;
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

        if (playerInsideCollider && Input.GetKey(KeyCode.F) && !directionChanged) // Check the directionChanged flag
        {
            Debug.Log("Pressed F");

            // Reverse the X-speed
            speed = -speed * 3;

            // Extend the life of the obstacle by 5 seconds
            health += 5;

            // Cancel the previous autodestruction and reschedule it
            CancelInvoke("Autodestruction");
            Invoke("Autodestruction", health);

            directionChanged = true; // Set the flag to true
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCollider = true;
            directionChanged = false; // Reset the directionChanged flag

            // Change the text when the player enters the collider
            ui_Interact.text = "Press F";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCollider = false;

            // Reset the text when the player exits the collider
            ui_Interact.text = "";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameObject player = collision.collider.gameObject;
            HealthPlayer healthPlayer = player.GetComponent<HealthPlayer>();

            if (healthPlayer != null)
                healthPlayer.ReduceHealth(2);


        }

        if(collision.collider.CompareTag("hitbox"))
        {
            GameObject boss = collision.collider.gameObject;
            BossHealth bossHealth = boss.GetComponent<BossHealth>();

            if (bossHealth != null)
                bossHealth.ReduceBossHealth_(10);

            Destroy(this.gameObject);
        }
    }
}
