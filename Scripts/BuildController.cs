using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class BuildController : NetworkBehaviour
{
    [SerializeField] Transform[] buildPreview;
    public Transform looker;
    public bool isBuilding = false;
    RaycastHit hit;
    [SerializeField] Transform[] build;
    [SerializeField] BuildCollision[] collider;
    [SerializeField] LayerMask mask;
    [SyncVar] int buildIndex = 0;
    [SerializeField] float buildDistance = 4.5f;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            isBuilding = !isBuilding;


        if (isBuilding) // && Physics.Raycast(looker.position, looker.forward, out hit, 6f, mask)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.E))
            {
                buildPreview[buildIndex].gameObject.SetActive(false);

                if (buildIndex >= buildPreview.Length - 1)
                    CmdChangeBuildType(0);
                else
                    CmdChangeBuildType(buildIndex + 1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                buildPreview[buildIndex].gameObject.SetActive(false);

                if (buildIndex <= 0)
                    CmdChangeBuildType(buildPreview.Length - 1);
                else
                    CmdChangeBuildType(buildIndex - 1);
            }

            Vector3 dir = looker.forward*buildDistance;

            buildPreview[buildIndex].gameObject.SetActive(true);
            buildPreview[buildIndex].position = (new Vector3(
                   Mathf.RoundToInt(looker.position.x) != 0 ? Mathf.RoundToInt(looker.position.x / 3.3f) * 3.3f : 0,
                  (Mathf.RoundToInt(looker.position.y - 1.5f) != 0 ? Mathf.RoundToInt((looker.position.y-1.5f) / 3.3f) * 3.3f : 0),
                   Mathf.RoundToInt(looker.position.z) != 0 ? Mathf.RoundToInt(looker.position.z / 3.3f) * 3.3f : 3.3f
                )) + (new Vector3(
                   Mathf.RoundToInt(dir.x) != 0 ? Mathf.RoundToInt(dir.x / 3.3f) * 3.3f : 3.3f
                , (Mathf.RoundToInt(dir.y) != 0 ? Mathf.RoundToInt(dir.y / 3.3f) * 3.3f : 0) + buildPreview[buildIndex].localScale.y*2
                ,  Mathf.RoundToInt(dir.z) != 0 ? Mathf.RoundToInt(dir.z / 3.3f) * 3.3f : 3.3f));

            buildPreview[buildIndex].eulerAngles = new Vector3(buildPreview[buildIndex].eulerAngles.x, Mathf.RoundToInt(looker.rotation.eulerAngles.y) != 0 ? Mathf.RoundToInt(transform.eulerAngles.y / 90f) * 90f : 0, buildPreview[buildIndex].eulerAngles.z);

            if (Input.GetMouseButton(0) && collider[buildIndex].isGrounded && collider[buildIndex].canSpawn)
            {
                Debug.Log("Local: " + build[buildIndex]);
                CmdBuild(build[buildIndex], buildPreview[buildIndex].position, buildPreview[buildIndex].rotation);
            }
        }
        else
        {
            buildPreview[buildIndex].gameObject.SetActive(false);
        }
    }

    [Command]
    void CmdBuild(Transform obj, Vector3 pos, Quaternion rot)
    {

        //Vector3 dir = looker.forward * buildDistance;

        //buildPreview[buildIndex].gameObject.SetActive(true);
        //buildPreview[buildIndex].position = (new Vector3(
        //       Mathf.RoundToInt(looker.position.x) != 0 ? Mathf.RoundToInt(looker.position.x / 3) * 3 : 0,
        //      (Mathf.RoundToInt(looker.position.y - 1.5f) != 0 ? Mathf.RoundToInt((looker.position.y - 1.5f) / 3) * 3 : 0),
        //       Mathf.RoundToInt(looker.position.z) != 0 ? Mathf.RoundToInt(looker.position.z / 3) * 3 : 3
        //    )) + (new Vector3(
        //       Mathf.RoundToInt(dir.x) != 0 ? Mathf.RoundToInt(dir.x / 3) * 3 : 3
        //    , (Mathf.RoundToInt(dir.y) != 0 ? Mathf.RoundToInt(dir.y / 3) * 3 : 0) + buildPreview[buildIndex].localScale.y * 2
        //    , Mathf.RoundToInt(dir.z) != 0 ? Mathf.RoundToInt(dir.z / 3) * 3 : 3));

        //buildPreview[buildIndex].eulerAngles = new Vector3(buildPreview[buildIndex].eulerAngles.x, Mathf.RoundToInt(looker.rotation.eulerAngles.y) != 0 ? Mathf.RoundToInt(transform.eulerAngles.y / 90f) * 90f : 0, buildPreview[buildIndex].eulerAngles.z);

        Debug.Log("Server: " + obj);
        //RpcSpawnBuild(build[buildIndex], buildPreview[buildIndex].position, buildPreview[buildIndex].rotation);
        //GameObject buildGO = Instantiate(build[buildIndex].gameObject, buildPreview[buildIndex].position, buildPreview[buildIndex].rotation);
        GameObject buildGO = Instantiate(build[buildIndex].gameObject, pos, rot);
        NetworkServer.Spawn(buildGO);
    }

    [Command]
    void CmdChangeBuildType(int amount)
    {
        buildIndex = amount;
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BuildController : MonoBehaviour
//{
//    [SerializeField] Transform[] buildPreview;
//    [SerializeField] Transform looker;
//    bool isBuilding = false;
//    RaycastHit hit;
//    [SerializeField] Transform[] build;
//    [SerializeField] LayerMask mask;
//    int buildIndex = 0;

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Tab))
//            isBuilding = !isBuilding;

