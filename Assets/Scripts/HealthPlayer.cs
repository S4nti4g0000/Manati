using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{

    public int health;
    public int MaxHealth = 100;

    private Slider hSlider_;

    private void Awake()
    {
        hSlider_ = GameObject.Find("PlayerSlider").GetComponent<Slider>();
        health = MaxHealth; // Set the health to its maximum value
        UpdateSlider(); // Update the slider with the initial health value.
    }

    public void ReduceHealth(int amount)
    {
        health -= amount;

        // Make sure health doesn't go below 0.
        health = Mathf.Max(0, health);

        UpdateSlider(); // Update the slider with the new health value.
    }

    void UpdateSlider()
    {
        if (hSlider_ != null)
        {
            // Update the slider value based on health.
            hSlider_.value = health;
        }
    }
}
