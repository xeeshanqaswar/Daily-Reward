using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleUsage : MonoBehaviour
{
    private void OnEnable()
    {
        DailyReward.OnClaimReward += MyCustomFunctionality;
    }
    
    // Day count starts from 0 to onward
    private void MyCustomFunctionality(int day)
    {
        Debug.Log("Player claimed the reward of Day "+ day);
    }

    private void OnDisable()
    {
        DailyReward.OnClaimReward -= MyCustomFunctionality;
    }
}
