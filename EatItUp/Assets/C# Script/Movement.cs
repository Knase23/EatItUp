using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Vector3 Direction;
    [Range(1,10)]
    public float speed;

    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    public void Move(Vector3 dir)
    {
        
        if (dir != Vector3.zero)
        {
            Direction = dir;
            Direction.Normalize();
        }
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position + Direction);
        if (screenPos.x < 0 || screenPos.y < 0 ||
            screenPos.x > Screen.width || screenPos.y > Screen.height)
        {
            Direction = Vector3.zero;
        }
        rigidbody2d.velocity = Direction * speed;

        if(Direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        if(Direction.x < 0)
            transform.rotation = Quaternion.Euler(0, 0, -90);
        if(Direction.y > 0)
            transform.rotation = Quaternion.Euler(0, 0, 180);
        if(Direction.y < 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);

    }
}
public struct MoveCommand
{
    Vector2 dir;
    Movement component;

    public MoveCommand(Vector2 direction, Movement componentToManipulate)
    {
        dir = direction;
        component = componentToManipulate;
    }

    public void Do()
    {
        component?.Move(dir);
    }
    public void Undo()
    {
        component?.Move(-dir);
    }
    public static bool operator ==(MoveCommand a, MoveCommand b)
    {
        if (a.dir != b.dir)
            return false;

        if (a.component != b.component)
            return false;

        return true;
    }
    public static bool operator !=(MoveCommand a, MoveCommand b)
    {
        if (a == b)
            return false;

        return true;
    }
}