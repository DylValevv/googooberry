using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;
    public bool useStaticBillBoard;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!useStaticBillBoard) transform.LookAt(cameraTransform);
        else transform.rotation = cameraTransform.rotation;

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
