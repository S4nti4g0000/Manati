using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playermovement : MonoBehaviour
{
    Rigidbody rigid;
    public TMP_Text attackNotice;
    public Animator pAnim;

    [Header("Setup")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float laneDistance;
    [SerializeField] float slideDuration;
    [SerializeField] float slideSpeed;

    int currentLane = 0;
    int desiredLane = 0;
    float advSpeed = 10;
    bool jumping = false;
    bool sliding = false;
    private bool jumpedOverObstacle = false;
    bool secondphase = false;
    bool advancing = true;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(advancing)
        {
            if (transform.position.x <= 56.0f)
            {
                // Continue the advancing behavior
                // Check if the player wants to move to the right
                if (Input.GetKeyDown(KeyCode.A) && desiredLane < 1 && !jumping)
                {
                    desiredLane++;
                    pAnim.SetTrigger("StLeft");
                }
                // Check if the player wants to move to the left
                if (Input.GetKeyDown(KeyCode.D) && desiredLane > -1 && !jumping)
                {
                    desiredLane--;
                    pAnim.SetTrigger("StRight");
                }
                // Check if the player wants to jump
                if (Input.GetKeyDown(KeyCode.Space) && !jumping)
                {
                    rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    jumping = true;
                    pAnim.SetTrigger("Jump");
                }
                // Check if the player wants to slide
                if (Input.GetKeyDown(KeyCode.S) && !jumping && !sliding)
                {
                    sliding = true;
                    Invoke("StopSliding", slideDuration);
                }

                if(Input.GetKeyDown(KeyCode.F))
                    pAnim.SetTrigger("Attack");

                // Move the player horizontally
                Vector3 targetPosition = transform.position;
                targetPosition.z = desiredLane * laneDistance;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Check if the player is in the desired lane
                if (transform.position.z == targetPosition.z)
                    currentLane = desiredLane;

                // Slide the player
                if (sliding)
                    transform.position += Vector3.down * slideSpeed * Time.deltaTime;

                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.S))
                {
                    // Set "Idle" animation
                    pAnim.SetBool("Idle", true);
                }
                else
                {
                    // Reset "Idle" animation
                    pAnim.SetBool("Idle", false);
                }

            }            
        }
        if (transform.position.x >= 57.0f || transform.position.x == 59.0f)
        {
            advancing = false; // Stop advancing when reaching x position 57
            attackNotice.text = "F to attack";
            if (transform.position.x >= 57.0f || transform.position.x <= 56.0f)
                attackNotice.text = "";
        }
        else { advancing = true; }

    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player touched an obstacle
        if (other.CompareTag("JumpMove") && secondphase == true)
        {
            if (jumping && !jumpedOverObstacle)
            {
                // Player jumped over the obstacle
                Debug.Log("Jumped over an obstacle!");
                jumpedOverObstacle = true;
                StartCoroutine(MovePlayerSideways(2.0f));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player has moved past an obstacle
        if (other.CompareTag("JumpMove") && secondphase == true)
        {
            jumpedOverObstacle = false; // Reset the flag when exiting the obstacle trigger
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Reset jumping when the player lands on the ground
        if (collision.gameObject.CompareTag("F_Collider"))
        {
            jumping = false;
            sliding = false;
            rigid.useGravity = true;
        }
    }    

    void StopSliding()
    {
        sliding = false;
    }

    private IEnumerator MovePlayerSideways(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        Vector3 moveDirection = Vector3.right; // The direction to move (right in this case)

        while (Time.time < endTime)
        {
            if (advancing) // Check if we should still advance
            {
                float t = (Time.time - startTime) / duration;
                Vector3 moveAmount = moveDirection * advSpeed * Time.deltaTime;
                rigid.MovePosition(transform.position + moveAmount);
                yield return null;
            }
            else
            {
                yield break; // Exit the coroutine if we're not advancing
            }
        }
    }

    public void SetSecondPhase(bool isSecond)
    {
        secondphase = isSecond;
    }
}