using UnityEngine;
using System.Collections.Generic;

public class EssenceManager : MonoBehaviour
{
    private int redEssenceCount = 0;
    private int yellowEssenceCount = 0;
    private int blueEssenceCount = 0;

    public void CollectEssence(Essence essence)
    {
        switch (essence.color)
        {
            case EssenceColor.Red:
                redEssenceCount += essence.amount;
                Debug.Log("Collected Red Essence: " + redEssenceCount);
                break;
            case EssenceColor.Yellow:
                yellowEssenceCount += essence.amount;
                Debug.Log("Collected Yellow Essence: " + yellowEssenceCount);
                break;
            case EssenceColor.Blue:
                blueEssenceCount += essence.amount;
                Debug.Log("Collected Blue Essence: " + blueEssenceCount);
                break;
        }
    }
}