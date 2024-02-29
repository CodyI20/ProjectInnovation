#if UNITY_EDITOR
using Fusion;
using UnityEngine;

public class DebugMessages : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerTurnEnd += PlayerTurnEnd;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerTurnEnd -= PlayerTurnEnd;
    }

    void PlayerTurnEnd(PlayerRef player)
    {
        Debug.Log("Player " + player + " turn ended");
    }
}
#endif
