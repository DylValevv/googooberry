using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamCollider : MonoBehaviour
{
    /// <summary>
    /// kills enemy upon impact
    /// </summary>
    /// <param name="other">game object collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.transform.parent.GetComponent<Enemy>().TakeDamage(100);
        }
    }
}
