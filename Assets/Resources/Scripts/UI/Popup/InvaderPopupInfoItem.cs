using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvaderPopupInfoItem : MonoBehaviour
{
    public Text txtShipStatName;
    public Text txtShipStatValue;
    public Text txtShipStatLVLUPValue;
   
    public Text txtWeaponStatName;
    public Text txtWeaponStatValue;
    public Text txtWeaponStatLVLUPValue;

    public void init(string shipStatName, string shipStatValue, string shipStatLVLUPValue, string weaponStatName, string weaponStatValue, string weaponStatLVLUPValue)
    {
        txtShipStatName.text = shipStatName;
        txtShipStatValue.text = shipStatValue;
        txtShipStatLVLUPValue.text = shipStatLVLUPValue;
        
        txtWeaponStatName.text = weaponStatName;
        txtWeaponStatValue.text = weaponStatValue;
        txtWeaponStatLVLUPValue.text = weaponStatLVLUPValue;
    }
}
