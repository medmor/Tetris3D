using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Player")]
public class Player : ScriptableObject
{
    #region Declarations
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public int linesRemoved { get; private set; }
    [field: SerializeField] public int score { get; private set; }
    [field: SerializeField] public int level { get; private set; }
    [field: SerializeField] public int linesToLevelUp { get; private set; }

    #endregion

    public void InitPlayer()
    {
        if (GameSaveManager.Instance.IsLevelSaved(GameManager.Instance.GameMode))
            SetLevel(GameSaveManager.Instance.GetLevel(GameManager.Instance.GameMode));
        score = 0;
        linesRemoved = 0;
    }

    public void SetSpeed(float value) => speed = value;
    public void SetLinesRemoved(int value) => linesRemoved = value;
    public void SetScore(int value) => score = value;
    public void SetLevel(int value) => level = value;
    public void SetLinesToLevelUp(int value) => linesToLevelUp = value;
}
