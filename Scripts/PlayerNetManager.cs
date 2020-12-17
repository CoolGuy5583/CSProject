using UnityEngine;
using Mirror;

public class PlayerNetManager : NetworkBehaviour
{
    PlayerController player;
    Weapon weapon;
    bool isSet = false;

    void Start()
    {
        if (!isSet)
        {
            player = GetComponent<PlayerController>();
            weapon = GetComponentInChildren<Weapon>();
            if (isLocalPlayer)
            {
                weapon.enabled = true;
                GetComponent<BuildController>().enabled = true;
                player.camBoom.gameObject.SetActive(true);
                player.cam.gameObject.SetActive(true);
            }
            else
            {
                player.camBoom.gameObject.SetActive(false);
                weapon.mainCam.gameObject.SetActive(false);
                weapon.aimCam.gameObject.SetActive(false);
                Destroy(weapon.controller);
                weapon.enabled = false;
                Destroy(player.cam.gameObject);
            }
            isSet = true;
        }

        if (Time.time >= 2f && isSet == true)
        {
            Destroy(GetComponent<PlayerNetManager>());
        }
    }
}
