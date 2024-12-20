using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int playerPoints = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        playerPoints += points;
        Debug.Log($"���� ������: {playerPoints}");
    }

    public int GetPoints()
    {
        return playerPoints;
    }
    public void SetTotalTears(int totalTears)
    {
        playerPoints = totalTears;
        Debug.Log($"����� ���������� ���� �����������: {playerPoints}");
    }
}