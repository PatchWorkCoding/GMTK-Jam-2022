using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int width = 10, height = 10;

    int[,] board = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateBoard()
    {

    }

    public void Move()
    {

    }

    public void Attack(Vector2Int _index, int _damage)
    {

    }

    public void RangedAttack(Vector2Int _dir, int _length, int _damage)
    {

    }

    public void SpellAttack(Vector2Int _size, int _damage)
    {

    }

    int GetArrayIndex(Vector2Int _index)
    {
        if (_index.x <= board.GetLength(0) && _index.y < board.GetLength(1))
        {
            return board[_index.x, _index.y];
        }

        return 99;
    }
}
