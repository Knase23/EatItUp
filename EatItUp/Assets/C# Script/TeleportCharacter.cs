using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCharacter : MonoBehaviour
{
    public Direction directionCharachterNeedsToMoveIn;
    public Transform teleportPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Teleport(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Teleport(collision);
    }
    private void Teleport(Collider2D collision)
    {
        switch (directionCharachterNeedsToMoveIn)
        {
            case Direction.UP:
                if (collision.attachedRigidbody.velocity.y > 0)
                    collision.transform.position = teleportPosition.position;
                break;
            case Direction.DOWN:
                if (collision.attachedRigidbody.velocity.y < 0)
                    collision.transform.position = teleportPosition.position;
                break;
            case Direction.LEFT:
                if (collision.attachedRigidbody.velocity.x < 0)
                    collision.transform.position = teleportPosition.position;
                break;
            case Direction.RIGHT:
                if (collision.attachedRigidbody.velocity.x > 0)
                    collision.transform.position = teleportPosition.position;
                break;
            default:
                break;
        }
    }
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

}
