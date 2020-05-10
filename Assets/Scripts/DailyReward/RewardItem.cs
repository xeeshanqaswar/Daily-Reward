using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    [Header("UI REFERENCES")]
    public TextMeshProUGUI heading;
    public Image claimedOverlay;
    public Image image;
    public TextMeshProUGUI Amount;
    public bool claimed;


    private void OnEnable()
    {
        UpdateReward();
    }

    public void UpdateReward()
    {
        claimedOverlay.gameObject.SetActive(claimed);

    }
    
}
