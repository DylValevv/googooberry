using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueZone : MonoBehaviour
{
    public TextMeshProUGUI content;
    public Image portrait;
    public TextMeshProUGUI speakerName;
    public static DialogueZone instance;
    public Image portrait2;
    public int position;
    public bool typeSentence;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        transform.position.Set(transform.position.x, -position, transform.position.z);
        gameObject.SetActive(false);
    }

    public void setCharacter(Character character, string name)
    {
        if (character.portraitRight)
        {
            portrait2.sprite = character.portrait;   

        }
        else
        {
            portrait.sprite = character.portrait;
        }
        portrait.gameObject.SetActive(!character.portraitRight);
        portrait2.gameObject.SetActive(character.portraitRight);

        speakerName.color = character.textColor;
        content.color = character.textColor;
        portrait.sprite = character.portrait;
        speakerName.text = name;
    }
}
