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
    private bool lockMovement;
    public float normalMovementForce;
    public float vengeanceMovementForce;
    public float currentMovementForce;
    public float maxMoveSpeed;
    public float counterMovementModifier;

    // Dash
    public float _timeSinceLastDash;
    public float _dashCooldown;
    public float _dashForceMultiplier;

    // Physics

    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    private Vector3 normalVector = Vector3.up;
    private bool cancellingGrounded;
    public float _floatingDrag;
    public float _walkingDrag;

    //  Look
    [HideInInspector] public float _tempSensitivity;
    [HideInInspector] public float _currentSensitivity;
    //[HideInInspector] public float _maxSensitivity;
    public static float _savedSens = -1;
    private float xRotation;
    public bool _lockRotation;
    public Transform _orientation;

    //  Jump/Grounded

    [HideInInspector] public float jumpForce;
    [HideInInspector] public float jumpCooldown;
    private bool jumping;
    private bool readyToJump;
    public bool _grounded;
    public LayerMask whatIsGround;

    //  Weapons

    public Transform _arm;
    public Transform _armRotation;
    public List<GameObject> _weaponsToSpawn;
    [HideInInspector] public List<AWeapon> _weapons;
    [HideInInspector] public int _weaponIndex;
    [HideInInspector] public float _weaponSwapDuration;
    [HideInInspector] public int _weaponDirection; //1 is forward, -1 is back
    [HideInInspector] public bool _holstered;
    [HideInInspector] public bool _holstering;

    //  Melee

    public GameObject _melee;
    public float _meleeBaseDamage;
    public float _meleeDistance;
    public float _meleeKnockback;
    public float _meleeCooldown;
    public float _timeSinceLastMelee;

    //  Death

    public bool _dead;
    public bool _dying;

    //  Misc

    [HideInInspector] public PlayerUI _pUI;
    [HideInInspector] public KeyCode _lastHitKey;
    [HideInInspector] public Coroutine _dragCoroutine;
    [HideInInspector] public PlayerHealth _pHealth;
    [HideInInspector] public Rigidbody _playerRB;
    [HideInInspector] public Collider _playerCol;
    [HideInInspector] public Camera _playerCam;
    public Camera _FPSLayerCam;
    [HideInInspector] public bool _firstPersonActive;
    public float _totalDamageDealtByPlayer;

    //private AK.Wwise.RTPC speedRPTC;
    [SerializeField]
    private AK.Wwise.RTPC vengeanceRTPC;
    [SerializeField]
    private AK.Wwise.RTPC reverseWeaponRTPC;


    //FPS movement feedback
    const float maxTiltAngle = 7.5f;
    public float tiltAngle = 0;
    const float maxForwardWeaponOffset = 0.5f;
    public float weaponForwardOffset;
    public GameObject weaponForwardOffsetGameObject;

    public void Awake()
    {
        REF.PCon = this;
        transform.tag = "Player";
        _playerCam = Camera.main;
        _pUI = GetComponentInChildren<PlayerUI>();
        _playerRB = GetComponentInChildren<Rigidbody>();
        _playerCol = GetComponentInChildren<Collider>();
        _pHealth = GetComponentInChildren<PlayerHealth>();
    }

    public WeaponSoundsOrigin WeaponSoundOrigin;

    public void Start()
    {
        Events.instance.DamageDealtByPlayer += AddTotalDamage;

        currentMovementForce =  normalMovementForce;

        _firstPersonActive = true;
        readyToJump = true;
        _playerCol.isTrigger = false;
        _playerRB.freezeRotation = true;
        lockMovement = _lockRotation = false;
        _holstered = _holstering = false;
        _playerCol.isTrigger = false;

        _weaponIndex = 0;
        _weaponSwapDuration = 0.125f;
        _timeSinceLastDash = _dashCooldown;
        _weaponDirection = 1;
        _speedMultiplier = 1;

        _meleeBaseDamage = 20f;
        _meleeDistance = 30f;
        _meleeKnockback = 1000f;
        _timeSinceLastMelee = 0;

        _dead = false;
        _totalDamageDealtByPlayer = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vengeanceRTPC.SetGlobalValue(0);

        InitWeapons();
    }

    private void AddTotalDamage(float dmg)
    {
        _totalDamageDealtByPlayer += dmg;
    }

    public void Update()
    {
        if (!_dead)
        {
            HandleMouseClickInput();
            HandleKeyboardInput();
            HandleTools();
            Look();
            _timeSinceLastDash += Time.deltaTime;
            _timeSinceLastMelee += Time.deltaTime;
        }
    }
    public void FixedUpdate()
    {
        if(!lockMovement) HandleMovement();
    }

    private void InitWeapons()
    {
        _weapons = new List<AWeapon>();
        foreach (GameObject wep in _weaponsToSpawn)
        {
            GameObject wepObj = Instantiate(wep, _armRotation, false);
            wepObj.SetActive(false);
            AWeapon wepScript = wepObj.GetComponentInChildren<AWeapon>();
            _weapons.Add(wepScript);
        }
        _weaponIndex = 0;
        SetWeaponActive(_weaponIndex);
        _pUI._toolbar.SpawnToolIcons(_weapons);
    }
    //  Input

    private void HandleMouseClickInput()
    {
        if(_weapons[_weaponIndex] && !_holstering && !_holstered) _weapons[_weaponIndex].TryFire();
    }
    private void HandleKeyboardInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        float tiltLerpTime = 2.5f * Time.deltaTime;

        if (horizontalInput < 0)
        {
            tiltAngle = Mathf.Lerp(tiltAngle, maxTiltAngle, tiltLerpTime);
        }
        else if(horizontalInput > 0)
        {
            tiltAngle = Mathf.Lerp(tiltAngle, -maxTiltAngle, tiltLerpTime);
        }
        else
        {
            tiltAngle = Mathf.Lerp(tiltAngle, 0, tiltLerpTime);
        }

        float forwardLerpTime = 1f * Time.deltaTime;
        forwardInput = Input.GetAxisRaw("Vertical");
        if (forwardInput < 0)
        {
            weaponForwardOffset = Mathf.Lerp(weaponForwardOffset, maxForwardWeaponOffset, forwardLerpTime);
        }
        else if (forwardInput > 0)
        {
            weaponForwardOffset = Mathf.Lerp(weaponForwardOffset, -maxForwardWeaponOffset, forwardLerpTime);
        }
        else
        {
            weaponForwardOffset = Mathf.Lerp(weaponForwardOffset, 0, forwardLerpTime);
        }
        weaponForwardOffsetGameObject.transform.localPosition = new Vector3(0, 0, weaponForwardOffset);

        verticalInput = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            //speedRPTC.SetGlobalValue(15);
            //lowpassfilterRPC.SetGlobalValue(50);
            verticalInput = 1;
        }
        else
        {
            //speedRPTC.SetGlobalValue(1);
            //lowpassfilterRPC.SetGlobalValue(1);
        }
        if (Input.GetKey(KeyCode.C)) verticalInput = -1;




        if (Input.GetKeyDown(KeyCode.F)) TryMelee();

        if (Input.GetKey(KeyCode.LeftControl) && _weapons[_weaponIndex].CanReverseWeapon && !_weapons[_weaponIndex].Reloading) ReverseWeapon(true);
        else ReverseWeapon(false);

        jumping = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.LeftShift) &&  _timeSinceLastDash > _dashCooldown)
        {
            if (Input.GetKey(KeyCode.W))     Dash(0);
            if (Input.GetKey(KeyCode.A))     Dash(1);
            if (Input.GetKey(KeyCode.S))     Dash(2);
            if (Input.GetKey(KeyCode.D))     Dash(3);
            if (Input.GetKey(KeyCode.Space)) Dash(4);
            if (Input.GetKey(KeyCode.C))     Dash(5);
        }
    }

    //  Melee

    private void TryMelee()
    {
        if (_timeSinceLastMelee >= _meleeCooldown)
        {
            _timeSinceLastMelee = 0;
            StartCoroutine(Melee());
        }
    }

    private IEnumerator Melee()
    {
        HM.RotateLocalTransformToAngle(_melee.transform, new Vector3(0, 0, 0));
        RaycastHit hit = HM.RaycastAtPosition(_FPSLayerCam.transform.position, _FPSLayerCam.transform.forward, _meleeDistance, LayerMask.GetMask("Enemy"));
        if (hit.collider)
        {
            AkSoundEngine.PostEvent("Play_KickHitSound", gameObject);
            if (hit.collider.GetComponentInChildren<AEnemy>())
            {
                AEnemy enemy = hit.collider.GetComponentInChildren<AEnemy>();
                float finalMeleeDamage = _meleeBaseDamage;
                finalMeleeDamage *= Mathf.Max(1, _playerRB.velocity.magnitude / 15f); // do at minimum base damage
                finalMeleeDamage = Mathf.Min(_meleeBaseDamage * 3, finalMeleeDamage); // do maximum of 3 times the damage
                if (enemy._frozen) finalMeleeDamage *= 2; //double damage if frozen
                hit.collider.GetComponentInChildren<AEnemy>().TakeDamage(finalMeleeDamage);
                _playerRB.velocity = Vector3.zero;
                ApplyKnockback(_meleeKnockback);
            }
            else
            {
                ApplyKnockback(_meleeKnockback * 0.25f);
            }
        }
        else
        {
            AkSoundEngine.PostEvent("Play_KickMissedSound", gameObject);
        }
        

        yield return new WaitForSeconds(_meleeCooldown * 0.25f);
        HM.RotateLocalTransformToAngle(_melee.transform, new Vector3(30, 0, 0));
    }

    public void SetVengeanceMode(bool on)
    {
        if (on)
        {
            currentMovementForce = vengeanceMovementForce;
            maxMoveSpeed = 21;
            vengeanceRTPC.SetGlobalValue(50);
            // update counter movement as well
            //counterMovement;
        }
        else
        {
            currentMovementForce = normalMovementForce;
            maxMoveSpeed = 7;
            vengeanceRTPC.SetGlobalValue(0);
            // update counter movement as well
        }
    }


    private void Dash(int direction)
    {
        _playerRB.velocity = Vector3.zero;
        if (direction == 0) _playerRB.AddForce(_playerCam.transform.parent.forward  *      _dashForceMultiplier * currentMovementForce); // W
        if (direction == 1) _playerRB.AddForce(_playerCam.transform.parent.right    * -1 * _dashForceMultiplier * currentMovementForce); // A
        if (direction == 2) _playerRB.AddForce(_playerCam.transform.parent.forward  * -1 * _dashForceMultiplier * currentMovementForce); // S
        if (direction == 3) _playerRB.AddForce(_playerCam.transform.parent.right    *      _dashForceMultiplier * currentMovementForce); // D
        if (direction == 4) _playerRB.AddForce(transform.up                  *      _dashForceMultiplier * currentMovementForce); // Space = Swim Up
        if (direction == 5) _playerRB.AddForce(transform.up                  * -1 * _dashForceMultiplier * currentMovementForce); // C = Swim Down
        _timeSinceLastDash = 0;
        AkSoundEngine.PostEvent("Play_DashSound", gameObject);
        InitiateLowDrag();
    }

    public void InitiateLowDrag()
    {
        //if (_dragCoroutine != null) StopCoroutine(LowDragMode());
        _dragCoroutine = StartCoroutine(LowDragMode());
    }

    public IEnumerator LowDragMode()
    {
        //  Show Slipstream Deja Vu Eurobeat SFX you know the ones

        _floatingDrag = 0;
        for (int i = 0; i < 60; i++)
        {
            _floatingDrag = 3 * (i / 60f);
            _pUI.SpeedLinesUI(true, 0.5f * (1 - (i/60f)));
            yield return new WaitForFixedUpdate();
        }
        _floatingDrag = 3;
        _pUI.SpeedLinesUI(false, 0.5f);
        _dragCoroutine = null;
        yield return null;
    }

    private void ReverseWeapon(bool reversed)
    {
        if(reversed)
        {
            reverseWeaponRTPC.SetGlobalValue(100);
            _weaponDirection = -1;
            WeaponSoundOrigin.transform.localPosition = new Vector3(0, 0, -5);
            HM.RotateLocalTransformToAngle(_armRotation, new Vector3(_armRotation.localRotation.eulerAngles.x, 180, _arm.localRotation.eulerAngles.z));
        }
        else
        {
            reverseWeaponRTPC.SetGlobalValue(0);
            _weaponDirection = 1;
            WeaponSoundOrigin.transform.localPosition = new Vector3(0, 0, 1);
            HM.RotateLocalTransformToAngle(_armRotation, new Vector3(_armRotation.localRotation.eulerAngles.x, 0, _arm.localRotation.eulerAngles.z));
        }
    }

    private void Look()
    {
        if (_lockRotation) return;

        float mouseX = Input.GetAxis("Mouse X") * _currentSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _currentSensitivity * Time.fixedDeltaTime;

        //Find current look rotation
        Vector3 rot = _playerCam.transform.parent.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations

        // actual rotation of player
        _playerCam.transform.parent.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        _orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

        //  tilt cameras for effect
        _playerCam.transform.localRotation = Quaternion.Euler(0, 0, tiltAngle);
        _FPSLayerCam.transform.localRotation = Quaternion.Euler(0, 0, tiltAngle);

        //_playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, tiltAngle);
        //_FPSLayerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, tiltAngle);
    }

    // Movement


    private void HandleMovement()
    {
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

        if (_playerRB.velocity.magnitude > maxSpeed * _speedMultiplier)
        {
            horizontalInput = 0;
            forwardInput = 0;
            verticalInput = 0;
        }

        //Apply forces to move player
        if (!_grounded) //if floating, move towards camera, otherwise, try walking on the seabed
        {
            _playerRB.useGravity = false;
            _playerRB.drag = _floatingDrag;
            _playerRB.AddForce((
                _playerCam.transform.parent.forward * forwardInput
                + Vector3.up * verticalInput
                + _playerCam.transform.parent.right * horizontalInput)
                * currentMovementForce * Time.deltaTime * _speedMultiplier);
        }
        else
        {
            _playerRB.useGravity = true;
            _playerRB.drag = _walkingDrag;
            _playerRB.AddForce(_orientation.transform.forward * forwardInput * currentMovementForce * Time.deltaTime * _speedMultiplier);
            _playerRB.AddForce(_orientation.transform.right * horizontalInput * currentMovementForce * Time.deltaTime * _speedMultiplier);
        }
    }
    public void InitPlayerDeath()
    {
        if (_dead) return;
        _dead = true;
        lockMovement = true;
        _lockRotation = true;
        AkSoundEngine.PostEvent("Play_PlayerDies", gameObject);
        REF.Dialog.StartDialogue(Resources.Load("Dialogue/Conversations/Game Over Conversation") as ConversationScriptObj, false, false);
        REF.PlayerUI.InitiateDeathUI();
    }
    private void Jump()
    {
        if (_grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            _playerRB.AddForce(Vector2.up * jumpForce * 1.5f);
            _playerRB.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = _playerRB.velocity;
            if (_playerRB.velocity.y < 0.5f)
                _playerRB.velocity = new Vector3(vel.x, 0, vel.z);
            else if (_playerRB.velocity.y > 0)
                _playerRB.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

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
            _playerRB.AddForce(currentMovementForce * _orientation.transform.right * Time.deltaTime * -mag.x * counterMovementModifier);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            _playerRB.AddForce(currentMovementForce * _orientation.transform.forward * Time.deltaTime * -mag.y * counterMovementModifier);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(_playerRB.velocity.x, 2) + Mathf.Pow(_playerRB.velocity.z, 2))) > maxMoveSpeed)
        {
            float fallspeed = _playerRB.velocity.y;
            Vector3 n = _playerRB.velocity.normalized * maxMoveSpeed;
            _playerRB.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    //  TOOLS

    private void HandleTools()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)  NextWeapon();
            else if (Input.mouseScrollDelta.y < 0) PreviousTool();
        }
        else HandleToolIndex();
    }
    private void HandleToolIndex()
    {
        int oldToolIndex = _weaponIndex;
        bool swap = false;

        if (_holstering) return;

        //  Check if we have any inputs at all
        if (Input.anyKeyDown)
        {
            //  get any input and convert the ascii range into an int
            int inputToolIndex = Convert.ToInt32(_lastHitKey) - 49; //49 = Alpha1 => 49 - 49 = 0 => Our first tool in the list 
            if (inputToolIndex < 0 || inputToolIndex > 9) return; // the numbers in the ascii range are between 48 (= 0) and 57 (= 9), so only accept inputs between 1 and 9, else return
            if (inputToolIndex != _weaponIndex) swap = true; //if we are not selecting the same tool as last time, swap
            _weaponIndex = inputToolIndex; //now overwrite the tool index to compare with the next input at a later time

            if (_weaponIndex == oldToolIndex) return; //dont do anything if we have the same tool selected
            if (_weaponIndex >= _weapons.Count) //dont go out of bounds
            {
                _weaponIndex = oldToolIndex;
                return;
            }
            REF.PlayerUI.SelectTool(_weaponIndex);
            if (swap)
            {
                StopAllCoroutines();
                StartCoroutine(SwapAnimation());
            }
            else
            {
                if (_holstered || oldToolIndex != _weaponIndex)
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

    public void SelectToolFromToolbar(int index)
    {
        if (index == _weaponIndex) return;
        _weaponIndex = index;
        StartCoroutine(SwapAnimation());
    }

    private IEnumerator HolsterTool(bool hideUI)
    {
        _holstering = true;
        _holstered = false;
        HM.RotateLocalTransformToAngle(_arm, new Vector3(0, 0, 0));

        //  1s duration = 50 * 0.01 Real Time Second Waits = 0.5s for Holster
        float duration = _weaponSwapDuration * 50;
        for (int i = 0; i < duration; i++)
        {
            HM.RotateLocalTransformToAngle(_arm, new Vector3(i / duration * (duration * 2f), 0, 0));
            yield return new WaitForSeconds(0.01f);
        }
        _holstered = true;
        _holstering = false;
        if (hideUI) REF.PlayerUI.ChangeToolBarOpacity(false);
    }
    private IEnumerator UnholsterTool(bool showUI)
    {
        _holstering = true;
        _holstered = true;
        SetWeaponActive(_weaponIndex);

        //TODO: add weapon swap sound here!!
        AkSoundEngine.PostEvent(_weapons[_weaponIndex].weaponSwapSoundID, gameObject);

        //  1s duration == 50 * 0.01 Real Time Second Waits = 0.5s for Unholster
        float duration = _weaponSwapDuration * 50;
        for (int i = 0; i < duration; i++)
        {
            HM.RotateLocalTransformToAngle(_arm, new Vector3((duration * 2f) - i / duration * (duration * 2f), 0, 0));
            yield return new WaitForSeconds(0.01f);
        }
        HM.RotateLocalTransformToAngle(_arm, new Vector3(0, 0, 0));
        _holstered = false;
        _holstering = false;

        if (showUI) REF.PlayerUI.ChangeToolBarOpacity(true);
    }

    private void PreviousTool()
    {
        _weaponIndex--;
        if (_weaponIndex < 0)
        {
            _weaponIndex = _weapons.Count - 1;
        }
        StopAllCoroutines();
        StartCoroutine(SwapAnimation());
    }
    private void NextWeapon()
    {
        _weaponIndex++;
        if (_weaponIndex > _weapons.Count - 1) _weaponIndex = 0;
        StopAllCoroutines();
        StartCoroutine(SwapAnimation());
    }
    private IEnumerator SwapAnimation()
    {
        REF.PlayerUI.SelectTool(_weaponIndex);
        if (!_holstered)
        {
            StartCoroutine(HolsterTool(false));
            yield return new WaitWhile(() => !_holstered);
        }
        StartCoroutine(UnholsterTool(true));
    }
    private void SetWeaponActive(int i)
    {
        if (_weapons.Count == 0) return;
        foreach (AWeapon g in _weapons)
        {
            if (g) g.gameObject.SetActive(false);
        }
        if (_weapons[i]) _weapons[i].gameObject.SetActive(true);
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


    //  Misc

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = _orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(_playerRB.velocity.x, _playerRB.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = _playerRB.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    public void ApplyKnockback(float knockback)
    {
        _playerRB.AddForce(_playerCam.transform.parent.forward * (-1 * _weaponDirection) * knockback);
    }

    private void OnGUI()
    {
        if (Event.current.isKey)
        {
            if (Event.current.keyCode != KeyCode.None) _lastHitKey = Event.current.keyCode;
        }
    }
}