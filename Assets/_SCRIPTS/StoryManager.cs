using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An object that has necessary references to all story based stuff
//given a string, run a function, use Invoke
public class StoryManager : MonoBehaviour
{
    [SerializeField] DialogueStart Ila;
    [SerializeField] DialogueStart Tilak;
    [SerializeField] DialogueStart Odei;
    [SerializeField] DialogueStart Miak;
    [SerializeField] DialogueStart Olent;
    [SerializeField] GameState gameState;
    [SerializeField] PlayerController player;


    public static StoryManager instance;

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

        DontDestroyOnLoad(gameObject);
    }

    #region Helper
    #endregion

    #region Universal
    public void Execute(string function)
    {
        Invoke(function, 0);
    }
    private void FullHealCharacter()
    {
        gameState.AddPlayerHealth(gameState.maxPlayerHealth);
    }
    private void HealCharacter2()
    {
        gameState.AddPlayerHealth(2);
    }
    #endregion

    #region Core0
    //TLDR:
    //Go to odei to turn in crystal
    //Go to Ila to heal 
    //Go to Tilak to gain finisher
    //Odei will tell him to explore new area
    //Use finisher to open up blockade
    //Enter area 1, there is a room fight some baddies, once you click
    private void ActivatePlayerFinisher()
    {
        StepOdeiDialogue();
        player.SetThirdHit();
    }

    //call after speaking with Tilak
    private void StepOdeiDialogue()
    {
        Odei.dialogue = new Dialogue("C.0.ODEI+", "Conversation");
    }

    private void StepTilakDialogue()
    {
        //Odei.dialogue = new Dialogue("C.0.TILAK1", "Conversation");
    }
    #endregion

    #region Core1
    #endregion

    #region Core2
    #endregion

    #region Core3
    #endregion
}
