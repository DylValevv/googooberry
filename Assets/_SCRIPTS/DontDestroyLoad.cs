using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