//        if (isBuilding)
//        {
//            if (Physics.Raycast(looker.position, looker.forward, out hit, 3f, mask))
//            {
//                buildPreview[buildIndex].gameObject.SetActive(true);
//                buildPreview[buildIndex].position = new Vector3(
//                       Mathf.RoundToInt(hit.point.x) != 0 ? Mathf.RoundToInt(hit.point.x / 2) * 2 : 2
//                    , (Mathf.RoundToInt(hit.point.y) != 0 ? Mathf.RoundToInt(hit.point.y / 2) * 2 : 0) + buildPreview[buildIndex].localScale.y * 2
//                    , Mathf.RoundToInt(hit.point.z) != 0 ? Mathf.RoundToInt(hit.point.z / 2) * 2 : 2);
//                buildPreview[buildIndex].eulerAngles = new Vector3(0, Mathf.RoundToInt(transform.eulerAngles.y) != 0 ? Mathf.RoundToInt(transform.eulerAngles.y / 90f) * 90f : 0, 0);

//            }
//            else
//            {
//                buildPreview[buildIndex].gameObject.SetActive(true);
//                buildPreview[buildIndex].localPosition = new Vector3(
//                       Mathf.RoundToInt((looker.forward * 3f).x) != 0 ? Mathf.RoundToInt((looker.forward * 3f).x / 2) * 2 : 2
//                    , (Mathf.RoundToInt((looker.forward * 3f).y) != 0 ? Mathf.RoundToInt((looker.forward * 3f).y / 2) * 2 : 0) + buildPreview[buildIndex].localScale.y * 2
//                    , Mathf.RoundToInt((looker.forward * 3f).z) != 0 ? Mathf.RoundToInt((looker.forward * 3f).z / 2) * 2 : 2);

//            }
//            if (Input.GetMouseButtonDown(0))
//                Instantiate(build[buildIndex], buildPreview[buildIndex].position, buildPreview[buildIndex].rotation);
//        }
//        else
//        {
//            buildPreview[buildIndex].gameObject.SetActive(false);
//        }
//    }
//}
