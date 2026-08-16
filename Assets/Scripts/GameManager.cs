using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text hiScoreText;
    [SerializeField] private Image flashImage;

    [Header("Explosion Settings")]
    [Range(0.1f, 2f)]
    [SerializeField] private float fadeDuration = 0.5f;

    [Range(0f, 5f)]
    [SerializeField] private float holdDuration = 1f;

    public int Score { get; private set; } = 0;

    private const string HiScoreKey = "hiscore";

    private void Awake()
    {
        HandleSingleton();

        if (scoreText == null)
        {
            scoreText = FindFirstObjectByType<Text>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void HandleSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        if (blade != null)
        {
            blade.enabled = true;
        }

        if (spawner != null)
        {
            spawner.enabled = true;
        }

        Score = 0;

        if (scoreText != null)
        {
            scoreText.text = Score.ToString();
        }

        if (hiScoreText != null)
        {
            hiScoreText.text = "Best: " + PlayerPrefs.GetFloat(HiScoreKey, 0f);
        }
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsByType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsByType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        Score += points;

        if (scoreText != null)
        {
            scoreText.text = Score.ToString();
        }

        float hiScore = PlayerPrefs.GetFloat(HiScoreKey, 0f);

        if (Score > hiScore)
        {
            hiScore = Score;
            PlayerPrefs.SetFloat(HiScoreKey, hiScore);

            if (hiScoreText != null)
            {
                hiScoreText.text = "Best: " + hiScore;
            }
        }
    }

    public void Explode()
    {
        if (blade != null)
        {
            blade.enabled = false;
        }

        if (spawner != null)
        {
            spawner.enabled = false;
        }

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            flashImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(holdDuration);

        NewGame();

        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            flashImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

}
