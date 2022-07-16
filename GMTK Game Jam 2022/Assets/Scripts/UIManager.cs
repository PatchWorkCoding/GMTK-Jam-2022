using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> healthUI = new List<Sprite>();
    [SerializeField]
    Sprite[] dieFaces = new Sprite[6];
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
        attackDie.sprite = dieFaces[_face];
    }

    public void UpdateHealth(int _face)
    {
        healthDie.sprite = dieFaces[_face];
        //healthValue -= _Damage;
        //yield return new WaitForSeconds(0.2f);
        //curHealth = healthUI[healthValue];
    }
}
