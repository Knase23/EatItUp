using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyAI : AI
{


    Pac pac;

    public BlinkyAI(Character controlledCharacter)
    {
        charachter = controlledCharacter;
    }
    public override MoveCommand DecisionOfMovement()
    {
        //if the character is Pacman, 
        // Move to the nearest pebble


        // if we are in the start box
        // Move to startbox outside position


        // Check if we are about to hit a wall in the direction we are currently walk thwords
        //if not and the direction is not Vector2.Zero, use latestMoveCommand 
        // if we are to close to a wall we are about to walk into 
        //      Are we chaseing the player
        //              Then take the direction that takes us closer to our goal position
        //      Check in which direction we can walk in, that is not the opposite way we came.
        //      Then take one of the directions

        return latestMoveCommand;
    }

    public override void Sensors()
    {
        //För varje riktning, kolla om jag har line of sight med Pac.
        // Om jag har det börja följ honom
        //for (int i = 0; i < 4; i++)
        //{
        //    Vector2 direction = i == 0 ? Vector2.up : i == 1 ? Vector2.right : i == 2 ? Vector2.down : Vector2.left;
        //    Ray2D ray = new Ray2D(charachter.transform.position, direction);
        //    RaycastHit2D hit =  Physics2D.Raycast(ray.origin, ray.direction);            
        //}
        // Om inte hittat honom gå framåt
    }
}
