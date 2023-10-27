using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Xml.Schema;
//using UnityEngine.UIElements;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;
    [Header("SelectLevel")]
    public int level = 1;
    [Header("LevelData")]
    public LevelData LevelData;
    private List<TilesData> tileDataType = new List<TilesData>();
    private Dictionary<TilesData, int> tileInitData = new Dictionary<TilesData, int>();
    [Header("LevelDetail")]
    private string displayName;
    private int playTime;
    private int maxSlotCount;
    private GameObject itemTile;
    private Transform spawnPos;
    [SerializeField] private Collider boardCollider;
    [SerializeField] private Transform posSlot;
    [SerializeField] private Transform dumpSlot;
    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI txtTimer = null;
    [SerializeField] private TextMeshProUGUI txtScore = null;
    [SerializeField] private TextMeshProUGUI txtName = null;
    [SerializeField] private TextMeshProUGUI txtCombo = null;
    [SerializeField] private Slider comboMeter = null;
    [SerializeField] private Button btnUndo = null;
    private Coroutine clockCoroutine;

    private void Awake()
    {
        Instance = this;
        Camera mainCamera = Camera.main;

        Canvas canvas = GetComponentInChildren<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = mainCamera;
    }
    private void StartClock()
    {
        if (clockCoroutine != null)
        {
            StopCoroutine(clockCoroutine);
        }
        clockCoroutine = StartCoroutine(ClockCoroutine());
    }

    private IEnumerator ClockCoroutine()
    {
        int remainingSeconds = playTime;

        while (remainingSeconds > 0)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(remainingSeconds);
            string clockString = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            txtTimer.text = clockString;

            yield return new WaitForSeconds(1f);

            remainingSeconds--;
        }

        // Clock has finished, do something
        Debug.Log("Clock finished!");
        SetGameLose();
    }
    private void Start()
    {
        scorePoint = 0;
        spawnPos = new GameObject("SpawnPos").transform;
        spawnPos.parent = this.transform;
        Debug.Log("Prefab/" + Config.TILE_FREFAB);
        itemTile = Resources.Load<GameObject>("Prefab/" + Config.TILE_FREFAB);
        btnUndo.onClick.AddListener(SetUndo);
        initLevel();
        StartClock();
    }
    public Transform DumpSlot
    {
        get { return dumpSlot; }
        set { dumpSlot = value;}
    }
    public Transform TileOnBoardTrans
    {
        get { return spawnPos; }
        set { spawnPos = value; }
    }
    private void initLevel()
    {
        txtScore.text = scorePoint.ToString();
        foreach (LevelData.TileItemData tile in LevelData.TileData)
        {
            tileDataType.Add(tile.TileItem);
            tileInitData.Add(tile.TileItem, tile.MatchNumber);
        }
        spawnTile(tileInitData);
        name = LevelData.Name;
        displayName = LevelData.DisplayName;
        playTime = LevelData.PlayTime;
        maxSlotCount = LevelData.MaxSlotCount;
        txtName.text = displayName;
    }
    private void spawnTile(Dictionary<TilesData, int> data) 
    {
        int z = 0;
        foreach (var item in data.Keys)
        {
            
            if (data.ContainsKey(item))
            {
                int matchCount = data[item];
                int itemType = (int)item.itemType;
                for (int i = 0; i < 3 * matchCount; i++)
                {
                    z++;
                    ItemTile tile = Instantiate(itemTile, spawnPos).GetComponentInChildren<ItemTile>();
                    tile.name = z.ToString();
                    setRandomSpawnPoint(tile.gameObject);
                    tile.InitTile(item);
                }
            }
        }
    }
    private void setRandomSpawnPoint(GameObject tile)
    {
        Bounds bounds = boardCollider.bounds;
        float offsetX = Random.Range(-bounds.extents.x + 1f, bounds.extents.x - 1f);
        float offsetY = Random.Range(tile.transform.position.y - 3, tile.transform.position.y + 3);
        float offsetZ = Random.Range(-bounds.extents.z + 1f, bounds.extents.z - 1f);
        float offsetRotate = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0, offsetRotate, 0);
        tile.transform.rotation = randomRotation;
        tile.transform.position = bounds.center + new Vector3(offsetX, offsetY, offsetZ);
    }
    #region SLOT
    [Header("SLOT")]
    [SerializeField] private List<ItemTile> listTilesSlot = new List<ItemTile>();
    [SerializeField] private List<ItemTile> listTilesSlotMatch = new List<ItemTile>();
    public void MoveTileToSlot(ItemTile _itemTile)
    {
        if (listTilesSlot.Count < maxSlotCount)
        {
            _itemTile.transform.parent = posSlot;
            int slotIndex = FindIndexAddItemTileSlot(_itemTile);
            _itemTile.MoveTileToSlot(slotIndex);
            listCheckUndo_ItemTileSlots.Add(_itemTile);
            listTilesSlot.Insert(slotIndex, _itemTile);
            StartCoroutine(SetListItemSlot_ResetPosition(0.01f));
        }
    }
    public int FindIndexAddItemTileSlot(ItemTile itemTile)
    {
        int indexSlot = listTilesSlot.Count;
        for (int i = listTilesSlot.Count - 1; i >= 0; i--)
        {
            if (listTilesSlot[i].TileData.itemType == itemTile.TileData.itemType)
            {
                return i + 1;
            }
        }
        return indexSlot;
    }
    public IEnumerator SetListItemSlot_ResetPosition(float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < listTilesSlot.Count; i++)
        {
            listTilesSlot[i].ResetPosSlot(i);
        }
    }
    public void AddListMatch3(ItemTile _itemTile)
    {
        listTilesSlotMatch.Add(_itemTile);
    }
    public void OnItemSlotMoveFinished(ItemTile _itemTile)
    {
        List<ItemTile> listCheckMatch3 = FindMatch3_ItemTile_Slots(_itemTile);
        
        if (listCheckMatch3.Count == 3) 
        {
            for (int i = 0; i < listCheckMatch3.Count; i++)
            {
                listTilesSlot.Remove(listCheckMatch3[i]);
                listCheckUndo_ItemTileSlots.Remove(listCheckMatch3[i]);
            }
            ItemTile tile1 = listCheckMatch3[0];
            ItemTile tile2 = listCheckMatch3[1];
            ItemTile tile3 = listCheckMatch3[2];
            SoundHelper.PlayEffect("collected");
            Sequence mergeSequence = DOTween.Sequence();
            mergeSequence.Insert(0,tile1.transform.DOMoveX(tile2.transform.position.x, 0.2f).SetEase(Ease.OutBack));
            mergeSequence.Insert(0, tile3.transform.DOMoveX(tile2.transform.position.x, 0.2f).SetEase(Ease.OutBack));
            mergeSequence.OnComplete(() =>
            {
                for (int i = 0; i < listCheckMatch3.Count; i++)
                {
                    listCheckMatch3[i].OnMatch3Complete();
                }
                raiseCombo();
                StarController.instance.AddStar(tile1.transform.position, txtScore.transform.position, currentCombo, changeScore);
            });
            
            StartCoroutine(SetListItemSlot_ResetPosition(0.3f));
            StartCoroutine(CheckGameWin());
        }
        else
        {
            StartCoroutine(CheckGameOver());
        }
    }
    public List<ItemTile> FindMatch3_ItemTile_Slots(ItemTile _itemTile)
    {
        List<ItemTile> listCheckMatch3_ItemTileSlot = new List<ItemTile>();
        for (int i = 0; i < listTilesSlot.Count; i++)
        {
            if (listTilesSlot[i].TileData.itemType == _itemTile.TileData.itemType)
            {
                listCheckMatch3_ItemTileSlot.Add(listTilesSlot[i]);

                if (listCheckMatch3_ItemTileSlot.Count == 3)
                {
                    return listCheckMatch3_ItemTileSlot;
                }
            }
        }

        return listCheckMatch3_ItemTileSlot;
    }
    #endregion
    #region UNDO
    [Header("UNDO")]
    public List<ItemTile> listCheckUndo_ItemTileSlots = new List<ItemTile>();

    public bool CheckUndoAvaiable()
    {
        return listCheckUndo_ItemTileSlots.Count > 0;
    }
    public void SetUndo()
    {
        if (CheckUndoAvaiable())
        {
            ItemTile itemTileSlot_Undo = listCheckUndo_ItemTileSlots[listCheckUndo_ItemTileSlots.Count - 1];
            itemTileSlot_Undo.SetItemTileUndo();
            listCheckUndo_ItemTileSlots.Remove(itemTileSlot_Undo);
            listTilesSlot.Remove(itemTileSlot_Undo);
            StartCoroutine(SetListItemSlot_ResetPosition(0.01f));
        }
    }
    #endregion
    #region GAME_POINT
    private int scorePoint = 0;
    
    private void changeScore()
    {
        scorePoint++;
        txtScore.text = scorePoint.ToString();
    }
    #endregion
    #region GAME_CONDITION
    public bool CheckGameFinish()
    {
        if (spawnPos.childCount > 0)
        {
            return false;
        }
        return true;
    }
    public IEnumerator CheckGameWin()
    {
        bool isWon = CheckGameFinish();
        if (isWon)
        {
            Debug.Log("playSound");
        }
        yield return new WaitForSeconds(0.5f);
        if (isWon)
        {
            SetGameWin();
        }
    }
    public IEnumerator CheckGameOver()
    {
        bool isLost = CheckGameLose();
        if (isLost)
        {
            Debug.Log("playSound");
        }
        yield return new WaitForSeconds(0.5f);
        if (isLost)
        {
            SetGameLose();
        }
    }
    public bool CheckGameLose()
    {
        if (listTilesSlot.Count < maxSlotCount)
        {
            return false;
        }
        return true;
    }
    public void SetGameWin()
    {
        Debug.Log("WIN");
        Config.SetCurrLevel(level + 1);
        Config.currSelectLevel = Config.currLevel;
        GameSceneController.instance.ShowWinGame(scorePoint);
    }
    public void SetGameLose()
    {
        Debug.Log("LOSE");
        Config.SetCurrLevel(level);
        Config.currSelectLevel = Config.currLevel;
        GameSceneController.instance.ShowLoseGame();
    }
    #endregion
    #region COMBO_METER
    //raise combo
    private int currentCombo = 0;
    private Coroutine changeSliderCoroutine;

    private void raiseCombo()
    {
        comboMeter.gameObject.SetActive(true);
        currentCombo++;
        currentCombo = Mathf.Clamp(currentCombo, 0, Config.MAX_COMBO);
        txtCombo.text = Config.COMBO + currentCombo;
        if (changeSliderCoroutine != null)
        {
            StopCoroutine(changeSliderCoroutine);
        }
        changeSliderCoroutine = StartCoroutine(changeSliderValue(comboMeter.maxValue, 0));
    }
    private void resetCombo()
    {
        currentCombo = 0;
        comboMeter.gameObject.SetActive(false);
    }
    private IEnumerator changeSliderValue(float startValue, float targetValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < Config.TIME_COMBO)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / Config.TIME_COMBO);

            float newValue = Mathf.Lerp(startValue, targetValue, t);
            comboMeter.value = newValue;

            yield return null;
        }

        // Ensure the final value is set accurately
        comboMeter.value = targetValue;
        if (comboMeter.value == comboMeter.minValue)
        {
            resetCombo();
        }
    }
    #endregion
}
