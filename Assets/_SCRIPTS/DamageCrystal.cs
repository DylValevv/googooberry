using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DamageCrystal : MonoBehaviour
{
    public CrystalCores crystalData;
    public bool enemiesDefeated = false;
    public GameObject destroyedParticleEffect;
    public GameObject crystalMesh;
    //public Image gameOverPanel;
    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<PlayerController>().gameState;
        /*if (crystalData.isMined == 1)
        {
            gameObject.SetActive(false);
        }*/

        crystalData.isMined = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && enemiesDefeated)
        {
            Debug.Log("crystal core hit");
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        crystalData.Health -= 1;
        if (crystalData.Health <= 0)
        {
            Destroyed();
        }
    }

    private void Destroyed()
    {
        AudioManager.instance.PlayAction("CrystalBreak");
        crystalData.isMined = 1;
        destroyedParticleEffect.SetActive(true);
        crystalMesh.SetActive(false);
        gameState.crystalProgress++;
        //AlphafestGameOver();
    }

    /*
    private void AlphafestGameOver()
    {
        Sequence mySequence = DOTween.Sequence();

        gameOverPanel.gameObject.SetActive(true);

        TextMeshProUGUI deathText = gameOverPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        mySequence.AppendInterval(3f);
        mySequence.Append(gameOverPanel.DOFade(1f, 0.5f));
        mySequence.Append(deathText.DOFade(1f, 0.5f));
    }*/
}
