using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private RectTransform leaderbordContent;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text startCountText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private GameObject finalScoreLayout;
    [SerializeField] private Button doubleScoreButton;
    [SerializeField] private LeaderbordItem leaderbordItem;

    private event Action OnScoreDoubledClick;

    public LeaderbordItem CreateLeaderbordItem(string name)
    {
        var item = Instantiate(leaderbordItem, leaderbordContent);
        item.SetName(name);
        item.SetScore(0);
        return item;
    }

    public void UpdateGameTime(string time)
    {
        timeText.text = time;
    }

    public void SetActiveStartCount(bool value)
    {
        startCountText.gameObject.SetActive(value);
    }

    public void UpdateStartCount(int count)
    {
        startCountText.text = $"{count}";
    }

    public void SetActiveFinalScore(bool value)
    {
        finalScoreLayout.SetActive(value);
    }
    
    public void SetActimeDoubleScoreButton(bool value)
    {
        doubleScoreButton.gameObject.SetActive(value);
    }

    public void SetFinalScore(int score)
    {
        finalScoreText.text = $"{score}";
    }

    public void SetScoreDoubledClick(Action callback)
    {
        OnScoreDoubledClick = callback;
    }

    private void OnDoubleScortClick()
    {
        OnScoreDoubledClick?.Invoke();
    }

    private void Awake()
    {
        doubleScoreButton.onClick.AddListener(OnDoubleScortClick);
    }

    private void OnDestroy()
    {
        doubleScoreButton.onClick.RemoveAllListeners();
        OnScoreDoubledClick = null;
    }

    private void Start()
    {
        SetActiveFinalScore(false);
    }
}
