using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> healthUI = new List<Sprite>();
    [SerializeField]
    public Sprite[] moveDieFaces = new Sprite[6];
    [SerializeField]
    Sprite attackSprite = null;
    Sprite curFace;
    Sprite curHealth = null;
    int healthValue = 6;

    public IEnumerator updateFace(int _curFace)
    {
        yield return new WaitForSeconds(.2f);
        curFace = moveDieFaces[_curFace];
    }

    public IEnumerable updateHealth(int _Damage)
    {
        healthValue -= _Damage;
        yield return new WaitForSeconds(0.2f);
        curHealth = healthUI[healthValue];
    }
}
