using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> healthUI = new List<Sprite>();
    [SerializeField]
    public Sprite[] dieFaces = new Sprite[6];
    [SerializeField]
    Image attackDie = null;
    [SerializeField]
    Image healthDie = null;

    Sprite attackSprite = null;
    Sprite curFace;
    Sprite curHealth = null;
    //int healthValue = 6;

    public void UpdateMoveDie(int _face)
    {
        attackDie.transform.rotation = Quaternion.Euler(0, 0, 0);
        attackDie.sprite = dieFaces[_face];
    }

    public void UpdateTransitions(Vector2Int _dir)
    {
        attackDie.sprite = dieFaces[6];
        if (_dir == new Vector2Int(0, 1))
        {
            attackDie.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (_dir == new Vector2Int(0, -1))
        {
            attackDie.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (_dir == new Vector2Int(1, 0))
        {
            attackDie.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (_dir == new Vector2Int(-1, 0))
        {
            attackDie.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    public void UpdateHealth(int _face)
    {
        healthDie.sprite = dieFaces[_face];
        //healthValue -= _Damage;
        //yield return new WaitForSeconds(0.2f);
        //curHealth = healthUI[healthValue];
    }
}
