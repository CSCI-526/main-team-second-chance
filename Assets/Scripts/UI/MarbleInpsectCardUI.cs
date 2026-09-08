using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarbleInpsectCardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private Image cardPanel;
    
    public void UpdateInformation(Marble marble)
    {
        MarbleData marbleData = marble.GetMarbleData();
        titleText.SetText(marbleData.MarbleName);
        cardPanel.material = null;
        string description = marbleData.MarbleDescription;
        if (marbleData.AbilityObject != null)
        {
            int maxTriggers = marbleData.AbilityObject.abilityMaxTriggers;
            description += $"\n\n(Uses left {maxTriggers - marble.timesCasted}/{maxTriggers})";
        }
        descriptionText.SetText(description);
    }
}
