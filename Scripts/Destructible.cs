using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Destructible : NetworkBehaviour
{
    public enum Material { Wood, Brick, Steel };

    [SerializeField] float health = 100f;
    [SerializeField] Material material = Material.Wood;
    [SerializeField] float amount = 5f;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            CmdDestroy(gameObject);
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdDestroy(GameObject obj)
    {
        RpcDestroy(obj);
    }

    [ClientRpc]
    void RpcDestroy(GameObject obj)
    {
        Destroy(obj);
        Destroy(gameObject);
    }

    public float Damage(float amount)
    {
        health -= amount;
        return health;
    }
}
