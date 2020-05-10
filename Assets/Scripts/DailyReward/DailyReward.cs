using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{

    #region FIELDS DELCERATION

    public SingleRewardTemplate[] rewardData;

    [Header("DATA COLLECTION")] public float rewardDuration = 12f;
    public bool resetOnDaysEnd;
    public GameObject rewardPrefab;
    public Transform rewardContainer;

    [Header("UI REFERENCES")] public Button rewardBtn;
    public TextMeshProUGUI timeLeftDisplay;

    #region EVENTS DECLERATION

    public static event Action<int> OnClaimReward;

    #endregion

    // Private Fields Decleration ======
    private long _lastRewardClaimed;
    private float _rewardDurationInMinisec;
    public List<RewardItem> spawnedRewards = new List<RewardItem>();

    private int _currentDay = 0;
    public int CurrentDay
    {
        get => _currentDay;
        private set => _currentDay = value;
    }

    private bool _isReadyForReward = false;
    public bool IsReadyForReward
    {
        get => _isReadyForReward;
        private set => _isReadyForReward = value;
    }

    #endregion

    private void Awake()
    {
        _rewardDurationInMinisec = rewardDuration * 3600f * 1000f;
        SpawnRewardItems();
    }

    private void OnEnable()
    {
        CurrentDay = int.Parse(PlayerPrefs.GetString("NextRewardDay", "0"));
        _lastRewardClaimed = long.Parse(PlayerPrefs.GetString("LastTimeRewardClaimed", "0"));
        rewardBtn.interactable = IsRewardReady();
        UpdateRewardItemUi();
    }

    public void SpawnRewardItems()
    {
        for (int i = 0; i < rewardData.Length; i++)
        {
            GameObject obj = Instantiate(rewardPrefab, Vector3.zero, Quaternion.identity, rewardContainer);
            RewardItem rewardObj = obj.GetComponent<RewardItem>();
            spawnedRewards.Add(rewardObj);
            // Update Ui
            rewardObj.heading.text = rewardData[i].rewardTitle;
            rewardObj.image.sprite = rewardData[i].rewardImage;
            rewardObj.Amount.text = rewardData[i].rewardAmount.ToString();
            rewardObj.claimed = CurrentDay > i;
            rewardObj.UpdateReward();
        }
    }

    private void Update()
    {
        if (!rewardBtn.IsInteractable())
        {
            if (IsRewardReady())
            {
                timeLeftDisplay.text = "00:00:00";
                rewardBtn.interactable = true;
                return;
            }

            // Display Time Left ======
            long timeDiff = DateTime.Now.Ticks - _lastRewardClaimed;
            long timeDiffms = timeDiff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (_rewardDurationInMinisec - timeDiffms) / 1000f;

            string h = Mathf.Floor(secondsLeft / 3600f).ToString("00");
            string m = Mathf.Floor((secondsLeft % 3600f) / 60f).ToString("00");
            string s = Mathf.Floor((secondsLeft % 3600f) % 60f).ToString("00");

            timeLeftDisplay.text = h + ":" + m + ":" + s;
        }

    }

    /// <summary>
    /// Logic that will execute on Reward claim press
    /// </summary>
    public void ClaimRewardBtn()
    {
        // Call action on Claim Reward with corresponding day
        OnClaimReward?.Invoke(CurrentDay);

        // Update Day and Reward Item
        if (CurrentDay == (rewardData.Length - 1))
        {
            if (resetOnDaysEnd)
            {
                CurrentDay = 0;
            }
            else
            {
                CurrentDay++;
                rewardBtn.gameObject.SetActive(false);
            }
            PlayerPrefs.SetString("NextRewardDay", CurrentDay.ToString());
        }
        else
        { 
            CurrentDay++;
            PlayerPrefs.SetString("NextRewardDay", CurrentDay.ToString());
        }
        UpdateRewardItemUi();


        rewardBtn.interactable = false;
        _lastRewardClaimed = DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastTimeRewardClaimed", _lastRewardClaimed.ToString());
    }

    private bool IsRewardReady()
    {
        long timeDiff = DateTime.Now.Ticks - _lastRewardClaimed;
        long timeDiffms = timeDiff / TimeSpan.TicksPerMillisecond;

        float secondsLeft = (_rewardDurationInMinisec - timeDiffms) / 1000f;

        if (secondsLeft < 0f)
        {
            IsReadyForReward = true;
            return true;
        }

        IsReadyForReward = false;
        return false;
    }

    private void UpdateRewardItemUi()
    {
        for (int i = 0; i < spawnedRewards.Count; i++)
        {
            if (i< CurrentDay)
            {
                spawnedRewards[i].claimed = true;
                spawnedRewards[i].UpdateReward();
            }
            else
            {
                spawnedRewards[i].claimed = false;
                spawnedRewards[i].UpdateReward();
            }
        }
    }

}



[Serializable]
public class SingleRewardTemplate
{
    public string rewardTitle;
    public Sprite rewardImage;
    public int rewardAmount;
}
