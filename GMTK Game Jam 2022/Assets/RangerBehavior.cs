using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerBehavior :EnemyBehavior
{
    [SerializeField]
    int range = 0;
    bool readyAttack = false;

    protected override IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            if (!readyAttack)
            {
                if (Vector2Int.Distance(target, index) > 1)
                {
                    if (target.x == index.x || target.y == index.y)
                    {
                        //Attack
                        if (!readyAttack)
                        {
                            readyAttack = true;
                            yield return new WaitForSeconds(0.1f);
                        }

                    }

                    else
                    {

                    }
                }
                else
                {

                }
            }
            else
            {
                StartCoroutine(Attack());   
                yield return new WaitForSeconds(0.1f);
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

    IEnumerator Attack()
    {
        GM.RangedAttack(index, new Vector2Int(CollapseToOne(target.x, index.x), CollapseToOne(target.y, index.y)), range, attack);
        yield return new WaitForSeconds(0.1f);
    }
}
