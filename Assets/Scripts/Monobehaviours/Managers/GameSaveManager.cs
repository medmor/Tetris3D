using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveManager : Manager<GameSaveManager>
{
    public readonly string Level = "level";
    public readonly string BestScore = "bestScore";
    public bool IsLevelSaved(Enums.GameMode mode) => PlayerPrefs.HasKey(Level + mode);

    public bool IsBestScoreSaved(Enums.GameMode mode)
        => PlayerPrefs.HasKey(BestScore + mode);

    public void SetLevel(Enums.GameMode mode, int value)
        => PlayerPrefs.SetInt(Level + mode, value);

    public int GetLevel(Enums.GameMode mode)
        => PlayerPrefs.GetInt(Level + mode);

    public void SetBestScore(Enums.GameMode mode, int value)
        => PlayerPrefs.SetInt(BestScore + mode, value);

    public int GetBestScore(Enums.GameMode mode)
        => PlayerPrefs.GetInt(BestScore + mode);

    public void ResetSaving()
    {
        SetBestScore(GameManager.Instance.GameMode, 0);
        SetLevel(GameManager.Instance.GameMode, 0);
    }
}
