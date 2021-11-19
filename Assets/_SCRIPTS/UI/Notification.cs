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
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI content;
    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void createSequence()
    {
        transform.localScale = new Vector3(0, 0, 0);
        Sequence mySequence;
        mySequence = DOTween.Sequence();
        AudioManager.instance.PlayAction("Impact3");
        mySequence.Append(transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutExpo)).AppendInterval(5).Append(transform.DOScale(new Vector3(0, 0, 0), 1).SetEase(Ease.OutExpo));
    }


    public void Notify(string title, string message)
    {
        this.title.text = title;
        content.text = message;
        createSequence();
    }
}
