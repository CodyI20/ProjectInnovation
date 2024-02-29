using UnityEngine;
using TMPro;
using Fusion;

public class TurnTimerUI : NetworkBehaviour
{
    private bool isTimerActive { get; set;}
    private float currentTurnTime { get; set;}

    public GameObject TurnTimer { get; private set; }
    private TMP_Text timerText { get; set; }
    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        // Get the change detector.
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SnapshotTo, false);
    }

    private void Awake()
    {
        timerText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMatchStart += SetGameObjectActive;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMatchStart -= SetGameObjectActive;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    void SetGameObjectActive()
    {
        isTimerActive = true;
        gameObject.SetActive(true);
    }

    
    void RpcUpdateTimerText(float updatedTime)
    {
        currentTurnTime = updatedTime;
    }

    private void Update()
    {
        if (isTimerActive)
        {
            UpdateTimerText();
            RpcUpdateTimerText(GameManager.Instance.CurrentTurnTime);
        }
    }

    void UpdateTimerText()
    {
        timerText.text = currentTurnTime.ToString("F0");
    }
}
