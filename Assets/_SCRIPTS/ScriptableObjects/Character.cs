using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterName", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject
{
    public Sprite portrait;
    public int soundIndex;
    public float[] sounds;
    public Color textColor;
}
