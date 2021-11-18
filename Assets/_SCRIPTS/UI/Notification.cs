using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Notification holds the functions to visually display a notification
/// </summary>
public class Notification : MonoBehaviour
{
    Sequence mySequence;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        createSequence();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void createSequence()
    {
        mySequence = DOTween.Sequence();
        mySequence.Pause();
        mySequence.Append(transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutExpo)).AppendInterval(5).Append(transform.DOScale(new Vector3(0, 0, 0), 1).SetEase(Ease.OutExpo));
    }


    public void Notify(string message)
    {
        textMeshPro.text = message;
        mySequence.Restart();
    }
}