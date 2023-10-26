using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StarController : MonoBehaviour
{
    public static StarController instance;
    [SerializeField] private GameObject starPrefab = null;
    private Transform target;
    [Space]
    [Header("Available coins: (coins to pool")]
    private int maxStar;
    [SerializeField] private Transform poolPos;
    Queue<GameObject> starsQueue = new Queue<GameObject>();
    [Space]
    [Header("Animation Setting")]
    [SerializeField][Range(0.5f, 0.9f)] float minDuration;
    [SerializeField][Range(0.9f, 2f)] float maxDuration;
    [SerializeField] private Ease easeType;
    [SerializeField] private float spread;
    [SerializeField] private ParticleSystem hitEffect = null;

    private int star = 0;
    private void Awake()
    {
        instance = this;
        prepareStars();
    }
    public int Star
    {
        get { return star; }
        set { star = value; }
    }
    private void prepareStars()
    {
        maxStar = Config.MAX_COMBO + 10;
        for (int i = 0; i < maxStar; i++)
        {
            GameObject star;
            star = Instantiate(starPrefab);
            star.transform.parent = poolPos;
            star.SetActive(false);
            starsQueue.Enqueue(star);
        }
    }
    public void AddStar(Vector3 collectedCoinPos, Vector3 targetPosition, int amount, System.Action callback = null)
    {
        hitEffect.gameObject.SetActive(true);
        hitEffect.transform.position = collectedCoinPos;
        hitEffect.Play();
        for (int i = 0; i < amount; i++)
        {
            if (starsQueue.Count > 0)
            {
                GameObject star = starsQueue.Dequeue();
                star.SetActive(true);
                star.transform.position = collectedCoinPos + new Vector3(Random.Range(-spread, spread), 0, 0);
                float duration = Random.Range(minDuration, maxDuration);
                star.transform.DOMove(targetPosition, duration).SetEase(easeType).OnComplete(() =>
                {
                    callback?.Invoke();
                    star.SetActive(false);
                    starsQueue.Enqueue(star);
                });
            }
        }
    }

}
