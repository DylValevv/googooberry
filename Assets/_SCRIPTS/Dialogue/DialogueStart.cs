using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class DialogueStart : MonoBehaviour
{
    //Dialogue start must access scene, and serve dialogue to a single dialogue zone
    Dialogue dialogue;//allows dialogue trigger to use dialogue constructor class we set up
    public DialogueZone dialogueZone;
    [SerializeField]
    private string SceneInCaps;
    private int index = 0;

    //PUBLIC options for Inspector
    [SerializeField]
    private bool oneTimeDialogue;
    [SerializeField]
    private bool typeSentence;
    public bool ready = false;
    [SerializeField]
    private GameObject talkIcon;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dialogue = new Dialogue(SceneInCaps, "Conversation");//we now have the dialogue
        talkIcon.transform.localScale.Set(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ready && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("entered convo");
            dialogueZone.gameObject.SetActive(true);
            dialogueZone.transform.DOMoveY(80, .2f);
            NextSentence();
        }
    }

    private void NextSentence()
    {
        //set speaker
        if (dialogue.sentences[index].Contains("//"))
            switchSpeaker(dialogue.sentences[index]);

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
        dialogueZone.speakerName.text = s;
        dialogueZone.portrait.sprite = Resources.Load<Sprite>("PORTRAITS/" + s) ?? Resources.Load<Sprite>("PORTRAITS/DEFAULT");
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

