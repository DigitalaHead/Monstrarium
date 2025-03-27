using UnityEngine;
using System.Collections.Generic;

public class EssencePickup : MonoBehaviour
{
    [SerializeField]
    private EssenceColor essenceColor; // Цвет эссенции

    public Essence GetEssence()
    {
        return new Essence { color = essenceColor }; // Возвращаем эссенцию с цветом
    }
}
