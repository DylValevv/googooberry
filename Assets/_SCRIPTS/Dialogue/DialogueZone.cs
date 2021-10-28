using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueZone : MonoBehaviour
{
    private void Start()
    {
        transform.position.Set(transform.position.x, -80, transform.position.z);
        gameObject.SetActive(false);
    }
    public TextMeshProUGUI content;
    public Image portrait;
    public TextMeshProUGUI speakerName;
}
