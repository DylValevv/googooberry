using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalData", menuName = "CrystalCores", order = 1)]
public class CrystalCores : ScriptableObject
{

    public int maxHealth = 10;

    public int isMined = 0; //0 if not mined, 1 if mined

}
