using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Weapon HUD Display")]
    public TextMeshProUGUI magazineDisplay;

    public void UpdateMagazineDisplay(int curMagazine, int magazineMax)
    {
        magazineDisplay.text = curMagazine + " / " + magazineMax;
    }
}
