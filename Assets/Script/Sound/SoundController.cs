using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundController : MonoBehaviour {
    public static SoundController instance;
    public static bool LOCK_CHANGE_SOUND_MUSIC = false;
    [SerializeField] private AudioSource soundMusic = null;

    [SerializeField] private AudioSource soundEffect = null;

    [SerializeField] private List<AudioSource> listSoundEffect;

    [SerializeField]
    private Dictionary<string, AudioClip> dicSound;

    public int StatusSoundMusic = 0;//0 mute, 1 sound
    public int StatusSFX = 0;//0 mute, 1 sound
    public const float MAX_VOL = 0.2f;

    private Tweener tweenerMute;

    private AudioClip currentMusic;

    [SerializeField] private List<AudioClip> audioClips = null;

    private System.Action callbackEnd;


    public System.Action<int> HandleChangeSoundState;
    // Use this for initialization
    int timeCountdown;
    int cacheStatus = 0;
    private void Awake()
    {
        instance = this;
        dicSound = new Dictionary<string, AudioClip>();
        listSoundEffect = new List<AudioSource>();
        listSoundEffect.Add(soundEffect);
        
    }
    //public void LoadAssetPetSound()
    //{
    //    if (soundPetIsLoaded) return;
    //    AssetBundle assetBundle = AssetBundleHelper.GetAssetBundleByName(NameAssetBundle.SOUND_PET);
    //    AudioClip[] _audioClips = assetBundle.LoadAllAssets<AudioClip>();
    //    foreach (var item in _audioClips)
    //    {
    //        audioClips.Add(item);
    //    }
    //    soundPetIsLoaded = true;
    //}

    public void AddSoundFromBundle(AssetBundle assetBundle)
    {
        AudioClip[] _audioClips = assetBundle.LoadAllAssets<AudioClip>();
        foreach (var item in _audioClips)
        {
            if (!audioClips.Contains(item)) audioClips.Add(item);
        }
    }

    public void UpdateStatusSound()
    {
        //StatusSound = GameUtils.GetSoundStatus();
        cacheStatus = StatusSoundMusic;
    }



    private void SaveChangeStatusSound()
    {
        if (cacheStatus != StatusSoundMusic)
        {
            try
            {
                Debug.Log("SaveChangeStatusSound" + StatusSoundMusic);
                cacheStatus = StatusSoundMusic;
                //GameUtils.SaveSetting();
            }
            catch (System.Exception)
            {
            }
        }
    }



    public void InitSound(int sound)
    {
        StatusSoundMusic = sound;
        HandleChangeSoundState?.Invoke(StatusSoundMusic);
    }

    public void DispatchEvent()
    {
        HandleChangeSoundState?.Invoke(StatusSoundMusic);
    }

    public void PlaySoundEffect(string name)
    {
        AudioClip sound = getSoundByName(name);
        if (sound == null)
        {
            Debug.LogError("missing sound " + name);
        }
        AudioSource audioSource = getSoundEffectFree();
        audioSource.bypassEffects = false;
        audioSource.bypassListenerEffects = false;
        audioSource.bypassReverbZones = false;
        audioSource.volume = StatusSFX == 1 ? MAX_VOL : 0;
        audioSource.PlayOneShot(sound);
    }

    /// <summary>
    ///  Chi su dung PlaySoundEffectVer2 khi am thanh can Stop, Pause , UnPause
    /// </summary>
    public void PlaySoundEffectVer2(string name)
    {
        AudioClip sound = getSoundByName(name);
        if (sound == null)
        {
            Debug.LogError("missing sound " + name);
        }
        AudioSource audioSource = getSoundEffectFree();
        audioSource.bypassEffects = false;
        audioSource.bypassListenerEffects = false;
        audioSource.bypassReverbZones = false;
        audioSource.volume = StatusSFX == 1 ? MAX_VOL : 0;
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void StopSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (listSoundEffect[i].isPlaying && listSoundEffect[i].clip!= null && listSoundEffect[i].clip.name == name)
            {
                listSoundEffect[i].Stop();
                listSoundEffect[i].clip = null;
            }
        }
    }
    public void PauseSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (listSoundEffect[i].isPlaying && listSoundEffect[i].clip!= null && listSoundEffect[i].clip.name == name)
            {
                listSoundEffect[i].Pause();
            }
        }
    }
    public void UnPauseSoundEffect(string name)
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (!listSoundEffect[i].isPlaying  && listSoundEffect[i].clip!= null &&  listSoundEffect[i].clip.name == name)
            {
               listSoundEffect[i].UnPause();
            }
        }
    }

    private AudioSource getSoundEffectFree()
    {
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            if (!listSoundEffect[i].isPlaying && listSoundEffect[i].clip ==null)
            {
                return listSoundEffect[i]; 
            }
        }
        GameObject go = new GameObject();
        go.name = "Effect_" + (listSoundEffect.Count + 1);
        go.transform.SetParent(transform);
        AudioSource audioSource = go.AddComponent<AudioSource>();
        listSoundEffect.Add(audioSource);
        
        return audioSource;
    }

    public void PlaySoundMusic(string nameSound, bool loop = true, System.Action callbackEnd = null)
    {
        if (LOCK_CHANGE_SOUND_MUSIC) return;
        this.callbackEnd = callbackEnd;
        AudioClip audioClip = getSoundByName(nameSound);
        if (audioClip == null)
        {
//#if !UNITY_EDITOR
//            StartCoroutine(AssetURLHelper.LoadAudioClip(nameSound, audioResult =>
//            {
//                if (audioResult != null)
//                {
//                    dicSound.Add(nameSound, audioResult);
//                    playSound(audioResult);
//                }
//            }));
//#endif

            //string[] assets = new string[] { NameAssetBundle.SOUND_HOME };
            //AssetBundleHelper.Instance.LoadAssetBundle(assets, Enums.TypeLoading.None, false, () =>
            //{
            //    StartCoroutine(loadSoundComplete());
            //});
        }
        else
        {
            playSound(audioClip, loop);
        }
    }
    public void StopSoundMusic(float duration)
    {
        if (StatusSoundMusic == 1)
        {
            soundMusic.DOFade(0, duration);
        }
    }

    private void playSound(AudioClip audioClip, bool loop)
    {
        bool changeAudio = true;
        if (currentMusic != null)
        {
            changeAudio = currentMusic.name != audioClip.name;
        }
        currentMusic = audioClip;
       
        if (changeAudio)//
        {
            float length = audioClip.length;
            //this.Wait(length, onEndSoundChange);
            soundMusic.volume = 0;
            soundMusic.clip = audioClip;
            soundMusic.loop = loop;
            soundMusic.Play(0);
        }
        else // ko doi~ nhac thi` play tiep
        {
            
        }
        if (StatusSoundMusic == 1)
        {
            soundMusic.DOFade(MAX_VOL, 1f);
        }
        
    }
    private void onEndSoundChange()
    {   
        callbackEnd?.Invoke();
        callbackEnd = null;
    }

    public void MuteSound()
    {
        StatusSoundMusic = 0;
        if (tweenerMute != null) tweenerMute.Kill();
        tweenerMute = soundMusic.DOFade(0, 0.3f);
    }
    public void UnMuteSound()
    {
        StatusSoundMusic = 1;
        soundMusic.volume = MAX_VOL;
        if (tweenerMute != null) tweenerMute.Kill();
    }

    public void MuteSFS()
    {
        StatusSFX = 0;
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            listSoundEffect[i].volume = 0;
        }
    }
    public void UnMuteSFS()
    {
        StatusSFX = 1;
        for (int i = 0; i < listSoundEffect.Count; i++)
        {
            listSoundEffect[i].volume = MAX_VOL;
        }
    }

    public void StopSoundMusic()
    {
        soundMusic.DOFade(0, 1).OnComplete(()=> {
            soundMusic.Stop();
        });
    }

    public void ForceStopSound()
    {
        currentMusic = null;
        StopSoundMusic();
    }
    private AudioClip getSoundByName(string nameSound)
    {
        //if (dicSound.ContainsKey(nameSound)) return dicSound[nameSound];
        for (int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i].name == nameSound)
            {
                return audioClips[i];
            }
        }
        return null;
    }
    public void ForceMuteForRV(bool isMute)
    {
        soundMusic.mute = isMute;
    }
}
