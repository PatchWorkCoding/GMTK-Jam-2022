using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyBehavior : MonoBehaviour
{
    GameManager GM = null;

    [SerializeField]
    int health = 0;
    [SerializeField]
    protected int speed = 0;
    [SerializeField]
    int attack = 0;
    [SerializeField]
    protected Vector2Int target = new Vector2Int(0, 0);
    [SerializeField]
    int range = 0;

    [SerializeField]
    int spawnIndex = 0;

    Vector2Int index = Vector2Int.zero;

    // Start is called before the first frame update
    public void Init(GameManager _GM, Vector2Int _index)
    {
        GM = _GM;
        this.gameObject.tag = "Enemy";

        index = _index;
    }

    public void Turn()
    {
        //Movement();
        //Attack();
        print("Did a turn");
        TurnOver();
    }
    private void TurnOver()
    {
        GM.ProgressTurn();
    }
    protected virtual IEnumerator Attack()
    {

        GM.Attack(target, attack);
        yield return new WaitForSeconds(0.4f);
        TurnOver();

    }
    protected virtual IEnumerator Movement()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(Attack());
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
