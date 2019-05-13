using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    public static PlayerHandler inst;

    InputController[] controllers;
    public Character[] playableChracters;

    private InputController currentHolderOfPac;


    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        controllers = GetComponentsInChildren<InputController>();
        GameManager.INSTANCE.AssignPlayersToControllers(ref controllers);


        List<int> selected = new List<int>();
        //Randomize the Controllers Playercharacter
        foreach (var controller in controllers)
        {
            int rand = Random.Range(0,playableChracters.Length);
            while (selected.Contains(rand))
                rand = Random.Range(0, playableChracters.Length);

            selected.Add(rand);
            controller.controlledCharacter = playableChracters[rand];
            controller.controlledCharacter.SetCurrentController(controller);
        }

    }
    public void SetCurrentHolderOfPac(InputController current)
    {
        currentHolderOfPac = current;
    }
    public void GivePointsForPacman()
    {
        currentHolderOfPac.GetComponent<Score>().AddToValue(10);
    }
    public void ResetCharactersPosition()
    {
        foreach (var character in playableChracters)
        {
            character.GetMovement()?.TeleportToSpawnPoint();
        }
    }
}
