using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cinemachine;

public class WorldGen : MonoBehaviour
{

    public Autoscrollmovement amovScript;
    public Movement moveScript;
    public RandomGenerator randomScript;

    public List<GameObject> toInstantiate = new List<GameObject>();
    public GameObject mainMap;
    public GameObject floor;
    public CinemachineVirtualCamera virtualCamera;

    private Vector3 spawnPosition;
    private GameObject generator;

    int cScore;

    float zP;
    float yP;

    void Start()
    {
        spawnPosition = transform.position;
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset.x = 3.75f;
        
    }

    void Update()
    {
        cScore = amovScript.GetScore();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cScore < 5)
        {
            Debug.Log("Collided Correctly");
            foreach (GameObject obj in toInstantiate)
            {
                zP = spawnPosition.z;
                yP = spawnPosition.y;

                // Instantiate the object and store a reference to it.
                GameObject instantiatedObj = Instantiate(obj, new Vector3(spawnPosition.x + 52.3f, yP - 5.75f, zP + 3), Quaternion.identity);

                spawnPosition += Vector3.right * obj.transform.localScale.x;
            }
        }

        if (other.CompareTag("Player") && cScore >= 5 && mainMap != null)
        {
            generator = GetComponent<GameObject>();
            Debug.Log("Should instantiate");
            zP = spawnPosition.z;
            yP = spawnPosition.y;
            Instantiate(mainMap, new Vector3(spawnPosition.x + 47.3f, yP - 5.75f, zP + 3), Quaternion.identity);

            amovScript.enabled = false;

            if (moveScript != null)
            {
                moveScript.enabled = true;
            }
            AdjustCameraOffset();
            floor.gameObject.SetActive(false);


        }

    }

    private void AdjustCameraOffset()
    {
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset.x = 1.75f;
    }
}