using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static Sprite LoadSprite(string path, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>("Sprite/" + path);
        foreach (var sprite in all)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }
        return null;
    }
    public static IEnumerator DelayFunction(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
