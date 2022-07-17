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

    Vector2Int fireDir = Vector2Int.zero;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (name == "enemy: 0")
            {

            }

            if (!readyAttack)
            {
                if (Vector2Int.Distance(target, index) > 1 && (target.x == index.x || target.y == index.y))
                {
                    readyAttack = true;
                    curMarker = new GameObject();
                    Vector2Int _curIndex = index;
                    fireDir = new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y));
                    for (int i = 0; i < range; i++)
                    {
                        _curIndex += fireDir;

                        if (GM.GetBoardCellState(_curIndex) != 99 && GM.GetBoardCellState(_curIndex) != -1)
                        {
                            Transform _cur = Instantiate(shootMarker, new Vector3(_curIndex.x, 0, _curIndex.y),
                                Quaternion.identity).transform;

                            _cur.forward = new Vector3(fireDir.x, 0, fireDir.y);
                            _cur.transform.parent = curMarker.transform;
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
                    Coroutine _move = StartCoroutine(Move());
                    yield return _move;
                }
            }
            else
            {
                spriteRenderer.sprite = defaultSprite;
                Destroy(curMarker);

                Coroutine _attack = StartCoroutine(Attack());

                readyAttack = false;
                yield return _attack;
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
            Vector2Int[] _otherDirs = new Vector2Int[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, -_prefferedDir.y),
            };

            float _closetDir = 10000;
            int _closetIndex = -1;

            for (int i = 0; i < _otherDirs.Length; i++)
            {
                if (Vector2Int.Distance(target, _otherDirs[i]) < _closetDir)
                {
                    _closetIndex = i;
                    _closetDir = Vector2Int.Distance(target, _otherDirs[i]);
                }
            }

            _returnDirs.Add(_otherDirs[_closetIndex]);

            for (int i = 0; i < _otherDirs.Length; i++)
            {
                if (i != _closetIndex)
                {
                    _returnDirs.Add(_otherDirs[i]);
                }
            }
        }

        else if (_prefferedDir.x != 0)
        {
            Vector2Int[] _otherDirs = new Vector2Int[]
            {
                (new Vector2Int(0, 1)),
                (new Vector2Int(0, -1)),
                (new Vector2Int(-_prefferedDir.x, 0)),
            };

            float _closetDir = 10000;
            int _closetIndex = -1;

            for (int i = 0; i < _otherDirs.Length; i++)
            {
                if (Vector2Int.Distance(target, _otherDirs[i]) < _closetDir)
                {
                    _closetIndex = i;
                    _closetDir = Vector2Int.Distance(target, _otherDirs[i]);
                }
            }

            _returnDirs.Add(_otherDirs[_closetIndex]);

            for (int i = 0; i < _otherDirs.Length; i++)
            {
                if (i != _closetIndex)
                {
                    _returnDirs.Add(_otherDirs[i]);
                }
            }
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

    private void OnDestroy()
    {
        Destroy(curMarker);
    }

    IEnumerator Attack()
    {
        if (name == "enemy: 6")
        {
            print("attackig");
        }
        GM.RangedAttack(index, fireDir, range, attack);
        yield return new WaitForSeconds(0.1f);
    }
}
