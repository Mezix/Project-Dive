using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    //  Movement
    private float horizontalInput, forwardInput, verticalInput;
    private float desiredX;
    private float _speedMultiplier;
    public float _sprintValue;
    private bool lockMovement;
    public float moveSpeedForce;
    public float maxMoveSpeed;
    public float counterMovement;

    //  Look
    [HideInInspector] public float _tempSensitivity;
    [HideInInspector] public float _currentSensitivity;
    [HideInInspector] public float _maxSensitivity;
    public static float _savedSens = -1;
    private float xRotation;
    private bool lockRotation;
    public Transform _orientation;

    //  Jump/Grounded

    [HideInInspector] public float jumpForce;
    [HideInInspector] public float jumpCooldown;
    private bool jumping;
    private bool readyToJump;
    public bool _grounded;

    //  Tools and UI
    public PlayerUI _pUI;
    public List<AToolUI> Tools;
    private int _toolIndex;
    public Transform ArmRotation;
    [HideInInspector] public bool _holstered;
    [HideInInspector] public bool _holstering;

    //  Misc
    [HideInInspector] public KeyCode lastHitKey;
    [HideInInspector] public PlayerHealth _pHealth;
    [HideInInspector] public Rigidbody playerRB;
    [HideInInspector] public Collider playerCol;
    [HideInInspector] public Camera _playerCam;
    public Camera _FPSLayerCam;
    [HideInInspector] public bool _firstPersonActive;
    public LayerMask whatIsGround;

    //Physics

    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    private Vector3 normalVector = Vector3.up;
    private bool cancellingGrounded;
    public float _floatingDrag;
    public float _walkingDrag;

    //  Weapons

    public AWeapon _currentWeapon;
    public int _weaponDirection; //1 is forward, -1 is back
    public Transform _armRotation;

    private void Awake()
    {
        REF.PCon = this;
        transform.tag = "Player";
        _playerCam = Camera.main;
        playerRB = GetComponentInChildren<Rigidbody>();
        playerCol = GetComponentInChildren<Collider>();
        _pHealth = GetComponentInChildren<PlayerHealth>();
    }
    void Start()
    {
        if (_savedSens == -1) _currentSensitivity = _tempSensitivity = 250; //  Initialize sensitivity once per launch, otherwise use the static saved variable
        else _currentSensitivity = _savedSens;
        _maxSensitivity = 700;

        _firstPersonActive = true;
        readyToJump = true;
        playerCol.isTrigger = false;
        playerRB.freezeRotation = true;
        lockMovement = lockRotation = false;
        _holstered = _holstering = false;
        playerCol.isTrigger = false;
        _toolIndex = 0;
        _weaponDirection = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        //HandleTools();
        HandleMouseClickInput();
        HandleKeyboardInput();
        Look();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    //  Input

    private void HandleKeyboardInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        forwardInput = Input.GetAxisRaw("Vertical");
        verticalInput = 0;
        if (Input.GetKey(KeyCode.Space)) verticalInput = 1;
        if (Input.GetKey(KeyCode.LeftControl)) verticalInput = -1;

        jumping = Input.GetKey(KeyCode.Space);
        if (Input.GetKey(KeyCode.LeftShift)) Sprint(true);
        else Sprint(false);

    }
    private void HandleMouseClickInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool firingPossible = _currentWeapon.Fire();
            if (!firingPossible) return;
            ApplyKnockback();
        }
        if (Input.GetKey(KeyCode.Mouse1)) ReverseWeapon(true); 
        else  ReverseWeapon(false); 
    }

    private void ReverseWeapon(bool reversed)
    {
        if(reversed)
        {
            _weaponDirection = -1;
            HM.RotateLocalTransformToAngle(_armRotation, new Vector3(_armRotation.localRotation.eulerAngles.x, 180, ArmRotation.localRotation.eulerAngles.z));
        }
        else
        {
            _weaponDirection = 1;
            HM.RotateLocalTransformToAngle(_armRotation, new Vector3(_armRotation.localRotation.eulerAngles.x, 0, ArmRotation.localRotation.eulerAngles.z));
        }
    }

    private void Look()
    {
        if (lockRotation) return;

        float mouseX = Input.GetAxis("Mouse X") * _currentSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _currentSensitivity * Time.fixedDeltaTime;

        //Find current look rotation
        Vector3 rot = _playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        _playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        _FPSLayerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        _orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    // Movement

    private void HandleMovement()
    {
        //Extra gravity
        //playerRB.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(horizontalInput, forwardInput, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = maxMoveSpeed * _speedMultiplier;

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (horizontalInput > 0 && xMag > maxSpeed * _speedMultiplier) horizontalInput = 0;
        if (horizontalInput < 0 && xMag < -maxSpeed * _speedMultiplier) horizontalInput = 0;
        if (forwardInput > 0 && yMag > maxSpeed * _speedMultiplier) forwardInput = 0;
        if (forwardInput < 0 && yMag < -maxSpeed * _speedMultiplier) forwardInput = 0;

        if (playerRB.velocity.magnitude > maxSpeed * _speedMultiplier)
        {
            horizontalInput = 0;
            forwardInput = 0;
            verticalInput = 0;
        }

        //Apply forces to move player
        if (!_grounded) //if floating, move towards camera, otherwise, try walking on the seabed
        {
            playerRB.useGravity = false;
            playerRB.drag = _floatingDrag;
            playerRB.AddForce((
                _playerCam.transform.forward * forwardInput
                + _playerCam.transform.up * verticalInput
                + _playerCam.transform.right * horizontalInput)
                * moveSpeedForce * Time.deltaTime * _speedMultiplier);
        }
        else
        {
            playerRB.useGravity = true;
            playerRB.drag = _walkingDrag;
            playerRB.AddForce(_orientation.transform.forward * forwardInput * moveSpeedForce * Time.deltaTime * _speedMultiplier);
            playerRB.AddForce(_orientation.transform.right * horizontalInput * moveSpeedForce * Time.deltaTime * _speedMultiplier);
        }
    }
    private void Sprint(bool printing)
    {
        if (printing) _speedMultiplier = _sprintValue;
        else _speedMultiplier = 1;
    }
    private void Jump()
    {
        if (_grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            playerRB.AddForce(Vector2.up * jumpForce * 1.5f);
            playerRB.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = playerRB.velocity;
            if (playerRB.velocity.y < 0.5f)
                playerRB.velocity = new Vector3(vel.x, 0, vel.z);
            else if (playerRB.velocity.y > 0)
                playerRB.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!_grounded || jumping) return;
        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            playerRB.AddForce(moveSpeedForce * _orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            playerRB.AddForce(moveSpeedForce * _orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(playerRB.velocity.x, 2) + Mathf.Pow(playerRB.velocity.z, 2))) > maxMoveSpeed)
        {
            float fallspeed = playerRB.velocity.y;
            Vector3 n = playerRB.velocity.normalized * maxMoveSpeed;
            playerRB.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = _orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(playerRB.velocity.x, playerRB.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = playerRB.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }



    //  Grounded
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }


    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                _grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }
    private void StopGrounded()
    {
        _grounded = false;
    }

    private void ApplyKnockback()
    {
        playerRB.AddForce(_playerCam.transform.forward * (-1 * _weaponDirection ) * _currentWeapon._knockbackForce);
    }








    //  TOOLS

    private void HandleTools()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                NextTool();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                PreviousTool();
            }
        }
        else
        {
            HandleToolIndex();
        }
    }

    private void OnGUI()
    {
        if (Event.current.isKey)
        {
            if (Event.current.keyCode != KeyCode.None) lastHitKey = Event.current.keyCode;
        }
    }
    private void HandleToolIndex()
    {
        int oldToolIndex = _toolIndex;
        bool swap = false;

        //  Check if we have any inputs at all
        if (_holstering) return;
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.X))
        {
            if (!_holstered)
            {
                StopAllCoroutines();
                StartCoroutine(HolsterTool(true));
            }
        }

        if (Input.anyKeyDown)
        {
            //  get any input and convert the ascii range into an int
            int tmpToolIndex = Convert.ToInt32(lastHitKey) - 49; //49 = Alpha1 => 49 - 49 = 0 => Our first tool in the list 
            if (tmpToolIndex < 0 || tmpToolIndex > 9) return; // the numbers in the ascii range are between 48 (= 0) and 57 (= 9), so only accept inputs between 1 and 9, else return
            if (tmpToolIndex != _toolIndex) swap = true; //if we are not selecting the same tool as last time, swap
            _toolIndex = tmpToolIndex; //now overwrite the tool index to compare with the next input at a later time

            if (_toolIndex >= Tools.Count) return; //dont go out of bounds
            REF.PlayerUI.SelectTool(_toolIndex);
            if (swap)
            {
                StopAllCoroutines();
                StartCoroutine(SwapAnimation());
            }
            else
            {
                if (_holstered || oldToolIndex != _toolIndex)
                {
                    StopAllCoroutines();
                    StartCoroutine(UnholsterTool(true));
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(HolsterTool(true));
                }
            }
        }
    }
    private IEnumerator HolsterTool(bool hideUI)
    {
        _holstering = true;
        _holstered = false;
        HM.RotateLocalTransformToAngle(ArmRotation, new Vector3(0, 0, 0));
        float TimeToSwap = 25f;

        for (int i = 0; i < TimeToSwap; i++)
        {
            HM.RotateLocalTransformToAngle(ArmRotation, new Vector3(i / TimeToSwap * (TimeToSwap * 2), 0, 0));
            yield return new WaitForSecondsRealtime(0.01f);
        }
        _holstered = true;
        _holstering = false;
        //if (hideUI) REF.PlayerUI.ChangeToolBarOpacity(false);
    }
    private IEnumerator UnholsterTool(bool showUI)
    {
        _holstering = true;
        _holstered = true;
        SetToolActive(_toolIndex);

        float TimeToSwap = 25f;
        for (int i = 0; i < TimeToSwap; i++)
        {
            HM.RotateLocalTransformToAngle(ArmRotation, new Vector3((TimeToSwap * 2) - i / TimeToSwap * (TimeToSwap * 2), 0, 0));
            yield return new WaitForSecondsRealtime(0.01f);
        }
        HM.RotateLocalTransformToAngle(ArmRotation, new Vector3(0, 0, 0));
        _holstered = false;
        _holstering = false;

        if (showUI) REF.PlayerUI.ChangeToolBarOpacity(true);
    }

    private void PreviousTool()
    {
        _toolIndex--;
        if (_toolIndex < 0)
        {
            _toolIndex = Tools.Count - 1;
        }
        StopAllCoroutines();
        StartCoroutine(SwapAnimation());
    }
    private void NextTool()
    {
        _toolIndex++;
        if (_toolIndex > Tools.Count - 1) _toolIndex = 0;
        StopAllCoroutines();
        StartCoroutine(SwapAnimation());
    }
    private IEnumerator SwapAnimation()
    {
        REF.PlayerUI.SelectTool(_toolIndex);
        if (!_holstered)
        {
            StartCoroutine(HolsterTool(false));
            yield return new WaitWhile(() => !_holstered);
        }
        StartCoroutine(UnholsterTool(true));
    }
    private void SetToolActive(int i)
    {
        if (Tools.Count == 0) return;
        foreach (AToolUI g in Tools)
        {
            if (g) g.gameObject.SetActive(false);
        }
        if (Tools[i]) Tools[i].gameObject.SetActive(true);
    }




}