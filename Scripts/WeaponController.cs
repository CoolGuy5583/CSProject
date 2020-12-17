using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    int currentWeaponIndex = 0;
    public WeaponStat currentWeaponStats;
    RectTransform weaponDisplay_P;
    Image[] weaponDisplays = new Image[5];
    PlayerController player;
    GameObject axe;

    void Start()
    {
        weaponDisplay_P = GameObject.Find("gotten weapons").GetComponent<RectTransform>();

        for (int i = 0; i < 5; i++)
        {
            weaponDisplays[i] = weaponDisplay_P.GetChild(i).GetComponent<Image>();
        }

        player = GetComponentInParent<PlayerController>();

        CmdSwitchWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !player.build.isBuilding)
        {
            if (currentWeaponIndex >= transform.childCount - 2)
                currentWeaponIndex = 0;
            else
                currentWeaponIndex++;

            CmdSwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !player.build.isBuilding)
        {
            currentWeaponIndex = -1;
        }

        weaponDisplays[currentWeaponIndex+1].GetComponentInChildren<TextMeshProUGUI>().text = currentWeaponStats.ammo.ToString();
    }

    //[Command(ignoreAuthority = true)]
    void CmdSwitchWeapon()
    {
        RpcSwitchWeapon();
    }

    //[ClientRpc]
    void RpcSwitchWeapon()
    {
        int i = -1;

        foreach (Transform weapon in transform)
        {
            if (i == currentWeaponIndex)
            {
                if (currentWeaponIndex != -1)
                {
                    weapon.gameObject.SetActive(true);
                    currentWeaponStats = weapon.gameObject.GetComponent<WeaponStat>();
                    weaponDisplays[i+1].GetComponent<RectTransform>().localPosition = new Vector2(weaponDisplays[i+1].GetComponent<RectTransform>().localPosition.x, 7);
                }
                else
                {
                    axe.SetActive(true);
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            if (i != -1)
            {
                weaponDisplays[i+1].gameObject.SetActive(true);
                weaponDisplays[i+1].transform.GetChild(0).GetComponent<Image>().sprite = weapon.GetComponent<WeaponStat>().img;
                //weaponDisplays[i+1].GetComponentInChildren<TextMeshProUGUI>().text = currentWeaponStats.ammo.ToString();
                if (i != currentWeaponIndex)
                {
                    weaponDisplays[i+1].GetComponent<RectTransform>().localPosition = new Vector2(weaponDisplays[i+1].GetComponent<RectTransform>().localPosition.x, 0);
                }
            }
            else
            {

            }

            i++;
        }

    }

    public void Equip()
    {
        if (transform.childCount < 5)
        {

        }
        else
        {

        }
    }
}
