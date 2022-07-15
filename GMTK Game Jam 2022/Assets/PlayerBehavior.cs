using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    GameManager GM;

    Vector2Int index = Vector2Int.zero;
    int health = 0;

    public void Init(GameManager _GM)
    {
        GM = _GM;
        index = Vector2Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(new Vector2Int(0,1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(new Vector2Int(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(new Vector2Int(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(new Vector2Int(1, 0));
        }
    }

    public Vector2Int Index
    {
        get { return index; }
    }

    void Move(Vector2Int _moveDir)
    {
        if (GM.Move(index, _moveDir))
        {
            transform.position = transform.position + new Vector3(_moveDir.x, 0, _moveDir.y);
            index += _moveDir;
        }
    }
}
