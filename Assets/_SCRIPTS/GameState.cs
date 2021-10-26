using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{
    public Object[] crystalCores;
    //float, telling how many crystals have been mined
    public float crystalProgress;

    public void getProgress()
    {
        float progress = 0;
        foreach (Object i in crystalCores) //the line the error is pointing to
        {
            //progress += i.isMined;
        }
        crystalProgress = progress;
    }
}
