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
    GameObject arrowPrefab = null;

    GameObject curArrowParent = null;
    GameManager GM;

    Vector2Int index = Vector2Int.zero;
    

    public void Init(GameManager _GM, Vector2Int _index)
    {
        GM = _GM;
        index = _index;
        LayoutMoveArrows();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray _ray = myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            Vector2Int _dir = Vector2Int.zero;
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
                        Move(_dir);
                    }
                    else if(_hit.transform.name[1] == 'a')
                    {

                    }
                }
            }

            
        }
        
    }

    public Vector2Int Index
    {
        get { return index; }
    }

    void Move(Vector2Int _moveDir)
    {
        if (GM.Move(index, _moveDir))
        {
            transform.position = transform.position + new Vector3(_moveDir.x, 0, _moveDir.y);
            index += _moveDir;
            LayoutMoveArrows();
        }
    }

    public void LayoutMoveArrows()
    {
        if (curArrowParent != null)
        {
            Destroy(curArrowParent);            
        }

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
            else if(_curCellState > 0)
            {
                GameObject _curArrow = Instantiate(arrowPrefab, new Vector3((index + _dirs[i]).x, 1, (index + _dirs[i]).y),
                    Quaternion.identity);
                _curArrow.name = _names[i] + "a";

                _curArrow.transform.parent = curArrowParent.transform;
            }
        }
        
    }
}
