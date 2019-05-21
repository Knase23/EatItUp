using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    public long id;
    public InputController currentController;
    Movement movement;

    protected void OnStart()
    {
        movement = GetComponent<Movement>();
    }
    public Movement GetMovement()
    {
        return movement;
    }
    private void FixedUpdate()
    {
        if(GameManager.INSTANCE.IsTheHost())
        {
            Movement.MovementData movmentData = new Movement.MovementData(transform.position.x, transform.position.y, transform.rotation.eulerAngles.z, id);
            if (movement.CheckIfDiffrentLocation(movmentData))
            {
                movement.SetLatestUpdatedPosition(movmentData);

                if (DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.CHARACTER_POSITION, movmentData.ToBytes()))
                {
                    //Debug.Log("Update clients, Charachter Position");
                }
            }
        }
    }
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y);
    }
    public void SetRotation(float z)
    {
        transform.rotation = Quaternion.Euler(0, 0, z); ;
    }
    public void SetCurrentController(InputController controller)
    {
        currentController = controller;
        controller.controlledCharacter = this;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = currentController.color;

        if (tag == "Pac")
            PlayerHandler.inst.SetCurrentHolderOfPac(controller);
    }
    public InputController GetCurrentController()
    {
        return currentController;
    }

}
