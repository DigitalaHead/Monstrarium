using UnityEngine;
using System.Collections;

[System.Serializable]
public class Essence 
{
    public EssenceColor color { get; private set; } // Свойство для цвета эссенции

    public Essence(EssenceColor essenceColor)
    {
        color = essenceColor;
    }
}

public class EssenceController : MonoBehaviour
{
    [SerializeField]
    private EssenceColor essenceColor; // Цвет эссенции

    public EssenceColor Color => essenceColor; // Свойство для доступа к цвету

    void Start()
    {
        
    }

    // Метод для получения эссенции
    public Essence CreateEssence()
    {
        return new Essence(essenceColor); // Возвращаем эссенцию с цветом
    }
}