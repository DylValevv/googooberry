using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject playerObj;

    // Start is called before the first frame update
    void Start()
    {
        playerObj = FindObjectOfType<PlayerController>().gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>() == null)
            {
                Debug.Log("this is a wing boyssss: " + other.name);
            }
            else
            {
                playerObj.GetComponent<PlayerController>().Die();
            }
        }
    }
}
