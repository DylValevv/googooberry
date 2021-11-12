using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class DialogueStart : MonoBehaviour
{
    //Dialogue start must access scene, and serve dialogue to a single dialogue zone
    public Dialogue dialogue;//allows dialogue trigger to use dialogue constructor class we set up
    public DialogueZone dialogueZone;
    [SerializeField]
    private StoryManager storyManager;
    public string scene;
    private int index = 0;

    //PUBLIC options for Inspector
    [SerializeField]
    private bool oneTimeDialogue;
    [SerializeField]
    private bool typeSentence;
    public bool ready = false;
    [SerializeField]
    private GameObject talkIcon;

    [SerializeField]
    private AudioManager audioManager;

    private Character character;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dialogue = new Dialogue(scene, "Conversation");//we now have the dialogue
        talkIcon.transform.localScale.Set(0, 0, 0);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioManager.PlayDialogue("moth4");
            ready = true;
            talkIcon.SetActive(true);
            talkIcon.transform.DOScale(1f, .3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ready = false;
            talkIcon.transform.DOScale(0f, .3f).OnComplete(()=>talkIcon.SetActive(false));
            ExitDialogue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("What the waht");

        }
        if (ready && Input.GetKeyDown(KeyCode.E))
        {
            dialogueZone.gameObject.SetActive(true);
            dialogueZone.transform.DOMoveY(80, .2f);
            NextSentence();
        }
    }

    private void NextSentence()
    {
        //set speaker
        string s = dialogue.sentences[index];

        while (dialogue.sentences[index].Contains("//"))
        {
            if (dialogue.sentences[index].Contains("//:"))
            {
                storyManager.Execute(s.TrimStart('/', ':'));
                index++;
            }
            else
            {
                switchSpeaker(dialogue.sentences[index]);
            }
        }



        audioManager.PlayDialogue("moth" + character.sounds[character.soundIndex++ % character.sounds.Length]);
        if (!typeSentence) dialogueZone.content.text = dialogue.sentences[index];
        else { StopAllCoroutines(); StartCoroutine(TypeSentence(dialogue.sentences[index])); }

        //next sentence, check if conversation is over
        index++;
        if (index >= dialogue.sentences.Length)
        {
            ExitDialogue();
            if (oneTimeDialogue) ready = false;
        }
    }

    private void ExitDialogue()
    {
        index = 0;
        dialogueZone.transform.DOMoveY(-80f, .3f).OnComplete(() => dialogueZone.gameObject.SetActive(false));
    }

    private void switchSpeaker(string s)
    {
        dialogueZone.transform.DOMoveY(-80, .2f);
        s = s.Trim('/');
        index++;//skip the speaker tag so it does not display
        character = Resources.Load<Character>("CHARACTERS/" + s) ?? Resources.Load<Character>("CHARACTERS/DEFAULT");
        dialogueZone.speakerName.color = character.textColor;
        dialogueZone.content.color = character.textColor;

        dialogueZone.speakerName.text = s;
        dialogueZone.portrait.sprite = character.portrait;
        dialogueZone.transform.DOMoveY(80, .2f);

    }

    IEnumerator TypeSentence(string line)
    {
        dialogueZone.content.text = "";
        string s = "";
        char letter;
        for (int i = 0; i < line.Length; i++)
        {
            letter = line.ToCharArray()[i];
            if (letter == '<')
            {
                while (letter != '>' && i + 1 < line.Length)
                {

                    s += letter;
                    i++;
                    letter = line.ToCharArray()[i];
                }
                s += letter;
                dialogueZone.content.text += s;
                yield return null;
            }
            else
            {
                dialogueZone.content.text += letter;
                yield return null;
            }
        }
    }
}

