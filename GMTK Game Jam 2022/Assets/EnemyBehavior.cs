using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    GameManager GM = null;

    [SerializeField]
    int health = 0;

    // Start is called before the first frame update
    void Init(GameManager _GM)
    {
        GM = _GM;
    }

    // Update is called once per frame
    void Update()
    {
        
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
