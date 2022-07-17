using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBehavior : EnemyBehavior
{
    [SerializeField]
    int movesBetweenAttacks = 0;

    int movesSinceLastAttack = 0;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (movesSinceLastAttack < movesBetweenAttacks)
            {
                StartCoroutine(Attack());
                movesSinceLastAttack = 0;
                yield return new WaitForSeconds(1f);
                break;
            }
            else
            {
                StartCoroutine(Move());
                yield return new WaitForSeconds(0.5f);
            }
            
            curMoves++;
        }

        TurnOver();
    }

    protected override Vector2Int[] GeneratePossibleDirections()
    {
        List<Vector2Int> _returnDirs = new List<Vector2Int>();
        
        int _x = CollapseToOne(target.x, index.x);
        int _y = CollapseToOne(target.y, index.y);

        Vector2Int _prefferedDir = new Vector2Int(_x == 0 ? RandomNegativeOne() : _x, _y == 0 ? RandomNegativeOne() : _y);
        if (movesSinceLastAttack < movesBetweenAttacks)
        {
            _returnDirs.Add(new Vector2Int(-_prefferedDir.x, -_prefferedDir.y));
            _returnDirs.Add(new Vector2Int(_prefferedDir.x, -_prefferedDir.y));
            _returnDirs.Add(new Vector2Int(-_prefferedDir.x, _prefferedDir.y));
            _returnDirs.Add(_prefferedDir);
        }

        else
        {
            _returnDirs.Add(_prefferedDir);
            _returnDirs.Add(new Vector2Int(_prefferedDir.x, -_prefferedDir.y));
            _returnDirs.Add(new Vector2Int(-_prefferedDir.x, _prefferedDir.y));
            _returnDirs.Add(new Vector2Int(-_prefferedDir.x, -_prefferedDir.y));
        }

        movesSinceLastAttack++;
        return _returnDirs.ToArray();
        //return base.GeneratePossibleDirections();
    }

    IEnumerator Attack()
    {
        //print("did an attack");
        GM.BestowCurse(target);
        //GM.SpellAttack(index + new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y)), new Vector2Int(3,3), attack);
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

    int RandomNegativeOne()
    {
        int _rnd = Random.Range(0, 2);
        if (_rnd == 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
