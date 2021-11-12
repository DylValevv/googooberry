using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueZone : MonoBehaviour
{
    public TextMeshProUGUI content;
    public Image portrait;
    public TextMeshProUGUI speakerName;
    public static DialogueZone instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        transform.position.Set(transform.position.x, -80, transform.position.z);
        gameObject.SetActive(false);
    }


}
