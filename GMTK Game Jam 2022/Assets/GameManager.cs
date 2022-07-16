using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
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
    [SerializeField]
    List<GameObject> enemyPrefabs = null;

    [Header("Save Properties")]
    [SerializeField]
    string path = "DEFAULT";

    PlayerBehavior curPlayer = null;

    Dictionary<Vector2Int, EnemyBehavior> enemyDic = null;

    int[,] board = null;

    // Start is called before the first frame update
    void Awake()
    {
        board = new int[width, height];

        PopulateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateBoard()
    {
        string _loadPath = Application.dataPath + "/" + path + ".level";

        BinaryFormatter _formatter = new BinaryFormatter();
        FileStream _stream = new FileStream(_loadPath, FileMode.Open);

        BoardSave _saveData = _formatter.Deserialize(_stream) as BoardSave;

        _stream.Close();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                board[x, y] = _saveData.BoardStates[x][y];
                if (_saveData.BoardStates[x][y] > 0)
                {
                    if (_saveData.BoardStates[x][y] == 69)
                    {
                        curPlayer = Instantiate(playerPrefab, new Vector3(x, 0, y), Quaternion.identity).GetComponent<PlayerBehavior>();
                        curPlayer.Init(this, new Vector2Int(x, y));
                    }

                    else
                    {
                        print("called");
                        Instantiate(enemyPrefabs[_saveData.BoardStates[x][y] - 1], new Vector3(x, 0, y), Quaternion.identity);
                    }
                }
                
            }
        }
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

    public void BakeBoard()
    {
        int[][] _boardStates = new int[width][];
        for (int x = 0; x < width; x++)
        {
            _boardStates[x] = new int[height];
            for (int y = 0; y < height; y++)
            {

                
                RaycastHit _hit;
                if (Physics.Raycast(new Vector3(x, 100, y), Vector3.down, out _hit))
                {
                    switch (_hit.transform.tag)
                    {
                        case "Player":
                            _boardStates[x][y] = 69;
                            DestroyImmediate(_hit.transform.gameObject);
                            break;

                        case "Enemy":
                            _boardStates[x][y] = _hit.transform.GetComponent<EnemyBehavior>().SpawnIndex + 1;
                            print(_hit.transform.GetComponent<EnemyBehavior>().SpawnIndex + 1);
                            DestroyImmediate(_hit.transform.gameObject);
                            break;

                        case "Immoveable":
                            _boardStates[x][y] = -1;
                            break;

                        default:
                            _boardStates[x][y] = 0;
                            break;
                    }
                }

                else
                {
                    _boardStates[x][y] = -1;
                }
            }
        }

        string _savePath = Application.dataPath + "/" + path + ".level";
        BinaryFormatter _fromatter = new BinaryFormatter();
        FileMode _fileMode = FileMode.Create;

        FileStream _stream = new FileStream(_savePath, _fileMode);

        _fromatter.Serialize(_stream, new BoardSave(_boardStates));

        _stream.Close();
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, width));
        }

        for (int y = 0; y < height; y++)
        {
            Gizmos.DrawLine(new Vector3(0, 0, y), new Vector3(height, 0, y));
        }
    }

    public PlayerBehavior Player
    {
        get { return curPlayer; }
    }

    public void nextTurn()
    {

    }
}

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Bake Map"))
        {
            (target as GameManager).BakeBoard();
        }
    }
}

[System.Serializable]
public class BoardSave
{
    int[][] m_boardStates;

    public BoardSave(int[][] _boardStates)
    {
        m_boardStates = _boardStates;
    }

    public int[][] BoardStates
    {
        get { return m_boardStates; }
    }
}
