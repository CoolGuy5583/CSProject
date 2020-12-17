using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] float speed = 3, runMult = 4, lookSensitivity = 5, jumpForce = 100, camClampValue = 80, health = 100f, armour = 0f;
    [SerializeField] Animator animController;
    [SerializeField] public Transform camBoom, cam;
    [SerializeField] RectTransform crosshair;
    public LayerMask mask;
    [SerializeField] Transform sphereCastStart;
    float anim_speed = 0, anim_walkDir = 0;
    float camRotx = 0, camRoty = 0;
    //Rigidbody rb;
    bool grounded = true;
    [SerializeField] float smoothTime = 0.25f;
    [SerializeField] float camDistance = 5f;
    float size = 50f;
    public bool canShoot = true;
    public BuildController build;
    TextMeshProUGUI armourText, healthText;
    [SerializeField] Slider healthSlider, armourSlider;
    [SerializeField] CharacterController charController;
    Vector3 gravVel = Vector3.zero;
    RaycastHit groundHit;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        armourSlider = GameObject.Find("armourSlider").GetComponent<Slider>();
        healthSlider = GameObject.Find("healthSlider").GetComponent<Slider>();
        healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        armourText = armourSlider.GetComponentInChildren<TextMeshProUGUI>();
        crosshair = GameObject.Find("crosshair").GetComponent<RectTransform>();
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //rb = GetComponent<Rigidbody>();
        animController.SetFloat("speed", 0);
        animController.SetFloat("walkDir", 0);
        build = GetComponent<BuildController>();
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
            Destroy(gameObject);
        }

        if (!isLocalPlayer)
        {
            return;
        }

        healthSlider.value = health;
        armourSlider.value = armour;

        healthText.text = Mathf.RoundToInt(health).ToString();
        armourText.text = Mathf.RoundToInt(armour).ToString();

        //grounded = Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Vector3.one / 8, -transform.up, Quaternion.Euler(Vector3.zero), 1.5f, mask);
        //grounded = charController.isGrounded;
        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.up, 0.75f, mask);
        Move();
        Look();

        //if (!grounded)
        //{
        //    animController.SetBool("jump", true);
        //}
        //else
        //{
        //    animController.SetBool("jump", false);
        //}

        if (grounded)
        {
            animController.SetBool("jump", false);
        }
        else
        {
            if (!Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.up, 0.95f))
                animController.SetBool("jump", true);
            else
                animController.SetBool("jump", false);
        }

        crosshair.sizeDelta = new Vector2(size, size);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible == false)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Gravity();
    }

    void Gravity()
    {

        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.up, 0.78f, mask);

        if (grounded)
        {
            gravVel = new Vector3(0, 0f, 0);
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out groundHit, 0.6f, mask))
                gravVel = new Vector3(((groundHit.normal.x) * Time.fixedDeltaTime / 2) + gravVel.x, (Physics.gravity.y * Time.fixedDeltaTime / 60) + gravVel.y, ((groundHit.normal.z) * Time.fixedDeltaTime / 2) + gravVel.z);
            else
            {
                if (Physics.Raycast(transform.position, -transform.forward, out groundHit, .6f, mask))
                    gravVel = new Vector3(((groundHit.normal.x) * Time.fixedDeltaTime / 2) + gravVel.x, (Physics.gravity.y * Time.fixedDeltaTime / 60) + gravVel.y, ((groundHit.normal.z) * Time.fixedDeltaTime / 2) + gravVel.z);
                else
                    gravVel = new Vector3(0, (Physics.gravity.y * Time.fixedDeltaTime / 60) + gravVel.y, 0);
            }

        }

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        charController.Move(gravVel);
    }

    // private void OnDrawGizmos()
    // {
    //     if (grounded)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireCube(transform.position + new Vector3(0, 2, 0) + (Vector3.down * hit.distance), Vector3.one);
    //         animController.SetBool("hump", false);
    //     }
    //     else
    //     {
    //         animController.SetBool("jump", true);
    //     }
    // }

    private void Move()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(_x, 0, _z).normalized;
        Vector3 moveDir;

        float angle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + camBoom.eulerAngles.y;
        float dirY = Mathf.LerpAngle(transform.eulerAngles.y, angle, smoothTime);

        if (Input.GetKey(KeyCode.LeftShift) && (_x != 0 || _z != 0))
        {
            angle = Mathf.Atan2(move.x * runMult, move.z * runMult) * Mathf.Rad2Deg + camBoom.eulerAngles.y;
            dirY = Mathf.LerpAngle(transform.eulerAngles.y, angle, smoothTime);

            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            anim_speed = 120;
            transform.eulerAngles = new Vector3(0, dirY, 0);

            //transform.localPosition = transform.position + (moveDir * runMult * speed * Time.fixedDeltaTime);
            charController.Move(moveDir * runMult * speed * Time.deltaTime);

            size = Mathf.Lerp(size, 200f, Time.deltaTime * 5f);

            canShoot = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && (_x != 0 || _z != 0))
        {
            anim_speed = 30;
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            transform.eulerAngles = new Vector3(0, dirY, 0);

            //transform.localPosition = transform.position + (moveDir * speed * Time.fixedDeltaTime);
            charController.Move(moveDir * speed * Time.deltaTime);

            size = Mathf.Lerp(size, 100f, Time.deltaTime * 5f);
            canShoot = true && !build.isBuilding;
        }
        else
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, dirY, 0), Time.deltaTime * 5);
            anim_speed = 0;

            size = Mathf.Lerp(size, 50f, Time.deltaTime * 5f);

            canShoot = true && !build.isBuilding;
        }
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, dirY, Time.deltaTime * 20), 0);
        }

        animController.SetFloat("speed", anim_speed);
        animController.SetFloat("walkDir", anim_walkDir);
    }

    private void Look()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        float _yRot = Input.GetAxis("Mouse X");
        float _xRot = Input.GetAxis("Mouse Y");

        camRotx = Mathf.Clamp(camRotx - (_xRot * lookSensitivity), -camClampValue, camClampValue);
        camRoty += _yRot * lookSensitivity;

        camBoom.eulerAngles = new Vector3(camRotx, camRoty, 0);
    }

    private void Jump()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        animController.SetBool("jump", true);
        //rb.AddRelativeForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        gravVel = new Vector3(0, jumpForce, 0);
        //charController.SimpleMove(new Vector3(0, jumpForce, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Pickup pickup = collision.gameObject.GetComponent<Pickup>();

        if (pickup != null)
        {
            switch (pickup.type)
            {
                case Pickup.Types.Ammo:
                    Destroy(pickup.gameObject, 1f);
                    build.looker.GetComponentInParent<Weapon>().controller.currentWeaponStats.ammo += (int)pickup.amount;
                    break;
                default:
                    break;
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdDamage(float amount)
    {
        RpcDamage(amount);
    }

    [ClientRpc]
    void RpcDamage(float amount)
    {
        if (armour == 0)
        {
            health -= amount;
        }
        else
        {
            armour -= amount;
        }
    }
}
