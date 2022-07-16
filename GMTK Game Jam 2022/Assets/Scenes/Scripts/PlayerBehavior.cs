using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField]
    Camera myCamera;
    [SerializeField]
    LayerMask moveButtonLayer;
    [SerializeField]
    GameObject arrowPrefab = null, dieTablePrefab = null;
    [SerializeField]
    int movesPerTurn = 2;
    [SerializeField]
    int health = 6;
    [SerializeField]
    UIManager uiManager;
    [Header("Sprite Properties")]
    [SerializeField]
    SpriteRenderer spriteRenderer = null;
    [SerializeField]
    Sprite defaultSprite, Walk1, Walk2, attackSprite;

    GameObject curArrowParent = null;
    GameManager GM;
    DieRoller roller = null;

    Vector2Int index = Vector2Int.zero;
    
    int moveCount = 0;

    bool canAct = false;

    public void Init(GameManager _GM, Vector2Int _index)
    {
        GM = _GM;

        uiManager = GM.UIManager;
        uiManager.UpdateHealth(5);
        uiManager.UpdateMoveDie(0);

        index = _index;
        canAct = false;

        roller = Instantiate(dieTablePrefab).GetComponent<DieRoller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canAct)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray _ray = myCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit;

                Vector2Int _dir = Vector2Int.zero;
                Vector3 _rollRot = Vector3.zero;
                if (Physics.Raycast(_ray, out _hit, moveButtonLayer))
                {
                    switch (_hit.transform.name[0])
                    {
                        case 'w':
                            _dir = new Vector2Int(0, 1);
                           
                            break;

                        case 's':
                            _dir = new Vector2Int(0, -1);
                            
                            break;

                        case 'a':
                            _dir = new Vector2Int(-1, 0);
                            break;

                        case 'd':
                            _dir = new Vector2Int(1, 0);
                            break;

                        default:
                            break;
                    }

                    if (_dir != Vector2Int.zero)
                    {
                        if (_hit.transform.name[1] == 'm')
                        {
                            
                            StartCoroutine(Move(_dir));
                            moveCount++;
                        }
                        else if (_hit.transform.name[1] == 'a')
                        {
                            StartCoroutine(Attack(_dir));
                            moveCount++;
                        }
                    }
                }
            }
        }
        
        
    }

    public void StartTurn()
    {
        canAct = true;
        moveCount = 0;
        LayoutMoveArrows();
    }

    public Vector2Int Index
    {
        get { return index; }
    }

    IEnumerator Attack(Vector2Int _dir)
    {
        if (curArrowParent != null)
        {
            Destroy(curArrowParent);
        }

        spriteRenderer.sprite = attackSprite;
        GM.Attack(index + _dir, roller.DieFace());

        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = defaultSprite;

        if (moveCount >= movesPerTurn)
        {
            canAct = false;
            if (curArrowParent != null)
            {
                Destroy(curArrowParent);
            }
            GM.ProgressTurn();
        }
        else
        {
            LayoutMoveArrows();
        }
    }

    public IEnumerator Move(Vector2Int _moveDir)
    {
        if (GM.Move(index, _moveDir))
        {
            if (curArrowParent != null)
            {
                Destroy(curArrowParent);
            }

            Vector3 _rollRot = Vector3.zero;
            if (_moveDir == new Vector2Int(0,1))
            {
                _rollRot = (new Vector3(90, 0, 0));
                spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_moveDir == new Vector2Int(0, -1))
            {
                _rollRot = (new Vector3(-90, 0, 0));
                spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (_moveDir == new Vector2Int(-1, 0))
            {
                _rollRot = (new Vector3(0, 0, 90));
                spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if(_moveDir == new Vector2Int(1,0))
            {
                _rollRot = (new Vector3(0, 0, -90));
                spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            }

            roller.RollDie(_rollRot);

            Vector3 _finalPos = transform.position + new Vector3(_moveDir.x, 0, _moveDir.y);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = Walk1;
            transform.position = transform.position + (new Vector3(_moveDir.x, 0, _moveDir.y).normalized * 0.3f);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = Walk2;
            transform.position = transform.position + (new Vector3(_moveDir.x, 0, _moveDir.y).normalized * 0.3f);

            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = defaultSprite;
            transform.position = _finalPos;

            index += _moveDir;
            uiManager.UpdateMoveDie(roller.DieFace() - 1);

            if (canAct)
            {
                if (moveCount >= movesPerTurn)
                {
                    canAct = false;
                    if (curArrowParent != null)
                    {
                        Destroy(curArrowParent);
                    }
                    GM.ProgressTurn();
                }
                else
                {
                    LayoutMoveArrows();
                }
            }
        }
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;
        uiManager.UpdateHealth(Mathf.Clamp(health - 1, 0, 5));

        if (health == 0)
        {
            GM.ResetGame();
            Destroy(roller.gameObject);
            Destroy(gameObject);
            //Debug.Log("Dead");
        }
    }

    public void LayoutMoveArrows()
    {
        curArrowParent = new GameObject();
        curArrowParent.name = "curArrows";

        Vector2Int[] _dirs = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        string[] _names = new string[] { "w", "s", "d", "a" };

        for (int i = 0; i < _dirs.Length; i++)
        {
            int _curCellState = GM.GetBoardCellState(index + _dirs[i]);
            if (_curCellState == 0)
            {
                GameObject _curArrow = Instantiate(arrowPrefab, new Vector3((index + _dirs[i]).x, 0, (index + _dirs[i]).y),
                    Quaternion.identity);
                _curArrow.name = _names[i] + "m";

                _curArrow.transform.parent = curArrowParent.transform;
            }
            else if(_curCellState > 0 && _curCellState < 99)
            {
                GameObject _curArrow = Instantiate(arrowPrefab, new Vector3((index + _dirs[i]).x, 1, (index + _dirs[i]).y),
                    Quaternion.identity);
                _curArrow.name = _names[i] + "a";

                _curArrow.transform.parent = curArrowParent.transform;
            }
        }
        
    }
}
