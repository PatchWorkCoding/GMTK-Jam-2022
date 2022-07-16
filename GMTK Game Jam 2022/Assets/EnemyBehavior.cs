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
    // Start is called before the first frame update
    void Init(GameManager _GM)
    {
        GM = _GM;
        this.gameObject.tag = "Enemy";
    }

    private void Turn()
    {
        Movement();
        Attack();
        TurnOver();
    }
    private void TurnOver()
    {
        GM.nextTurn();
    }
    protected virtual IEnumerator Attack()
    {

        GM.Attack(target, attack);
        yield return new WaitForSeconds(0.4f);
        TurnOver();

    }
    protected virtual IEnumerator Movement()
    {

        //GM.enemydic(me).Index = ;
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(Attack());
    }
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (value == 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {

    }
}
