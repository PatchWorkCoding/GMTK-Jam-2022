using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Map Properties")]
    [SerializeField]
    int width = 10;
    [SerializeField]
    int height = 10;

    [Header("Piece Prefabs")]
    [SerializeField]
    GameObject playerPrefab = null;

    PlayerBehavior curPlayer = null;

    Dictionary<Vector2Int, EnemyBehavior> enemyDic = null;

    int[,] board = null;

    // Start is called before the first frame update
    void Awake()
    {
        board = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                board[x, y] = 0;
            }
        }

        PopulateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateBoard()
    {
        curPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerBehavior>();
        curPlayer.Init(this);
    }

    public bool Move(Vector2Int _curIndex, Vector2Int _dir)
    {
        int _boardCellState = GetBoardCellState(_curIndex + _dir);

        if (_boardCellState == 0)
        {
            SwapStates(_curIndex, _curIndex + _dir);
            return true;
        }

        return false;
    }

    public void Attack(Vector2Int _index, int _damage)
    {
        if (enemyDic.ContainsKey(_index))
        {
            int _state = enemyDic[_index].Health - _damage;

            enemyDic[_index].Health -= _damage;
            SetBoardCellState(_index, Mathf.Clamp(_state, 0, 100));
        }
        
        else if (curPlayer.Index == _index)
        {
            //curPlayer.health
        }
    }

    public void RangedAttack(Vector2Int _dir, int _length, int _damage)
    {

    }

    public void SpellAttack(Vector2Int _size, int _damage)
    {

    }

    public int GetBoardCellState(Vector2Int _index)
    {
        if ((_index.x < board.GetLength(0) && _index.y < board.GetLength(1)) && (_index.y >= 0 && _index.x >= 0))
        {
            return board[_index.x, _index.y];
        }

        return 99;
    }

    public void SetBoardCellState(Vector2Int _index, int _state)
    {
        if ((_index.x < board.GetLength(0) && _index.y < board.GetLength(1)) && (_index.y >= 0 && _index.x >= 0))
        {
            board[_index.x, _index.y] = _state;
        }
    }

    void SwapStates(Vector2Int _a, Vector2Int _b)
    {
        int _j = board[_a.x, _a.y];
        board[_a.x, _a.y] = board[_b.x, _b.y];
        board[_b.x, _b.y] = _j;
    }
}
