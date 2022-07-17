using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerBehavior :EnemyBehavior
{
    [SerializeField]
    int range = 0;
    [SerializeField]
    GameObject shootMarker = null;

    GameObject curMarker = null;
    bool readyAttack = false;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (!readyAttack)
            {
                if (Vector2Int.Distance(target, index) > 1 && (target.x == index.x || target.y == index.y))
                {
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
                    curMoves = maxMoves;
                }
                else
                {
                    StartCoroutine(Move());
                    yield return new WaitForSeconds(0.5f);
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

            curMoves++;
        }

        TurnOver();
    }

    protected override Vector2Int[] GeneratePossibleDirections()
    {
        List<Vector2Int> _returnDirs = new List<Vector2Int>();

        Vector2Int _prefferedDir = Vector2Int.zero;

        if (Vector2Int.Distance(target, index) <= 1)
        {
            if (target.x == index.x)
            {
                _prefferedDir = new Vector2Int(0, index.y - target.y);
            }

            else if (target.y == index.y)
            {
                _prefferedDir = new Vector2Int(index.x - target.x, 0);
            }
        }

        else
        {
            if (target.x != index.x)
            {
                if (target.x > index.x)
                {
                    _prefferedDir = (new Vector2Int(1, 0));
                    spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    _prefferedDir = (new Vector2Int(-1, 0));
                    spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else if (target.y != index.y)
            {
                if (target.y > index.y)
                {
                    _prefferedDir = (new Vector2Int(0, 1));
                    spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    _prefferedDir = (new Vector2Int(0, -1));
                    spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        _returnDirs.Add(_prefferedDir);

        if (_prefferedDir.y != 0)
        {
            _returnDirs.Add(new Vector2Int(1, 0));
            _returnDirs.Add(new Vector2Int(-1, 0));
            _returnDirs.Add(new Vector2Int(0, -_prefferedDir.y));
        }

        else if (_prefferedDir.x != 0)
        {
            _returnDirs.Add(new Vector2Int(0, 1));
            _returnDirs.Add(new Vector2Int(0, -1));
            _returnDirs.Add(new Vector2Int(-_prefferedDir.x, 0));
        }

        return _returnDirs.ToArray();
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

    IEnumerator Attack()
    {
        GM.RangedAttack(index, new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y)), range, attack);
        yield return new WaitForSeconds(0.1f);
    }
}
