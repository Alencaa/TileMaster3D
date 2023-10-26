using UnityEngine;
using UnityEngine.Playables;

public class ObjectPersistent : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }
}