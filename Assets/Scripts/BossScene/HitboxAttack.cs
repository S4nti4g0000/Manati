using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAttack : MonoBehaviour
{
    float accDamage;
    bool inDamageZone;
    bool impulseApplied; // Flag to track if the impulse has been applied
    GameObject player_;

    float damageWindowDuration = 5.0f; // 5 seconds duration for the damage window
    float backwardImpulseForce = 23.0f;

    public BossHealth boss_;

    // Start is called before the first frame update
    void Awake()
    {
        inDamageZone = false;
        impulseApplied = false; // Initialize the impulseApplied flag
        player_ = GameObject.Find("Capsule");
    }

    // Update is called once per frame
    void Update()
    {
        if (inDamageZone && Input.GetKeyDown(KeyCode.E))
        {
            accDamage += 0.3f;
            Debug.Log("Damage to boss: " + accDamage);

            // Start the damage window timer if it's not already running
            if (!IsInvoking("ResetDamageWindow"))
            {
                Invoke("ResetDamageWindow", damageWindowDuration);
            }
        }
    }

    // Coroutine to reset the damage window
    private IEnumerator ResetDamageWindowCoroutine()
    {
        yield return new WaitForSeconds(damageWindowDuration);
        inDamageZone = false;
        Debug.Log("Damage window ended");
        boss_.ReduceBossHealth_(accDamage);
        accDamage = 0f;

        if (!impulseApplied)
        {
            ApplyBackwardImpulse(); // Apply the impulse when the damage window ends, but only if it hasn't been applied already
        }
    }

    // Reset the damage window
    private void ResetDamageWindow()
    {
        StartCoroutine(ResetDamageWindowCoroutine()); // Start the coroutine
        Debug.Log("Damage window started");
    }

    private void ApplyBackwardImpulse()
    {
        if (player_ != null)
        {
            Rigidbody playerRigidbody = player_.GetComponent < Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 impulseDirection = -player_.transform.right; // Apply force opposite to player's forward direction
                playerRigidbody.AddForce(impulseDirection * backwardImpulseForce, ForceMode.Impulse);

                impulseApplied = true; // Set the flag to indicate that the impulse has been applied
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collided with player");
            inDamageZone = true;
            Debug.Log("Damage window started");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Out of the zone");
            // Don't disable inDamageZone here; wait for the damage window to end
        }
    }
}
