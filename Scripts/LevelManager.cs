using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Prefabs (sirayla dizilmeli)")]
    public GameObject[] levelPrefabs;

    private Queue<GameObject> activeLevels = new Queue<GameObject>();
    private int currentLevelIndex = 0;

    // Progress tracking
    [Header("Progress Bar Settings")]
    public RoundedProgressBar progressBar;   // Pause menu bar reference
    [Min(1)] public int totalCheckpoints = 5; // You can set this manually
    private HashSet<int> reachedSet = new HashSet<int>();

    void Start()
    {
        // Spawn first 3 levels (positions come from prefab)
        for (int i = 0; i < 3 && i < levelPrefabs.Length; i++)
        {
            GameObject level = Instantiate(levelPrefabs[i]);
            activeLevels.Enqueue(level);
            currentLevelIndex++;
            Debug.Log($"[LevelManager] Level {i} added to scene.");
        }

        // Reset progress bar (if exists)
        if (progressBar != null)
        {
            progressBar.useDebugDriver = false; // disable manual debug
            progressBar.ResetProgress(totalCheckpoints);
        }
    }

    // Called by checkpoint triggers
    public void OnCheckpointReached(int reachedLevel)
    {
        Debug.Log($"[LevelManager] Checkpoint triggered: Level {reachedLevel}");

        // Prevent counting the same checkpoint twice
        if (reachedSet.Add(reachedLevel))
        {
            if (progressBar != null)
                progressBar.OnCheckpointReachedOnce();
        }

        // Original level system (unchanged)
        if (reachedLevel >= 2)
        {
            // Add next level if exists
            if (currentLevelIndex < levelPrefabs.Length)
            {
                GameObject nextLevel = Instantiate(levelPrefabs[currentLevelIndex]);
                activeLevels.Enqueue(nextLevel);
                Debug.Log($"[LevelManager] Level {currentLevelIndex} added to scene.");
                currentLevelIndex++;
            }

            // Keep max 3 active levels
            if (activeLevels.Count > 3)
            {
                GameObject oldLevel = activeLevels.Dequeue();
                Debug.Log($"[LevelManager] {oldLevel.name} removed from scene.");
                Destroy(oldLevel);
            }
        }
    }
}