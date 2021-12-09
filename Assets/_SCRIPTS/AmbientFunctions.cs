using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AmbientFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Volume globalVolume;

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
                    break;
                case 1:
                    colorAdjustments.hueShift.value = -10;
                    break;
                case 2:
                    colorAdjustments.hueShift.value = -20;
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        } 
    }
  
}
