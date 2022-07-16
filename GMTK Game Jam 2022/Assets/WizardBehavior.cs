using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBehavior : EnemyBehavior
{
    [Header("Sprite Properties")]
    [SerializeField]
    SpriteRenderer spriteRenderer = null;
    [SerializeField]
    Sprite defaultSprite, Walk1, Walk2, attackSprite;
    [SerializeField]
    int movesBetweenAttacks = 0;

    int movesSinceLastAttack = 0;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (movesSinceLastAttack > movesBetweenAttacks)
            {
                StartCoroutine(Attack());
                movesSinceLastAttack = 0;
                yield return new WaitForSeconds(1f);
                break;
            }
            else
            {
                Vector2Int _dir = Vector2Int.zero;
                if (target.x != index.x)
                {
                    if (target.x > index.x)
                    {
                        _dir += new Vector2Int(1, 0);
                        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        _dir += new Vector2Int(-1, 0);
                        spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                    }
                }

                if (target.y != index.y)
                {
                    if (target.y > index.y)
                    {
                        _dir += new Vector2Int(0, 1);
                        spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                    {
                        _dir += new Vector2Int(0, -1);
                        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                        
                    }
                }

                StartCoroutine(Move(_dir));
                yield return new WaitForSeconds(0.5f);
            }
            movesSinceLastAttack++;
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

            yield return new WaitForSeconds(0.1f);
            index += _moveDir;
        }
    }

    IEnumerator Attack()
    {
        print("did an attack");
        GM.SpellAttack(index + new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y)), new Vector2Int(3,3), attack);
        yield return new WaitForSeconds(0.1f);
    }

    int CollapseToOne(int _a, int _b)
    {
        if (_a - _b != 0)
        {

            if (_a - _b > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return 0;
        }
    }
}
