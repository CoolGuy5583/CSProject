using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCollision : MonoBehaviour
{
    [SerializeField] Collider player;
    public bool isGrounded = true;
    public bool canSpawn = true;
    public bool isAttached = false;
    [SerializeField] Collider[] objs;
    [SerializeField] Destructible des;

    //void Start()
    //{
        //des = GetComponent<Destructible>() != null ? GetComponent<Destructible>() : GetComponentInParent<Destructible>();
    //}

    void FixedUpdate()
    {
        objs = Physics.OverlapBox(transform.position, transform.localScale/2*1.1f, transform.rotation);

        isGrounded = false;
        isAttached = true;

        foreach (Collider obj in objs)
        {
            isGrounded = true;

            if (objs.Length == 1)
            {
                if (player != null && obj == player)
                {
                    isGrounded = false;
                    break;
                }
                else if (obj.transform == transform)
                {
                    isGrounded = false;
                    break;
                }
            }
            if (obj.transform.position == transform.position)
            {
                canSpawn = false;
                break;
            }
            else
            {
                canSpawn = true;
            }

            if (obj.transform.position.y < transform.position.y && obj.transform != transform || obj.tag == "ground")
            {
                isAttached = true;
            }
        }


        if ((!isGrounded || !isAttached) && des != null)
        {
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.25f);

        des.Damage(100f);
    }
}
