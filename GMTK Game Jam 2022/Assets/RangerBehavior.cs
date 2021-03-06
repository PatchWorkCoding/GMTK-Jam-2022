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
            yield return new WaitForSeconds(0.25f);

            if (name == "enemy: 1")
            {

            }

            if (!readyAttack)
            {
                if (Vector2Int.Distance(target, Index) > 1 && (target.x == Index.x || target.y == Index.y))
                {
                    readyAttack = true;
                    curMarker = new GameObject();
                    Vector2Int _curIndex = Index;
                    fireDir = new Vector2Int(CollapseToOne(target.x, Index.x), CollapseToOne(target.y, Index.y));
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

        if (GM.GetBoardCellState(index) <= 0)
        {
            GM.SetBoardCellState(index, 1);
        }

        TurnOver();
    }

    protected override Vector2Int[] GeneratePossibleDirections()
    {
        List<Vector2Int> _returnDirs = new List<Vector2Int>();

        Vector2Int _prefferedDir = Vector2Int.zero;

        if (Vector2Int.Distance(target, Index) <= 1)
        {
            if (target.x == Index.x)
            {
                _prefferedDir = new Vector2Int(0, Index.y - target.y);
            }

            else if (target.y == Index.y)
            {
                _prefferedDir = new Vector2Int(Index.x - target.x, 0);
            }
        }

        else
        {
            if (target.x != Index.x)
            {
                if (target.x > Index.x)
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
            else if (target.y != Index.y)
            {
                if (target.y > Index.y)
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

        if (_returnDirs[0].y != 0)
        {
            _returnDirs.Add(new Vector2Int(1, 0));
            _returnDirs.Add(new Vector2Int(-1, 0));
            _returnDirs.Add(new Vector2Int(0, -_returnDirs[0].y));
        }

        else if (_returnDirs[0].x != 0)
        {
            _returnDirs.Add(new Vector2Int(0, 1));
            _returnDirs.Add(new Vector2Int(0, -1));
            _returnDirs.Add(new Vector2Int(-_returnDirs[0].x, 0));
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
        //int _dst = GM.RangedAttack(Index, fireDir, range, attack);
        int _dst = 0;
        Vector2Int _attackIndex = GM.RangedAttack(Index, fireDir, range, attack, out _dst);
        
        if (fireDir == new Vector2Int(0, 1))
        {
            transform.GetChild(1).localScale = new Vector3(-1, 1, 1);
        }
        else if (fireDir == new Vector2Int(0, -1))
        {
            transform.GetChild(1).localScale = new Vector3(1, 1, 1);
        }
        else if (fireDir == new Vector2Int(-1, 0))
        {
            transform.GetChild(1).localScale = new Vector3(-1, 1, 1);
        }
        else if (fireDir == new Vector2Int(1, 0))
        {
            transform.GetChild(1).localScale = new Vector3(1, 1, 1);
        }


        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(1).position = new Vector3(Index.x, 0.5f, Index.y);

        GM.pewSource.Play();
        yield return new WaitForSeconds(0.05f);
        GM.pewSource.Stop();
        yield return new WaitForSeconds(0.05f);

        for (int i = 0; i < _dst; i++)
        {

            transform.GetChild(1).position += new Vector3(fireDir.x, 0, fireDir.y);
            yield return new WaitForSeconds(0.05f);
        }

        transform.GetChild(1).gameObject.SetActive(false);
  
        if (name == "enemy: 1")
        {

        }

        GM.Attack(_attackIndex, attack);
        yield return new WaitForSeconds(0.3f);
    }
}
