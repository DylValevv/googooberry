using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    /*
     the Dialogue Trigger must
        a) ask for the scene
        b) Determine the dialogue and what goes to who
            this means it must match the speaker tag to the tmp
            this is done by checking the speaker tag(//BRIGHTSQUARE//>) to see when to switch the text
        c) set the tmp text to the dialogue sentence         
    */
    Dialogue dialogue;//allows dialogue trigger to use dialogue constructor class we set up
    DialogueManager manager;
    public string SceneInCaps;
    //public string filename;
    public TextMeshPro activeSpeaker;//gets active speaker from recognised speakers in Dialogue Manager
    private int dSpot = 0;
    Animator anim;

    //PUBLIC options for Inspector
    public bool oneTimeDialogue;
    public bool includePortrait;//useless rn
    public bool typeSentence;
    public bool ready = true;

    public SentenceAnimation sentenceAnimation;

    public enum SentenceAnimation //applied to tmp, consider making children of tmp the 
    {
        Grow,
        Slide,
        Fade,
        None
    }

    private void Start()
    {
        dialogue = new Dialogue(SceneInCaps, "Conversation");//we now have the dialogue
        manager = FindObjectOfType<DialogueManager>();//used exclusively for switchSpeaker() and exitDialogue()
        if (!GetComponent<Animator>())
        {
            sentenceAnimation = SentenceAnimation.None;
        }
        else anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (ready && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("entered convo");
            NextSentence();
        }
    }

    //private void OnDisable()
    //{
    //    ExitDialogue();
    //}

    private void NextSentence()//have the active tmp display the next sentence
    {
        //set speaker
        activeSpeaker = switchSpeaker(dialogue.sentences[dSpot]);

        //sentence effects
        if (sentenceAnimation != SentenceAnimation.None)
        {
            anim.SetBool(sentenceAnimation.ToString(), true);//only works if i make the animation one time and bool instantly turns off afterward
        }

        if (!typeSentence) activeSpeaker.text = dialogue.sentences[dSpot];
        else { StopAllCoroutines(); StartCoroutine(TypeSentence(dialogue.sentences[dSpot])); }

        //next sentence, check if conversation is over
        dSpot++;
        if (dSpot >= dialogue.sentences.Length)
        {
            ExitDialogue();
            if (oneTimeDialogue) ready = false;
        }
    }

    private void ExitDialogue()//deactivates all speaker tmps in manager, resets conversation to beginning
    {
        for (int i = 0; i < manager.speakers.Length; i++)
        {
            manager.speakers[i].gameObject.SetActive(false);//this deactivates all tmps on dialogue end
        }
        dSpot = 0;
    }

    private TextMeshPro switchSpeaker(string s)//TextmeshPros must be named //NAME//, as well as in script, this should sense a speaker change and chenge the active tmp
    {
        TextMeshPro result = activeSpeaker;
        if (s.Contains("//"))
        {
            dSpot++;//skip the speaker tag so it does not display
            for (int i = 0; i < manager.speakers.Length; i++)
            {
                if (manager.speakers[i].name == s)
                {
                    result = manager.speakers[i];//set the correct tmp
                    //this keeps only one dialogue open at a time - subject to change
                    if (activeSpeaker) activeSpeaker.gameObject.SetActive(false);
                    result.gameObject.SetActive(true);
                }
            }
        }
        return result;
    }

    IEnumerator TypeSentence(string line)
    {
        activeSpeaker.text = "";
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
                activeSpeaker.text += s;
                yield return null;
            }
            else
            {
                activeSpeaker.text += letter;
                yield return null;
            }
        }
    }
}