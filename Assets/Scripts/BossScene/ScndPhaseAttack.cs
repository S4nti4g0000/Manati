using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScndPhaseAttack : MonoBehaviour
{
    Rigidbody rigid;
    public GameObject boss;

    float jumpForce = 10f;
    bool hasJumped = false;

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
        boss = GameObject.Find("Truck");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F) && (transform.position.x >= 56f || transform.position.x == 59f))
        {
            Debug.Log("Pressed F");
            JumpTowardsBoss();            
        }
    }

    void JumpTowardsBoss()
    {
        // Assuming you want to jump upwards towards the boss:
        Vector3 jumpDirection = boss.transform.position - transform.position;
        jumpDirection.y = 2.5f; // You can adjust the jump height as needed
        rigid.AddForce(jumpDirection.normalized * jumpForce, ForceMode.Impulse);

        hasJumped = true; // Set the flag to prevent continuous jumping
    }
}
