using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    // Start is called before the first frame update
    public GameState gameState;

    //Player Health
    private int lastPlayerHealth;
    [SerializeField]
    private Transform healthUI;
    [SerializeField]
    private Color healthFullColor;
    [SerializeField]
    private Color healthEmptyColor;
    void Start()
    {
        gameState.playerHealth = 10;
        updateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(FloatChanged(gameState.playerHealth, lastPlayerHealth))
            updateHealth();
    }

    private bool FloatChanged(float newVal, float oldVal)
    {
        bool res = newVal == oldVal;
        oldVal = newVal;
        return !(res);
    }

    private void updateHealth()
    {
        int phealth = gameState.playerHealth;
        int children = healthUI.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            if(phealth > 0)
            {
                healthUI.transform.GetChild(i).GetComponent<Image>().color = healthFullColor;
                phealth--;
            }
            else
            {
                healthUI.transform.GetChild(i).GetComponent<Image>().color = healthEmptyColor;

            }
        }
    }


}
