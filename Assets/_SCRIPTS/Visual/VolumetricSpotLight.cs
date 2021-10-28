using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricSpotLight : MonoBehaviour
{
    [SerializeField]
    public Light spotLight;
    [SerializeField]
    public GameObject volume;
    [SerializeField]
    public Material material;
    [Range(1, 20)]
    public float darkness;
    // Start is called before the first frame update
    void Start()
    {
        Material temp = new Material(material);
        Color lightColor = spotLight.color;// new Color(spotLight.color.r / darkness, spotLight.color.g / darkness, spotLight.color.b / darkness);
        temp.SetColor("Color_", lightColor);
        volume.GetComponent<MeshRenderer>().material = temp;
        Debug.Log("Here");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
