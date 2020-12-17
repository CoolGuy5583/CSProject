using UnityEngine;
using TMPro;
using System.Collections;
using Mirror;

public class Weapon : NetworkBehaviour
{
    [SerializeField] GameObject impact;
    [SerializeField] Transform spine;
    float nextTimeToFire = 0f;
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject mainCam, aimCam;
    bool isScoping = false;
    bool prevScopingState = false;
    public RaycastHit hit;
    Quaternion prevRot = new Quaternion();
    [SerializeField] Transform recoilBone;
    Quaternion normRecoilBoneRot;
    public WeaponController controller;
    [SerializeField] TextMeshProUGUI ammoText;

    void Start()
    {
        normRecoilBoneRot = recoilBone.localRotation;
        ammoText = GameObject.Find("ammoCountText").GetComponent<TextMeshProUGUI>();
        mainCam.SetActive(false);
        aimCam.SetActive(false);
    }

    void Update()
    {
        if (ammoText == null)
            ammoText = GameObject.Find("ammoCountText").GetComponent<TextMeshProUGUI>();


        if (Input.GetMouseButtonDown(1))
            isScoping = true;
        else if (Input.GetMouseButtonUp(1))
            isScoping = false;

        if (controller.currentWeaponStats.ROF != 0)
        {
            if (Input.GetButton("Fire1"))
                Shoot();

        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }

        if (prevScopingState != isScoping)
        {
            if (isScoping)
                Scope();
            else
                UnScope();
        }

        prevScopingState = isScoping;

        ammoText.text = controller.currentWeaponStats.ammo.ToString() + " | " + controller.currentWeaponStats.ammoPerClip.ToString();
    }

    void LateUpdate()
    {
        spine.rotation = Quaternion.Lerp(prevRot, Quaternion.Euler(new Vector3(-transform.rotation.eulerAngles.x, spine.rotation.eulerAngles.y, spine.rotation.eulerAngles.z)), Time.deltaTime * 20);

            //controller.currentWeaponStats.bulletEffect.transform.LookAt(hit.point);

        prevRot = spine.rotation;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (Input.GetButton("Fire1"))
    //    {
    //        if (Time.timeSinceLevelLoad >= nextTimeToFire)
    //        {
    //            RaycastHit hit;
    //            if (Physics.Raycast(transform.position, new Vector3(transform.forward.x + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy), transform.forward.y + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy), transform.forward.z + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy)), out hit))
    //            {
    //                nextTimeToFire = 1 / rateOfFire + Time.timeSinceLevelLoad;
    //                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
    //                if (rb != null)
    //                {
    //                    rb.AddForce(-hit.normal * controller.currentWeaponStats.hitForce, ForceMode.Impulse);
    //                }
    //                Gizmos.DrawLine(transform.position, hit.point);
    //            }
    //        }
    //    }
    //}

    void Scope()
    {
        aimCam.SetActive(true);
        mainCam.SetActive(false);
        controller.currentWeaponStats.accuracy /= controller.currentWeaponStats.scopedAccuracyFactor;
    }

    void UnScope()
    {
        mainCam.SetActive(true);
        aimCam.SetActive(false);
        controller.currentWeaponStats.accuracy *= controller.currentWeaponStats.scopedAccuracyFactor;
    }

    public void Shoot()
    {
        if (Time.time >= nextTimeToFire && playerController.canShoot && controller.currentWeaponStats.ammo > 0)
        {
            controller.currentWeaponStats.ammo -= 1;
            if (controller.currentWeaponStats.ROF != 0)
                nextTimeToFire = 1f / controller.currentWeaponStats.ROF + Time.time;
            else
                nextTimeToFire += 1;

            if (Physics.Raycast(transform.position, new Vector3(transform.forward.x + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy), transform.forward.y + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy), transform.forward.z + Random.Range(-controller.currentWeaponStats.accuracy, controller.currentWeaponStats.accuracy)), out hit, controller.currentWeaponStats.range, playerController.mask))
            {
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(-hit.normal * controller.currentWeaponStats.hitForce, ForceMode.Impulse);
                }
                if (hit.transform.GetComponent<Destructible>())
                {
                    hit.transform.GetComponent<Destructible>().Damage(controller.currentWeaponStats.damage);
                }
                if (hit.transform.GetComponentInParent<Destructible>())
                {
                    hit.transform.GetComponentInParent<Destructible>().Damage(controller.currentWeaponStats.damage);
                }
                if (hit.transform.GetComponent<PlayerController>())
                {
                    hit.transform.GetComponent<PlayerController>().CmdDamage(controller.currentWeaponStats.damage);
                }
                StartCoroutine(PlayImpact());
                controller.currentWeaponStats.bulletEffect.transform.LookAt(hit.point);
            }
            controller.currentWeaponStats.muzzleFlash.Stop();
            controller.currentWeaponStats.bulletEffect.Stop();
            //shot.Stop();
            controller.currentWeaponStats.muzzleFlash.Play();
            controller.currentWeaponStats.bulletEffect.Play();
            controller.currentWeaponStats.shot.pitch = Random.Range(0.9f, 1.1f);
            controller.currentWeaponStats.shot.volume = Random.Range(0.9f, 1.1f);
            controller.currentWeaponStats.shot.Play();
        }
    }

    private IEnumerator PlayImpact()
    {
        yield return new WaitForSeconds(0.1f);

        Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
    }
}

//class weapons
//{
//    static GameObject autoMesh;
//    static GameObject sniperMesh;
//    static GameObject semiMesh;
//    static GameObject smgMesh;
//    static GameObject knifeMesh;
//    public weapons(GameObject auto, GameObject sniper, GameObject semi, GameObject smg, GameObject knife)
//    {
//        autoMesh = auto;
//        sniperMesh = sniper;
//        semiMesh = semi;
//        smgMesh = smg;
//        knifeMesh = knife;
//    }

//    public weapon auto = new weapon(50, 5, 0.02f, 15, autoMesh);
//    public weapon sniper = new weapon(int.MaxValue, 20, 0.1f, 100, sniperMesh);
//    public weapon semi = new weapon(int.MaxValue, 15, 0.05f, 50, semiMesh);
//    public weapon smg = new weapon(10, 2, 0.05f, 10, smgMesh);
//    public weapon knife = new weapon(int.MaxValue, 15, 0.05f, 50, semiMesh);
//}


//class weapon
//{
//    public weapon(int rof, float hf, float acc, float dam, GameObject gunObj)
//    {
//        int rateOfFire = rof;
//        float controller.currentWeaponStats.hitForce = hf;
//        float controller.currentWeaponStats.accuracy = acc;
//        float damage = dam;
//        GameObject gunObject = gunObj;
//    }
//}