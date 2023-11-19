using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float healthb_;
    public float MaxHealthb_ = 100f;

    private Slider bossSlider_;

    private void Awake()
    {
        bossSlider_ = GameObject.Find("BossSlider").GetComponent<Slider>();
        healthb_ = MaxHealthb_; // Set the health to its maximum value
        UpdateSlider_(); // Update the slider with the initial health value.
    }

    public void ReduceBossHealth_(float amount)
    {
        healthb_ -= amount;

        // Make sure health doesn't go below 0.
        healthb_ = Mathf.Max(0, healthb_);

        UpdateSlider_(); // Update the slider with the new health value.
    }

    void UpdateSlider_()
    {
        if (bossSlider_ != null)
        {
            // Update the slider value based on health.
            bossSlider_.value = healthb_;
        }
    }

    public float GetHealth()
    {
        return healthb_;
    }
}
