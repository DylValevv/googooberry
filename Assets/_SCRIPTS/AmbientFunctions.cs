using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AmbientFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Volume globalVolume;
    [SerializeField] Transform Mush;
    [SerializeField] Transform Mush_alt;


    [SerializeField] Material Mushroom0_mat;
    [SerializeField] Material Mushroom1_mat;
    [SerializeField] Material Mushroom2_mat;
    [SerializeField] MeshRenderer Mushroom;

    [SerializeField] MeshRenderer Mushroom_alt;
    [SerializeField] Material Mushroom_alt0_mat;
    [SerializeField] Material Mushroom_alt1_mat;
    [SerializeField] Material Mushroom_alt2_mat;


    private void Start()
    {
        globalVolume.enabled = false;
        //Mushroom = Mush.GetChild(0).GetComponent<MeshRenderer>();
        //Mushroom_alt = Mush.GetChild(0).GetComponent<MeshRenderer>();
    }
    public void Environment(int core)
    {
        ColorAdjustments colorAdjustments;
        globalVolume.enabled = true;

        if (globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            switch (core)
            {
                case 0:
                    colorAdjustments.hueShift.value = 0;
                    Mushroom.sharedMaterial = Mushroom0_mat;
                    Mushroom_alt.sharedMaterial = Mushroom_alt0_mat;
                    break;
                case 1:
                    colorAdjustments.hueShift.value = -10;
                    Mushroom.sharedMaterial = Mushroom1_mat;
                    Mushroom_alt.sharedMaterial = Mushroom_alt1_mat;
                    break;
                case 2:
                    colorAdjustments.hueShift.value = -20;
                    Mushroom.sharedMaterial = Mushroom2_mat;
                    Mushroom_alt.sharedMaterial = Mushroom_alt2_mat;
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        } 
    }
  
}
