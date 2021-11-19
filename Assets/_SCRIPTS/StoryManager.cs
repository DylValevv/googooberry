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
    [SerializeField] Notification notification;


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

        DontDestroyOnLoad(transform.parent);
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Notify("Welcome to Moon-Sighted.", 
            "<sprite name=\"L_ANALOG\"> to move. <sprite name=\"R_ANALOG\"> to look.\n <sprite name=\"A\"> " +
            "to jump.\n <sprite name=\"X\"> to attack/interact.\n <sprite name=\"B\"> to dodge.\n");
    }

    #region Helper
    #endregion

    #region Universal
    public void Execute(string function)
    {
        Invoke(function, 0);
    }

    private void Notify(string title, string message)
    {
        notification.Notify(title, message);
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
    private void ActivatePlayerDash()
    {
        StepOdeiDialogue();
        player.UnlockDash();
        Notify("Air Dash Unlocked", "Press <sprite name=\"A\"> in the air. Zoom forward and use the power in your wings.");

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
    public void EnemiesEliminatedNotification()
    {
        Notify("Uskerra Eliminated", "Smash the crystal to harvest the ilarka shards.");
    }
    public void EnemiesSpottedNotification()
    {
        Notify("Uskerra Spotted", "The dangerous creatures defend the ilarka core. Defeat them to harvest its shards.");

    }
    #endregion

    #region Core1
    #endregion

    #region Core2
    #endregion

    #region Core3
    #endregion
}
