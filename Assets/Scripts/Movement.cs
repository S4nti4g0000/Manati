using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float Speed;
    GameObject character;
    int direction;
    public float SwimmingUp;
    private float decay;
    bool Grounded;

    Vector3 prevPos;
    Vector3 initPos;
    private Rigidbody rb;

    bool swimmingTransition;

    private void Awake()
    {
        initPos = transform.position;
    }

    void Start()
    {
        character = this.gameObject;
        Speed = 0.0f;
        direction = 0;
        SwimmingUp = 0.00f;
        decay = 0.03f;

        rb = character.GetComponent<Rigidbody>();

        swimmingTransition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(character.transform.position.y < 6.7f)
        {
            if(!swimmingTransition)
            {
                swimmingTransition = true;

                SwimmingUp = 0.0f;
                Speed = 0.0f;
                rb.velocity = Vector3.zero;
            }
            MovementSwimming();
        }
            

        if (character.transform.position.y >= 6.7f)
        {
            swimmingTransition = false;
            MovementLand();
        }
            

        prevPos = character.transform.position;

    }

    private void MovementSwimming()
    {
        rb.useGravity = false;

        //--- Set Rotation ---//
        if (Input.GetKey(KeyCode.D))
            character.transform.Rotate(0, -0.5f, 0);
        else if (Input.GetKey(KeyCode.A))
            character.transform.Rotate(0, 0.5f, 0);

        //--- Set Direction ---//
        if (Input.GetKey(KeyCode.S))
            direction = -1;
        else if (Input.GetKey(KeyCode.W))
            direction = 1;
        else
        {
            //--- Slow down ---//
            if (Speed > 0.0f)
                Speed -= 4.0f * Time.deltaTime;
            else
                Speed = 0.0f;
        }

        //--- Swim up and down ---//
        if (Input.GetKey(KeyCode.Space))
        {
            SwimmingUp += 0.001f;
            // Apply an upward force to the Rigidbody
            rb.AddForce(Vector3.up * SwimmingUp, ForceMode.Impulse);
            SwimmingUp = Mathf.Clamp(SwimmingUp, 0.0f, 0.01f);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            SwimmingUp += 0.01f;
            // Apply a downward force to the Rigidbody
            rb.AddForce(Vector3.down * SwimmingUp, ForceMode.Impulse);
            SwimmingUp = Mathf.Clamp(SwimmingUp, 0.0f, 0.01f);
        }
        else
        {
            if (SwimmingUp > 0.0f)
                SwimmingUp -= 0.008f * Time.deltaTime;
            else { SwimmingUp = 0.0f; rb.velocity = Vector3.zero; }
                

        }

        //--- Move character ---//
        if (direction != 0)
        {
            Speed += 2.5f * Time.deltaTime;
            Speed = Mathf.Clamp(Speed, 0.0f, 5.0f);
            character.transform.Translate(Vector3.right * Speed * direction * Time.deltaTime);
        }
    }

    private void MovementLand()
    {
        rb.useGravity = true;

        // Check if the Shift key is held down for faster movement.
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 6.0f : 3.0f;

        //--- Move forward ---//
        if (Input.GetKey(KeyCode.W))
            direction = 1;
        else if (Input.GetKey(KeyCode.S))
            direction = -1;
        else
        {
            if (Speed > 0.0f)
                Speed -= 4.9f * Time.deltaTime;
            else
                Speed = 0.0f;
        }

        //--- Jumping ---//
        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            rb.AddForce(Vector3.up * 3.5f, ForceMode.Impulse);
            Grounded = false;
        }

        if (Input.GetKey(KeyCode.D))
            character.transform.Rotate(0, -0.5f, 0);
        else if (Input.GetKey(KeyCode.A))
            character.transform.Rotate(0, 0.5f, 0);

        if (direction != 0)
        {
            Speed += 1.0f * Time.deltaTime;
            Speed = Mathf.Clamp(Speed, 0.0f, moveSpeed); // Use moveSpeed for clamping.
            character.transform.Translate(Vector3.right * Speed * direction * Time.deltaTime);
        }
    }

    public void BackToOrigin()
    {
        character.transform.position = initPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("World") && this.enabled == true)
        {
            character.transform.position = prevPos;
            Speed -= 5.0f;
            Grounded = true;
        }            
    }
}
