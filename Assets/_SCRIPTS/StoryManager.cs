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
    [SerializeField] DialogueStart[] NPC;
    [SerializeField] GameState gameState;

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
    private void ActivatePlayerFinisher()
    {

    }
    #endregion

    #region Core1
    #endregion

    #region Core2
    #endregion

    #region Core3
    #endregion
}
