using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{

    public List<TileItemData> TileData;
    public int Level;
    public string Name;
    public string DisplayName;
    public int PlayTime;
    public int MaxSlotCount;
    [System.Serializable]
    public class TileItemData
    {
        public TilesData TileItem;
        public int MatchNumber;
    }
}