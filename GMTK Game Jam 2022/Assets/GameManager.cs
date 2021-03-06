using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM = null;

    [Header("Map Properties")]
    [SerializeField]
    int width = 10;
    [SerializeField]
    int height = 10;


    [Header("Piece Prefabs")]
    [SerializeField]
    GameObject playerPrefab = null;
    [SerializeField]
    GameObject firePrefab = null;
    [SerializeField]
    List<GameObject> enemyPrefabs = null;

    [Header("Save Properties")]
    [SerializeField]
    string path = "DEFAULT";

    [Header("UIStuff")]
    [SerializeField]
    UIManager UI = null;
    PlayerBehavior curPlayer = null;
    [SerializeField]
    public AudioSource beepSource = null, attackSource = null, 
        deathSource = null, enemyWalk = null, pewSource = null, hissSource = null;
    [SerializeField]
    public Sprite deathSprite1 = null, deathSprite2 = null;
    [SerializeField]
    GameObject transitionScreen = null;

    List<EnemyBehavior> curEnemies = null;
    List<GameObject> fireSprites = null;

    int curLevelIndex = 0;

    int turnOrderIndex = -1;
    //Dictionary<Vector2Int, EnemyBehavior> enemyDic = null;

    int[,] board = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (GM != null)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            GM = this;
            DontDestroyOnLoad(gameObject);
            curLevelIndex = 1;
            //path = "Level" + (curLevelIndex + 1);
            curLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.sceneLoaded += OnSceneLoaded;
            //PopulateBoard(path);
            //ProgressTurn();
        }

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PopulateBoard()
    {
        board = new int[width, height];
        curEnemies = new List<EnemyBehavior>();
        fireSprites = new List<GameObject>();
        turnOrderIndex = -2;

        
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
                        EnemyBehavior _enemy = Instantiate(enemyPrefabs[_saveData.BoardStates[x][y] - 1], new Vector3(x, 0, y),
                            Quaternion.identity).GetComponent<EnemyBehavior>();

                        _enemy.Init(this, new Vector2Int(x, y));
                        _enemy.gameObject.name = "enemy: " + curEnemies.Count;

                        curEnemies.Add(_enemy);
                    }
                }

            }
        }
    }

    public void PopulateBoard(string _populateBoard)
    {
        board = new int[width, height];
        curEnemies = new List<EnemyBehavior>();
        fireSprites = new List<GameObject>();
        turnOrderIndex = -2;

        string _loadPath = Application.persistentDataPath + "/" + path + ".level";

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
                        EnemyBehavior _enemy = Instantiate(enemyPrefabs[_saveData.BoardStates[x][y] - 1], new Vector3(x, 0, y),
                            Quaternion.identity).GetComponent<EnemyBehavior>();

                        _enemy.Init(this, new Vector2Int(x, y));
                        _enemy.gameObject.name = "enemy: " + curEnemies.Count;

                        curEnemies.Add(_enemy);
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
        /*
        else if (_boardCellState == 15)
        {
            SwapStates(_curIndex, _curIndex + _dir);
            return true;
        }
        */
        return false;
    }

    public bool Attack(Vector2Int _index, int _damage)
    {
        if (curPlayer.Index == _index)
        {
            Player.TakeDamage(_damage);
            return false;
        }
        else
        {
            for (int i = 0; i < curEnemies.Count; i++)
            {
                if (curEnemies[i].Index == _index)
                {
                    //int _state = curEnemies[i].Health - _damage;
                    curEnemies[i].Health -= _damage;

                    return curEnemies[i].Health <= 0;
                }
            }
        }
        return false;
    }

    public Vector2Int RangedAttack(Vector2Int _index, Vector2Int _dir, int _length, int _damage, out int _returnDst)
    {
        _returnDst = _length;
        Vector2Int _curIndex = _index;
        for (int i = 0; i < _length; i++)
        {
            _curIndex += _dir;
            if (GetBoardCellState(_curIndex) != 99 && GetBoardCellState(_curIndex) != -1)
            {
                if (curPlayer.Index == _curIndex)
                {
                    _returnDst = i;
                    return _curIndex;
                }

                else
                {
                    for (int n = 0; n < curEnemies.Count; n++)
                    {
                        if (curEnemies[n].Index == _curIndex)
                        {
                            int _state = curEnemies[n].Health - _damage;
                            curEnemies[n].Health -= _damage;
                            SetBoardCellState(_index, Mathf.Clamp(_state, 0, 100));
                            _returnDst = i;
                            return _curIndex;
                        }
                    }
                }
            }

            else
            {
                _returnDst = i;
                return _curIndex;
            }
        }

        return _curIndex;
    }

    public void SpellAttack(Vector2Int _index, Vector2Int _size, int _damage)
    {
        for (int x = -Mathf.FloorToInt(_size.x / 2); x < _size.x; x++)
        {
            for (int y = -Mathf.FloorToInt(_size.x / 2); y < _size.y; y++)
            {
                Vector2Int _curIndex = new Vector2Int(_index.x + x, _index.y + y);
                if (GetBoardCellState(_curIndex) != 99 && GetBoardCellState(_curIndex) >= 0)
                {
                    SetBoardCellState(_index, 15);
                }
            }
        }
    }

    public void BestowCurse(Vector2Int _index)
    {
        if (Player.Index == _index)
        {
            Player.Curse();
        }
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
            if (_state == 15)
            {
                print("called");
                fireSprites.Add(Instantiate(firePrefab, new Vector3(_index.x, 0, _index.y), firePrefab.transform.rotation));
            }
            board[_index.x, _index.y] = _state;
        }
    }

    void SwapStates(Vector2Int _a, Vector2Int _b)
    {

        int _j = board[_a.x, _a.y];
        if (board[_a.x, _a.y] == 15)
        {
            board[_a.x, _a.y] = 0;
        }
        else
        {
            board[_a.x, _a.y] = board[_b.x, _b.y];
        }
        board[_b.x, _b.y] = _j;
    }


    public void ResetGame()
    {
        for (int i = 0; i < curEnemies.Count; i++)
        {
            Destroy(curEnemies[i].gameObject);
        }
        curEnemies.Clear();
        curEnemies.TrimExcess();

        PopulateBoard(path);
        ProgressTurn();
    }

    public void RemoveEnemy(EnemyBehavior _enemy)
    {
        board[_enemy.Index.x, _enemy.Index.y] = 0;
        curEnemies.Remove(_enemy);
        curEnemies.TrimExcess();
    }

    public void InitializeBoard()
    {
        board = new int[width, height];
        curEnemies = new List<EnemyBehavior>();
        fireSprites = new List<GameObject>();
        turnOrderIndex = -2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                RaycastHit _hit;
                if (Physics.Raycast(new Vector3(x, 100, y), Vector3.down, out _hit))
                {
                    switch (_hit.transform.tag)
                    {
                        case "Player":
                            board[x,y] = 69;
                            //Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.blue, 10);
                            curPlayer = _hit.transform.gameObject.GetComponent<PlayerBehavior>();
                            curPlayer.Init(this, new Vector2Int(x, y));
                            //DestroyImmediate(_hit.transform.gameObject);
                            break;

                        case "Enemy":
                            board[x, y] = _hit.transform.GetComponent<EnemyBehavior>().SpawnIndex + 1;

                            EnemyBehavior _enemy = _hit.transform.GetComponent<EnemyBehavior>();

                            _enemy.Init(this, new Vector2Int(x, y));
                            _enemy.gameObject.name = "enemy: " + curEnemies.Count;

                            curEnemies.Add(_enemy);
                            break;

                        case "Immoveable":
                            board[x, y] = -1;
                            //Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.black, 10);
                            break;

                        default:
                            //Debug.Log("called");
                            //Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.white, 10);
                            board[x, y] = 0;
                            break;
                    }
                }

                else
                {
                    //Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.black, 10);
                    board[x, y] = -1;
                }
            }
        }
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
                            Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.blue, 10);
                            DestroyImmediate(_hit.transform.gameObject);
                            break;

                        case "Enemy":
                            _boardStates[x][y] = _hit.transform.GetComponent<EnemyBehavior>().SpawnIndex + 1;
                            Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.red, 10);
                            DestroyImmediate(_hit.transform.gameObject);
                            break;

                        case "Immoveable":
                            _boardStates[x][y] = -1;
                            Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.black, 10);
                            break;

                        default:
                            //Debug.Log("called");
                            Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.white, 10);
                            _boardStates[x][y] = 0;
                            break;
                    }
                }

                else
                {
                    Debug.DrawRay(new Vector3(x, 100, y), Vector3.down * 101f, Color.black, 10);
                    _boardStates[x][y] = -1;
                }
            }
        }

        string _savePath = Application.persistentDataPath + "/" + path + ".level";
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
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, width - 1));
        }

        for (int y = 0; y < height; y++)
        {
            Gizmos.DrawLine(new Vector3(0, 0, y), new Vector3(height - 1, 0, y));
        }
    }

    public PlayerBehavior Player
    {
        get { return curPlayer; }
    }

    public UIManager UIManager
    {
        get { return UI; }
    }

    public void ProgressTurn()
    {
        turnOrderIndex++;

        if (turnOrderIndex >= curEnemies.Count)
        {
            turnOrderIndex = -1;
            RemoveFire();
        }

        if (turnOrderIndex == -1)
        {
            Player.StartTurn();
        }

        else if (turnOrderIndex < curEnemies.Count)
        {
            curEnemies[turnOrderIndex].Turn();
        }

        if (curEnemies.Count <= 0)
        {
            curLevelIndex++;
            StartCoroutine(LoadNextLevel());
        }
    }

    public void StartBeep()
    {
        beepSource.Play();
    }

    public void StopBeep()
    {
        beepSource.Stop();
    }

    public void StartAttackBeep()
    {
        attackSource.Play();
    }

    public void StopAttackBeep()
    {
        attackSource.Stop();
    }

    void RemoveFire()
    {
        if (fireSprites.Count > 0)
        {
            for (int i = 0; i < fireSprites.Count; i++)
            {
                Destroy(fireSprites[i]);
            }
            fireSprites.TrimExcess();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == 15)
                {
                    board[x, y] = 0;
                }
            }
        }
    }

    IEnumerator LoadNextLevel()
    {
        transitionScreen.SetActive(true);
        path = "Level" + (curLevelIndex + 1);

        //curLevelIndex++;
        SceneManager.LoadScene(curLevelIndex, LoadSceneMode.Single);


        yield return new WaitForSeconds(1);

        transitionScreen.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 7)
        {
            InitializeBoard();
            ProgressTurn();
        }
        else
        {
            curLevelIndex = 1;
        }
    }
    //on
}

/*
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

   
        if (GUILayout.Button("Load Map"))
        {
            (target as GameManager).PopulateBoard();
        }
        
    }
}
*/

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
