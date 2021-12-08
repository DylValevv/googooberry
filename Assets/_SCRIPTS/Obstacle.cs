using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject rockCrumbleVFXprefab;
    [SerializeField] private Transform vfxSpawnLocation;

    [SerializeField] private GameObject pointLight;

    private enum abilityUnlockTag {CrystalSlam, Ranged};
    [SerializeField] private abilityUnlockTag unlockTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == unlockTag.ToString())
        {
            pointLight.SetActive(false);
            AudioManager.instance.PlayAction("ObstacleBreak");
            Instantiate(rockCrumbleVFXprefab, vfxSpawnLocation);
            Destroy(gameObject);
        }
    }
}
