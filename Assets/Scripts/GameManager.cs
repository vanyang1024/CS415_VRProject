using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Remove if you don't use UI
using TMPro;           // Remove if you don't use TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float gameDuration = 60f;      // total game time in seconds
    public int maxActiveTargets = 3;      // how many spheres at once

    [Header("Target Setup")]
    public GameObject targetPrefab;       // your sphere prefab
    public Transform[] spawnPoints;       // preset positions in scene

    [Header("UI (Optional)")]
    public Image timeBar;
    public GameObject timerText;

    private float remainingTime;
    private int score;
    private bool isPlaying;
    private int GameMode;
    private float prefabScale;

    private List<Transform> usedSpawnPoints = new List<Transform>();
    private List<GameObject> activeTargets = new List<GameObject>();

    private void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        prefabScale = 1f;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        // Update timer
        remainingTime -= Time.deltaTime;
        timeBar.fillAmount = remainingTime / gameDuration; 
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }

    // Called to (re)start a game
    public void StartGame()
    {
        // Reset state
        score = 0;
        remainingTime = gameDuration;
        isPlaying = true;

        UpdateScoreUI();
        UpdateTimerUI();

        // Clear old targets if any
        ClearAllTargets();
        timerText.SetActive(true);
        // Spawn initial set of targets
        FillTargets();
    }

    private void EndGame()
    {
        isPlaying = false;
        // Optionally: show end screen, highscore, etc.
        foreach (GameObject target in activeTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        timerText.SetActive(false);
        activeTargets.Clear();
        usedSpawnPoints.Clear();
        Debug.Log("Game Over! Final Score: " + score);
    }

    // Called by Target when it is clicked/hit
    public void OnTargetHit(GameObject target, float points)
    {
        if (!isPlaying)
            return;

        // Increase score
        score+=(int)points;
        UpdateScoreUI();

        // Free up that spawn point
        Transform spawnPoint = target.transform.parent;
        if (usedSpawnPoints.Contains(spawnPoint))
        {
            usedSpawnPoints.Remove(spawnPoint);
        }

        // Remove and destroy target
        activeTargets.Remove(target);
        Destroy(target);

        // Spawn a new one to keep maxActiveTargets
        FillTargets();
    }

    public void changeTargetScale(float newScale)
    {
        prefabScale = newScale*2f;
    }

    public void changeGameMode(int mode)
    {
        GameMode = mode;
        Debug.Log("Game mode changed to " + GameMode);
    }

    // Keep spawning until we have maxActiveTargets (or we run out of spawn points)
    private void FillTargets()
    {
        if (!isPlaying)
            return;

        while (activeTargets.Count < maxActiveTargets && usedSpawnPoints.Count < spawnPoints.Length)
        {
            Transform spawnPoint = GetRandomFreeSpawnPoint();
            if (spawnPoint == null)
                break;

            GameObject newTarget = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            if (GameMode == 1)
            {
                Target targetScript = newTarget.GetComponent<Target>();
                if (targetScript != null)
                {
                    targetScript.isShrinking = true;
                }
            }
            else if (GameMode == 2)
            {
                newTarget.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f), 0) * Random.Range(2f, 4f), ForceMode.VelocityChange);
            }
            
            newTarget.transform.localScale = Vector3.one * prefabScale;
            activeTargets.Add(newTarget);
            usedSpawnPoints.Add(spawnPoint);
        }
    }

    private Transform GetRandomFreeSpawnPoint()
    {
        // Build a list of free spawn points
        List<Transform> freePoints = new List<Transform>();
        foreach (Transform t in spawnPoints)
        {
            if (!usedSpawnPoints.Contains(t))
            {
                freePoints.Add(t);
            }
        }

        if (freePoints.Count == 0)
            return null;

        int index = Random.Range(0, freePoints.Count);
        return freePoints[index];
    }

    private void ClearAllTargets()
    {
        foreach (GameObject target in activeTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        activeTargets.Clear();
        usedSpawnPoints.Clear();
    }

    private void UpdateScoreUI()
    {
        if (timerText != null)
        {
            timerText.GetComponent<UnityEngine.UI.Text>().text = "Score: " + score.ToString();
        }
    }

    private void UpdateTimerUI()
    {

       
    }
}
