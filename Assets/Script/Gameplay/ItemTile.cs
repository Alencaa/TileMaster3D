using DG.Tweening;
using UnityEngine;

public class ItemTile : MonoBehaviour
{
    public static float TILE_SIZE = 2f;
    public static Vector3 TILE_VOLUME = new Vector3(1.7f, 0.3f, 2.8f);
    [SerializeField] private SpriteRenderer icon = null;
    [SerializeField] private GameObject outline = null;
    private TilesData tileData;
    private bool canRotate = true;
    private float minRotateAngle = -45f;
    private float maxRotateAngle = 45f;
    private Rigidbody rb;
    private BoxCollider touchCollider;
    private Vector3 oldSize;
    private Vector3 oldPos;


    private void Start()
    {
        Vector3 upDirection = transform.up;
        oldSize = transform.localScale;
        transform.rotation = Quaternion.FromToRotation(upDirection, Vector3.up) * transform.rotation;
        rb = GetComponent<Rigidbody>();
        touchCollider = GetComponent<BoxCollider>();

    }
    public TilesData TileData
    {
        get { return tileData; }
    }
    public void InitTile(TilesData _tileData)
    {
        tileData = _tileData;
        int tileType = (int)tileData.itemType;
        icon.sprite = GameUtils.LoadSprite(Config.SPRITE_NAME, tileType.ToString());
    }
    private void Update()
    {
        RotateTileX();
        RotateTileZ();
    }
    private float WrapAngle(float angle)
    {
        if (angle <= 180)
        {
            return angle;
        }
        else
        {
            return angle - 360f;
        }
    }
    private void RotateTileX()
    {
        if (!canRotate)
        {
            return;
        }
        float eulerZ = transform.eulerAngles.z;
        float eulerX = WrapAngle(transform.eulerAngles.x);
        float eulerY = transform.eulerAngles.y;
        if (eulerX < minRotateAngle || eulerX > maxRotateAngle)
        {
            float randomAngleX = Random.Range(-40, 40);
            transform.DORotateQuaternion(Quaternion.Euler(randomAngleX, eulerY, eulerZ), 0.4f);
            return;
        }
        else return;
        //RotateTileUpward();
    }
    private void RotateTileZ()
    {
        if (!canRotate)
        {
            return;
        }
        float eulerX = transform.eulerAngles.x;
        float eulerZ = WrapAngle(transform.eulerAngles.z);
        float eulerY = transform.eulerAngles.y;
        if (eulerZ < minRotateAngle || eulerZ > maxRotateAngle)
        {
            float randomAngleZ = Random.Range(-40, 40);
            transform.DORotateQuaternion(Quaternion.Euler(eulerX, eulerY, randomAngleZ), 0.4f);
            return;
        }
        else return;
    }
    private void OnMouseDown()
    {
        deactivateTile();
        SoundHelper.PlayEffect("clickTile");
        GameLevelManager.Instance.MoveTileToSlot(this);
    }
    private void deactivateTile()
    {
        canRotate = false;
        touchCollider.enabled = false;
        if (rb != null) rb.isKinematic = true;
    }
    private void activateTile()
    {
        transform.parent = GameLevelManager.Instance.TileOnBoardTrans;
        canRotate = true;
        touchCollider.enabled = true;
        if (rb != null) rb.isKinematic = false;
        transform.DOLocalMove(oldPos, 0.3f).SetEase(Ease.OutQuad);
        transform.DOScale(oldSize, 0.3f).SetEase(Ease.OutQuad);

    }
    private void OnMouseOver()
    {
        outline.SetActive(true);
    }
    private void OnMouseExit()
    {
        outline.SetActive(false);
    }
    Sequence moveToSlot_Sequence;
    public void MoveTileToSlot(int index)
    {
        oldPos = transform.position;
        moveToSlot_Sequence = DOTween.Sequence();
        moveToSlot_Sequence.Insert(0f,transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.4f));
        moveToSlot_Sequence.Insert(0f,transform.DOScale(TILE_VOLUME, 0.15f).OnComplete(() =>
        {
            SoundHelper.PlayEffect("drop");
        }));
        moveToSlot_Sequence.Insert(0f,transform.DOLocalMove(new Vector3(index * TILE_SIZE, 0, 0), 0.3f));
        moveToSlot_Sequence.OnComplete(() =>
        {
            GameLevelManager.Instance.OnItemSlotMoveFinished(this);
        });
    }
    Sequence moveResetPosSlot_Sequence;

    public void ResetPosSlot(int index, System.Action callback = null)
    {
        moveResetPosSlot_Sequence = DOTween.Sequence();
        SoundHelper.PlayEffect("slide");
        moveResetPosSlot_Sequence.Insert(0f, transform.DOLocalMove(new Vector3(index * TILE_SIZE, 0, 0), 0.3f));
        moveResetPosSlot_Sequence.OnComplete(() =>
        {
            callback?.Invoke();
        });
    }
    public void OnMatch3Complete()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.parent = GameLevelManager.Instance.DumpSlot;
            gameObject.SetActive(false);
        });
    }
    public void SetItemTileUndo()
    {
        activateTile();
    }
}



