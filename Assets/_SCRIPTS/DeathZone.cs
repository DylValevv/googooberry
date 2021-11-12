using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject playerObj;

    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerObj.GetComponent<PlayerController>().Die();
        }
    }
}
