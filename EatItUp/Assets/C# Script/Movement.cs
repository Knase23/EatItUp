using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Vector3 Direction;
    [Range(1,10)]
    public float speed;
    public bool pac;
    Vector3 spawnPoint;
    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
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
        if (pac)
        {
            if (Direction.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 90);
            if (Direction.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, -90);
            if (Direction.y > 0)
                transform.rotation = Quaternion.Euler(0, 0, 180);
            if (Direction.y < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void TeleportToSpawnPoint()
    {
        transform.position = spawnPoint;
        Direction = Vector3.zero;
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

    public override bool Equals(object obj)
    {
        if (!(obj is MoveCommand))
        {
            return false;
        }

        var command = (MoveCommand)obj;
        return dir.Equals(command.dir) &&
               EqualityComparer<Movement>.Default.Equals(component, command.component);
    }

    public override int GetHashCode()
    {
        var hashCode = -555351148;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(dir);
        hashCode = hashCode * -1521134295 + EqualityComparer<Movement>.Default.GetHashCode(component);
        return hashCode;
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