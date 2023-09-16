using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderbordItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public string Name { get; private set; }

    public void SetName(string name)
    {
        Name = name;
        nameText.text = name;
    }

    public void SetScore(int score)
    {
        scoreText.text = $"{score}";
    }
}
