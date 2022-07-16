using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreBehavior : EnemyBehavior
{
    [Header("Sprite Properties")]
    [SerializeField]
    SpriteRenderer spriteRenderer = null;
    [SerializeField]
    Sprite defaultSprite, Walk1, Walk2, attackSprite;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (Vector2Int.Distance(target, index) > 1)
            {
                print("called");
                if (target.x != index.x)
                {
                    if (target.x > index.x)
                    {
                        StartCoroutine(Move(new Vector2Int(1, 0)));
                        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        StartCoroutine(Move(new Vector2Int(-1, 0)));
                        spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (target.y != index.y)
                {
                    if (target.y > index.y)
                    {
                        StartCoroutine(Move(new Vector2Int(0, 1)));
                        spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        StartCoroutine(Move(new Vector2Int(0, -1)));
                        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }

            else
            {
                StartCoroutine(Attack());
                yield return new WaitForSeconds(1f);
                break;
            }

            curMoves++;
        }


        TurnOver();
    }

    IEnumerator Move(Vector2Int _moveDir)
    {
        if (GM.Move(index, _moveDir))
        {
            Vector3 _finalPos = transform.position + new Vector3(_moveDir.x, 0, _moveDir.y);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = Walk1;
            transform.position = transform.position + (new Vector3(_moveDir.x, 0, _moveDir.y).normalized * 0.3f);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = Walk2;
            transform.position = transform.position + (new Vector3(_moveDir.x, 0, _moveDir.y).normalized * 0.3f);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = defaultSprite;
            transform.position = _finalPos;

            index += _moveDir;
        }
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
        StartCoroutine(GM.Player.Move(target - index));

        yield return new WaitForSeconds(0.4f);
        spriteRenderer.sprite = defaultSprite;
    }
}
