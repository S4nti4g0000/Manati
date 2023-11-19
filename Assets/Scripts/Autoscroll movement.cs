using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Rendering;

public class Autoscrollmovement : MonoBehaviour
{
    public Movement mov;
    public Rigidbody rb;
    public GameObject mani;
    public TMP_Text scoreTxt;
    public Animator pAnim;

    float scroll_speed = 0.03f;
    public float z_speed = 0.0f;
    public float jumpForce = 8.0f;
    bool jumping = false;
    bool isBouncing = false;

    int direction;
    public int scoreInt;

    void Start()
    {
        mov.enabled = false;
        StartCoroutine(ScrollSpeedIncrease());
    }

    void Update()
    {
        mov.gameObject.transform.position = new Vector3(transform.position.x + scroll_speed, transform.position.y, transform.position.z);
        aScrollMov();

        if (isBouncing)
        {
            direction *= -1; // Reverse direction.
            isBouncing = false; // Disable bouncing so it only happens once.
        }
    }

    private void aScrollMov()
    {

        pAnim.SetTrigger("Swimming");
        

        if (Input.GetKey(KeyCode.S))
        {
            direction = -1;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            direction = 1;
        }
        else
        {
            //--- Slow down ---//
            if (z_speed > 0.0f)
                z_speed -= 4.0f * Time.deltaTime;
            else
                z_speed = 0.0f;
        }

        if (Input.GetKey(KeyCode.Space) && !jumping)
        {
            jumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            pAnim.SetTrigger("Jump");
        }

        if (direction != 0)
        {
            z_speed += 1.5f * Time.deltaTime;
            z_speed = Mathf.Clamp(z_speed, 0.0f, 5.0f);
            mani.transform.Translate(Vector3.forward * z_speed * direction * Time.deltaTime);
            
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("F_Collider"))
        {
            jumping = false;
        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            scroll_speed = -0.007f;
        }
        else if (collision.collider.CompareTag("World")) // Check for "World" tag.
        {
            // Set isBouncing to true to initiate a bounce.
            isBouncing = true;
        }
        else
        {
            scroll_speed = 0.03f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("checker"))
        {
            scoreInt++;
            scoreTxt.text = "Score: " + scoreInt.ToString();
        }
    }

    public int GetScore()
    {
        return scoreInt;
    }

    IEnumerator ScrollSpeedIncrease()
    {
        bool active = true;
        while (active)
        {
            if (scroll_speed < 0.03f)
            {
                scroll_speed += 0.01f;
                jumpForce += 0.1f;
            }else
            {
                yield return new WaitForSeconds(3f);
                scroll_speed += 0.01f;
                jumpForce += 0.1f;

                if (scroll_speed == 0.1)
                    active = false;
                else active = true;
            }         

            
        }
    }
}
