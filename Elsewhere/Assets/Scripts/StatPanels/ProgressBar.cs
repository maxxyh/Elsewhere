using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private int _maximum;
    private int _current;
    [SerializeField] private Image mask;
    [SerializeField] private Text levelLabel;
    [SerializeField] private Text numberLabel;

    private void Start()
    {
        SetCurrentFill(0, 100, 1);
    }

    public void SetCurrentFill(int current, int maximum, int currLevel)
    {
        //Debug.Log("setting progress bar fill");
        float fillAmount = (float) current / (float) maximum;
        mask.fillAmount = fillAmount;
        levelLabel.text = $"Level {currLevel}";
        numberLabel.text = $"{current} / {maximum}";
    }
}
