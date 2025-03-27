using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Text redEssenceText;
    public Text yellowEssenceText;
    public Text blueEssenceText;

    private int redEssenceCount = 0;
    private int yellowEssenceCount = 0;
    private int blueEssenceCount = 0;

    public void UpdateEssenceCount(EssenceColor color)
    {
        switch (color)
        {
            case EssenceColor.Red:
                redEssenceCount = 1;
                break;
            case EssenceColor.Yellow:
                yellowEssenceCount = 1;
                break;
            case EssenceColor.Blue:
                blueEssenceCount = 1;
                break;
        }
        UpdateInventoryUI();
    }

    public void UpdatePotionCount(int amount)
    {
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        redEssenceText.text = "Красная эссенция: " + redEssenceCount;
        yellowEssenceText.text = "Желтая эссенция: " + yellowEssenceCount;
        blueEssenceText.text = "Синяя эссенция: " + blueEssenceCount;
    }
}