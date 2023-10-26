using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
    public const string SPRITE_NAME = "TilesSprite2";
    public const string TILE_FREFAB = "Tile";
    public const string COMBO = "COMBO X";
    public const int TIME_COMBO = 6;
    public const int MAX_COMBO = 10;

    public enum ITEM_TYPE
    {
        ITEM_1 = 1,
        ITEM_2 = 2,
        ITEM_3 = 3,
        ITEM_4 = 4,
        ITEM_5 = 5,
        ITEM_6 = 6,
        ITEM_7 = 7,
        ITEM_8 = 8,
        ITEM_9 = 9,
        ITEM_10 = 10,
        ITEM_11 = 11,
        ITEM_12 = 12,
        ITEM_13 = 13,
        ITEM_14 = 14,
        ITEM_15 = 15,
        ITEM_16 = 16,
        ITEM_17 = 17,
        ITEM_18 = 18,
        ITEM_19 = 19,
        ITEM_20 = 20
    }
    #region CURR_LEVEL

    public static int currSelectLevel = 1;
    public const string CURR_LEVEL = "CURR_LEVEL";

    public static int currLevel = 1;

    public static void SetCurrLevel(int _currLevel)
    {

        currLevel = _currLevel;
        PlayerPrefs.SetInt(CURR_LEVEL, _currLevel);
        PlayerPrefs.Save();

    }
    public static void ClearPlayerPref()
    {
        currLevel = 1;
        PlayerPrefs.SetInt(CURR_LEVEL, 1);
        PlayerPrefs.Save();
    }
    public static void GetCurrLevel()
    {
        currLevel = PlayerPrefs.GetInt(CURR_LEVEL, 1);
    }
    #endregion
}
