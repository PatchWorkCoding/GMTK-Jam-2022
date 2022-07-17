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
                //print("called");
                yield return StartCoroutine(Move());
            }

            else
            {
                yield return StartCoroutine(Attack());
                break;
            }

            curMoves++;
        }


        TurnOver();
    }

    protected override Vector2Int[] GeneratePossibleDirections()
    {
        List<Vector2Int> _dirs = new List<Vector2Int>();
        if (target.x != index.x)
        {
            if (target.x > index.x)
            {
                _dirs.Add(new Vector2Int(1, 0));
                spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                _dirs.Add(new Vector2Int(-1, 0));
                spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (target.y != index.y)
        {
            if (target.y > index.y)
            {
                _dirs.Add(new Vector2Int(0, 1));
                spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _dirs.Add(new Vector2Int(0, -1));
                spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (_dirs[0].y != 0)
        {
            _dirs.Add(new Vector2Int(1, 0));
            _dirs.Add(new Vector2Int(-1, 0));
            _dirs.Add(new Vector2Int(0, -_dirs[0].y));
        }

        else if (_dirs[0].x != 0)
        {
            _dirs.Add(new Vector2Int(0, 1));
            _dirs.Add(new Vector2Int(0, -1));
            _dirs.Add(new Vector2Int(-_dirs[0].x, 0));
        }

        return _dirs.ToArray();
        //return base.GeneratePossibleDirections();
    }

    IEnumerator Attack()
    {
        spriteRenderer.sprite = attackSprite;
        Vector2Int _dir = index - target;
        if (_dir.x != 0)
        {
            spriteRenderer.transform.localScale =
            new Vector3(_dir.x, 1, 1);
        }
        else if (_dir.y != 0)
        {
            spriteRenderer.transform.localScale =
            new Vector3(-_dir.y, 1, 1);
        }

        spriteRenderer.transform.localScale = 
            new Vector3(index.x - target.x != 0 ? index.x - target.x : 1, 1, 
            (index.y - target.y != 0 ? -(index.y - target.y) : 1));

        GM.Attack(target, attack);

        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = defaultSprite;
    }
}
