using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBehavior : EnemyBehavior
{
    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (Vector2Int.Distance(target, index) > 1)
            {
                
                if (target.x != index.x)
                {
                    if (target.x > index.x)
                    {
                        Move(new Vector2Int(1, 0));
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        Move(new Vector2Int(-1, 0));
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (target.y != index.y)
                {
                    if (target.y > index.y)
                    {
                        Move(new Vector2Int(0, 1));
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        Move(new Vector2Int(0, -1));
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }

            else
            {
                GM.Attack(target, attack);
                yield return new WaitForSeconds(0.5f);
                break;
            }

            curMoves++;
        }


        TurnOver();
    }

    void Move(Vector2Int _dir)
    {
        if (GM.Move(index, _dir))
        {
            transform.position = transform.position + new Vector3(_dir.x, 0, _dir.y);
            index += _dir;
        }
    }
}
