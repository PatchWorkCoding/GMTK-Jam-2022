using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyBehavior : MonoBehaviour
{
    protected GameManager GM = null;

    [SerializeField]
    int health = 0;
    [SerializeField]
    protected int speed = 0;
    [SerializeField]
    protected int attack = 0;
    [SerializeField]
    protected Vector2Int target = new Vector2Int(0, 0);
    [SerializeField]
    protected int maxMoves = 0;

    [SerializeField]
    int spawnIndex = 0;

    [Header("Sprite Properties")]
    [SerializeField]
    protected SpriteRenderer spriteRenderer = null;
    [SerializeField]
    protected Sprite defaultSprite, Walk1, Walk2, attackSprite;


    protected int curMoves = 0;

    protected Vector2Int index = Vector2Int.zero;

    // Start is called before the first frame update
    public void Init(GameManager _GM, Vector2Int _index)
    {
        GM = _GM;
        this.gameObject.tag = "Enemy";
        index = _index;
    }

    public void Turn()
    {
        curMoves = 0;
        FindTarget();
        StartCoroutine(RunBehaviorTree());
    }
    
    
    protected virtual IEnumerator RunBehaviorTree()
    {
        while (curMoves < maxMoves)
        {
            yield return new WaitForSeconds(1);
            curMoves++;
        }

        TurnOver();
    }


    protected virtual Vector2Int[] GeneratePossibleDirections()
    {
        return null;
    }

    protected IEnumerator Move()
    {
        Vector2Int[] _dirs = GeneratePossibleDirections();
        for (int i = 0; i < _dirs.Length; i++)
        {
            if (GM.Move(index, _dirs[i]))
            {
                Vector3 _finalPos = transform.position + new Vector3(_dirs[i].x, 0, _dirs[i].y);

                yield return new WaitForSeconds(0.1f);
                spriteRenderer.sprite = Walk1;
                transform.position = transform.position + (new Vector3(_dirs[i].x, 0, _dirs[i].y).normalized * 0.3f);

                yield return new WaitForSeconds(0.1f);
                spriteRenderer.sprite = Walk2;
                transform.position = transform.position + (new Vector3(_dirs[i].x, 0, _dirs[i].y).normalized * 0.3f);

                yield return new WaitForSeconds(0.1f);
                spriteRenderer.sprite = defaultSprite;
                transform.position = _finalPos;

                yield return new WaitForSeconds(0.1f);
                index += _dirs[i];
                break;
            }
        }
    }

    protected virtual void FindTarget()
    {
        target = GM.Player.Index;
    }

    /*
    protected virtual IEnumerator Movement()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(Attack());
    }
    */
    /*
    protected virtual IEnumerator Attack()
    {
        GM.Attack(target, attack);
        yield return new WaitForSeconds(0.4f);
        TurnOver();
    }
    */
    protected void TurnOver()
    {
        GM.ProgressTurn();
    }

    public int Health
    {
        get { return health; }

        set 
        { 
            health = value;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public int SpawnIndex
    {
        get { return spawnIndex; }
    }

    public Vector2Int Index
    {
        get { return index; }
    }

    public void Die()
    {
        GM.RemoveEnemy(this);
        Destroy(gameObject);
    }
}
