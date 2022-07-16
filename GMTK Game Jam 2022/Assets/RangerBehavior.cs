using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerBehavior :EnemyBehavior
{
    [SerializeField]
    int range = 0;
    [Header("Sprite Properties")]
    [SerializeField]
    SpriteRenderer spriteRenderer = null;
    [SerializeField]
    Sprite defaultSprite, Walk1, Walk2, attackSprite;
    [SerializeField]
    GameObject shootMarker = null;

    GameObject curMarker = null;
    bool readyAttack = false;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (Vector2Int.Distance(target, index) > 1)
            {
                if (!readyAttack)
                {
                    if (target.x == index.x || target.y == index.y)
                    {
                        //Attack
                        if (!readyAttack)
                        {
                            //print("Will Attack");
                            readyAttack = true;
                            curMarker = new GameObject();
                            Vector2Int _curIndex = index;
                            for (int i = 0; i < range; i++)
                            {
                                _curIndex += new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y));
                                if (GM.GetBoardCellState(_curIndex) != 99)
                                {
                                    Instantiate(shootMarker, new Vector3(_curIndex.x, 0, _curIndex.y), 
                                        Quaternion.identity).transform.parent = curMarker.transform;
                                }

                                else
                                {
                                    break;
                                }
                            }

                            spriteRenderer.sprite = attackSprite;
                            yield return new WaitForSeconds(0.1f);
                        }

                    }

                    else
                    {
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
                }
                else
                {
                    spriteRenderer.sprite = defaultSprite;
                    Destroy(curMarker);
                    StartCoroutine(Attack());
                    readyAttack = false;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                if (GM.GetBoardCellState(index + (index - target)) == 0)
                {
                    StartCoroutine(Move(index - target));
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    readyAttack = true;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            

            curMoves++;
        }


        TurnOver();
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
        GM.RangedAttack(index, new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y)), range, attack);
        yield return new WaitForSeconds(0.1f);
    }
}
