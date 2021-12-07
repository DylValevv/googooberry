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

    [SerializeField] DialogueStart Npc1;
    [SerializeField] DialogueStart Npc2;
    [SerializeField] DialogueStart Npc3;
    [SerializeField] DialogueStart Npc4;


    [SerializeField] GameState gameState;
    [SerializeField] CrystalCores core;
    [SerializeField] PlayerController player;
    [SerializeField] Notification notification;
    AmbientFunctions ambientFunctions;
    public bool useController = true;
    private int lastCrystalProgress;

    public struct Control
    {
        public Control(string controller, string keyboard)
        {
            this.controller = controller;
            this.keyboard = keyboard;
        }
        public string controller;
        public string keyboard;
    }
    Control Look = new Control("R_ANALOG", "MOUSE");
    Control Move = new Control("L_ANALOG", "WASD");
    Control Jump = new Control("A", "SPACE");
    Control Attack = new Control("X", "LMOUSE");
    Control Dodge = new Control("B", "SHIFT");
    Control StartTalk = new Control("X", "E");
    Control ContinueTalk = new Control("X", "E");
    Control ExitTalk = new Control("B", "ESC");
    Control Slam = new Control("Y", "CTRL");

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

        //DontDestroyOnLoad(transform.parent);
    }

    private void Start()
    {
        
        ambientFunctions = GetComponent<AmbientFunctions>();
        player = FindObjectOfType<PlayerController>();
        StartGame();
        Notify("Welcome to Moon-Sighted.", 
            $"{GetSprite(Move)} to move. {GetSprite(Look)} to look.\n {GetSprite(Jump)} to jump.\n {GetSprite(StartTalk)} to interact.\n {GetSprite(Dodge)} to dodge.\n");
        lastCrystalProgress = -1;
    }

    private void Update()
    {
        if (IntChanged(gameState.crystalProgress, lastCrystalProgress))
        {
            ambientFunctions.Environment(gameState.crystalProgress);
            lastCrystalProgress = gameState.crystalProgress;
            Debug.Log("Core" + gameState.crystalProgress + " activated");
            Execute("Core" + gameState.crystalProgress);
        }
    }

    #region Helper
    public void StartGame()
    {
        gameState.crystalProgress = 0;
        //core.isMined = 0;
        //core.Health = 3;
    }

    private bool IntChanged(int newVal, int oldVal)
    {
        bool res = newVal == oldVal;
        return !(res);
    }

    private void SwapDialogue(DialogueStart dialogueStart, string newDialogue)
    {
        dialogueStart.scene = newDialogue;//for inspector visibility
        dialogueStart.dialogue = new Dialogue(newDialogue, "Conversation");
    }
    public string GetSprite(Control control)
    {
        string res;
        if (gameState.useController)
        {
            res = $"<sprite name=\"{control.controller}\">";
        }
        else
        {
            res = $"<sprite name=\"{control.keyboard}\">";

        }
        return res;
    }
    public void Execute(string function)
    {
        Invoke(function, 0);
    }


    #endregion

    #region Universal
    private void ScreenFade()
    {

    }
   
    private void Notify(string title, string message)
    {
        notification.Notify(title, message);
        // Debug.Log(message);
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
    private void Core0()
    {
        //starting sequence
        
        //player lock dash, projectile, slam
        player.LockDash();
        player.LockRanged();
        player.LockSlam();

        //npc dialogue initialization
        SwapDialogue(Odei, "C.0.ODEI");
        SwapDialogue(Ila, "C.0.ILA");
        SwapDialogue(Tilak, "C.0.TILAK");
        SwapDialogue(Olent, "C.0.OLENT");

        //SwapDialogue(Miak, "C.0.MIAK");
        //SwapDialogue(Npc1, "C.0.NPC1");
        //SwapDialogue(Npc2, "C.0.NPC2");
        //SwapDialogue(Npc3, "C.0.NPC3");
    }
    private void ActivatePlayerDash()
    {
        StepOdeiDialogue();
        player.UnlockDash();
        Notify("Air Dash Unlocked", $"Press {GetSprite(Jump)} in the air. Zoom forward and use the power in your wings.");
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
    private void Core1()
    {
        //screen shake?
        //destroy previous obstacles

        //player lock dash, projectile, slam
        player.UnlockDash();
        player.LockRanged();
        player.LockSlam();

        //npc dialogue initialization
        SwapDialogue(Odei, "C.1.ODEI");
        SwapDialogue(Ila, "C.1.ILA");
        SwapDialogue(Tilak, "C.1.TILAK");
        SwapDialogue(Olent, "C.1.OLENT");

        //SwapDialogue(Miak, "C.1.MIAK");
        //SwapDialogue(Npc1, "C.1.NPC1");
        //SwapDialogue(Npc2, "C.1.NPC2");
        //SwapDialogue(Npc3, "C.1.NPC3");
    }

    private void ActivatePlayerProjectile()
    {
        player.UnlockRanged();
        Notify("Projectile Finisher Unlocked", $"Press {GetSprite(Attack)} three times to trigger your finisher. Notice the projectile that shoots forward!");
    }
    #endregion

    #region Core2
    private void Core2()
    {
        //more screen shake?
        //destroy previous obstacles

        //player lock dash, projectile, slam
        player.UnlockDash();
        player.UnlockRanged();
        player.LockSlam();

        //npc dialogue initialization
        SwapDialogue(Odei, "C.2.ODEI");
        SwapDialogue(Ila, "C.2.ILA");
        SwapDialogue(Tilak, "C.2.TILAK");
        SwapDialogue(Olent, "C.2.OLENT");

        //SwapDialogue(Miak, "C.2.MIAK");
        //SwapDialogue(Npc1, "C.2.NPC1");
        //SwapDialogue(Npc2, "C.2.NPC2");
        //SwapDialogue(Npc3, "C.2.NPC3");
    }
    private void ActivatePlayerSlam()
    {
        player.UnlockSlam();
        Notify("Illarka Burst Unlocked", $"Press {GetSprite(Slam)} on the ground to cause a mighty burst, demolishing enemies in it's path. Use the ultimate power of the illarka crystal.");
    }
    #endregion

    #region Core3
    private void Core3()
    {
        //everyone die and stuff
    }
    #endregion
}
