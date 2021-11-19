using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    // Start is called before the first frame update
    public float current;
    public float max;
    [SerializeField] private Transform progress;

    void Start()
    {
        progress.localScale = new Vector3(1.05f, 1.1f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        progress.localScale = new Vector3(current/max, 1.1f, 1.1f);
    }
}
