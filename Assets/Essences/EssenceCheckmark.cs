using UnityEngine;
using UnityEngine.UI;

public class EssenceCheckmark : MonoBehaviour
{
    public EssenceColor essenceColor; // Цвет зелья, который представляет эта галочка
    private Image checkmarkImage;

    private void Awake()
    {
        checkmarkImage = GetComponent<Image>();
    }

    public void UpdateCheckmark(bool hasEssence)
    {
        // Если зелье есть, делаем галочку зеленой, иначе белой
        checkmarkImage.color = hasEssence ? Color.green : Color.white;
    }
}