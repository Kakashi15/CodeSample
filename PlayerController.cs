using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    #region ConstVariables
    public const string locomotionPlatNTurn180Transition = "LocomotionRun -> PlantNTurnRight180";
    public const string playerDamageAnim = "Damage";
    public const string playerExecutionAnim = "Execution";
    public const string bowEquipedRunStateInfo = "BowEquipedRun";
    public const string horizontalFloatAnimParameter = "Horizontal";
    public const string coverHorizontalAnimParameter = "CoverHorizontal";
    public const string strafeAnimParameter = "Strafe";
    public const string speedAimParameter = "Speed";
    public const string coverTransitionAnimParameter = "CoverTransition";
    public const string coverAimAnimParameter = "CoverAnim";
    public const string coverAnimParameter = "Cover";
    public const string verticalFloatAnimParameter = "Vertical";
    public const string aimLockAnimParameter = "Aim Lock";
    public const string equipBowAnimParameter = "EquipBow";
    public const string drawArrowAnimParameter = "DrawArrow";
    public const string runAnimParameter = "Run";
    public const int AvatarUpperBodyLayerIndex = 4;
    #endregion

    #region Variables
    //Input Variables
    private float horizontalInput;
    private float verticalInput;
    private float angleRotate;
    private float aimlockHorizontal;
    private float aimlockVeritcal;
    private float moveAmount;
    private bool runInput;
    private bool AimLock;
    private bool coverInput;
   
    bool equipBowInput = false;
    bool equipBowParameter = false;

    //Stores move directions from player inputs
    Vector3 moveDir;

    //Stores Look Position, used with IK to aim and look at a point
    private Vector3 lookPos;
    //Stores Look directions  
    private Vector3 lookDir;

    //Player States which blocks all input and actions
    public bool isDead = false;
    private bool inExecution = false;

    //Max offset for player aiming on Y-axis
    public float playerLookYoffsetCap;

    //Rag doll Rigibody and Colliders
    List<Rigidbody> rigidRagdoll = new List<Rigidbody>();
    List<Collider> rigidCollider = new List<Collider>();

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float diagonalMoveSpeed;          //Move speed while aiming and moving diagonally
    [SerializeField] private float rotationSpeed;         //Rotation speed when not equiping bow   
    [SerializeField] private float combatRotationSpeed;   //Rotation speed when bow equiped

    [Header("Combat")]
    [HideInInspector]public bool drawArrowInput;
    [SerializeField] private float health = 100;

    [Header("IkPositions")]
    [SerializeField] private Transform bowString;
    [SerializeField] private float m_LookWeight = 1f;
    [SerializeField] private float m_BodyWeight = 0.25f;
    [SerializeField] private float m_HeadWeight = 0.9f;
    [SerializeField] private float m_EyesWeight = 1f;
    [SerializeField] private float m_ClampWeight = 1f;

    [Header("Player Health UI")]
    [SerializeField] private Slider healthSlider;

    //Current angle between player forward and the lookdir
    float currentLookAngle;

    [SerializeField] private float playerMaxLookAngle = 80f;
    #endregion

    #region Componenets
    private AnimatorStateInfo animatorStateInfo;
    private AnimatorTransitionInfo animatorTransitionInfo;
    private Camera mainCamera;
    private Rigidbody rb;
    private PlayerCombat playerCombat;
    private AvatarMask upperBody;
    private WallCoverDetection wallCoverDetection;
    public Animator anim;
    #endregion

    //current cover position
    private Vector3 coverPosition;
    //Tuples returned from WallcoverDetection
    private Tuple<Transform, Vector3> coverPositionAndDirection;
    private bool inCover;                                            //Player in cover state
    [SerializeField] private float coverPositionLerpSpeed;                     //Lerp speed for cover transition
    private float coverHorizontalInput = 0f;                         //Horizontal input while in cover
    private bool getIntoCover;                                       //Get into cover transition state
    private bool getOutOfCover;                                      //Get out of cover transition state
    private bool coverTransition;                                    //In Cover transition, transition state
    private bool coverAim;                                           //Aiming while in cover State

    //Tracks player direction in cover movement
    private bool coverMovingRight = false;                           
    private bool coverMovingLeft = false;                            

    private Vector3 coverToAimPosition;                              //Player position when aiming from edge of a cover
    private Vector3 backToCoverPosition;                             //Player position when moving back to cover from aiming
    
    private bool isPlayerParallelToCoverWall = false;              

    private Vector3 coverEulerAngles;                                       //Stores cover Euler angle which is the used to orient player correctly on cover wall

    private bool blockAllInput;                                             //State for player input block

    private int frameCount;
    #region UnityMethods        
    void Start () {
        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        wallCoverDetection = GetComponent<WallCoverDetection>();
        playerCombat = GetComponent<PlayerCombat>();
        animatorTransitionInfo = anim.GetAnimatorTransitionInfo(0);
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        Cursor.visible = false;
        InitRagdoll();
    }
	
	
	void Update () {

        if (isDead || inExecution)
            return;
        GetInput();
        PlayerMovement();
        AnimationTransitionLogic();
        PlayerCoverMovement();

        if(healthSlider != null)
            healthSlider.value = health;
        if(health <= 0)
            IsDead();

        UpdateSates();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        CalculateLookAngle();
    }

    private void CalculateLookAngle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            lookPos = hit.point;
        }

        lookDir = lookPos - transform.position;
        lookDir.y = 0;

        currentLookAngle = Vector3.Angle(transform.forward, lookDir);
        currentLookAngle = currentLookAngle / 180f;
    }

    void InitRagdoll()
    {
        Rigidbody[] rigid = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigid.Length; i++)
        {
            if (rigid[i] == rb)
                continue;

            rigidRagdoll.Add(rigid[i]);
            rigid[i].isKinematic = true;

            Collider col = rigid[i].gameObject.GetComponent<Collider>();
            col.isTrigger = true;

            rigidCollider.Add(col);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {

        if (layerIndex == AvatarUpperBodyLayerIndex && anim.GetCurrentAnimatorStateInfo(4).IsName("DrawArrow"))
        {
            anim.SetIKPosition(AvatarIKGoal.RightHand, bowString.position);

            if(anim.GetCurrentAnimatorStateInfo(AvatarUpperBodyLayerIndex).normalizedTime >= 0.5f)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.4f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            }           
        }  

        if(equipBowParameter)
        {
            if (currentLookAngle < playerMaxLookAngle) 
            {
                anim.SetLookAtWeight(m_LookWeight, m_BodyWeight, m_HeadWeight, m_ClampWeight);
                Vector3 IKLookAtDir = transform.position + lookDir;
                IKLookAtDir.y = playerLookYoffsetCap;
                anim.SetLookAtPosition(IKLookAtDir);
            }
        }
    }
    #endregion

    private void AnimationTransitionLogic()
    {
        animatorTransitionInfo = anim.GetAnimatorTransitionInfo(0);
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (animatorTransitionInfo.IsName(locomotionPlatNTurn180Transition) || animatorTransitionInfo.IsName(locomotionPlatNTurn180Transition))
        {
            rb.velocity = Vector3.zero;
        }
    }

    #region CustomMethod
    /// <summary>
    /// Get all player Inputs
    /// </summary>
    void GetInput()
    {
        horizontalInput = !blockAllInput ? Input.GetAxis("Horizontal") : 0;
        verticalInput = !blockAllInput ? Input.GetAxis("Vertical") : 0;
        runInput = !blockAllInput ? Input.GetButton("Fire3") : false;
        equipBowInput = !blockAllInput ? Input.GetKeyUp(KeyCode.Q) : false; 
        drawArrowInput = !blockAllInput ? Input.GetButton("Fire1") : false; 
        AimLock = !blockAllInput ? Input.GetMouseButton(1) : false;
        coverInput = !blockAllInput ? Input.GetKeyDown(KeyCode.Space) : false; 
    }

    /// <summary>
    /// All player movement logic
    /// </summary>
    void PlayerMovement()
    {
        Vector3 stickDirection = new Vector3(horizontalInput, 0, verticalInput);            //Get Movement Input Vector
        Vector3 combinedVector = mainCamera.transform.forward + mainCamera.transform.up;    //Stores a new Vector which is an intermediate between Z and Y axis of the camera
        Vector3 camForward = combinedVector / combinedVector.magnitude;                     //Get the Unit vector which serves as a forward direction

        Vector3 inputDirection = Vector3.Cross(transform.forward, stickDirection);          //Gets Input direction, positive for Right and negative for left

        //Gets the Angel between input direction vector and Players current transform.forward
        float angle = Vector3.Angle(transform.forward, stickDirection) * (inputDirection.y >= 0 ? 1f:-1f) ; 

        angleRotate = angle / 180f;                                                         //Convert radian to degree

        Vector3 v = verticalInput * camForward;                                             //Vertical Input with respect to game's forward vector
        Vector3 h = (horizontalInput * mainCamera.transform.right);                         //Horizontal Input with respect to Camera's right vector                                         

        moveDir = (h + v).normalized;                                                       //Store Intermediate vector for both Vertical and Horizontal Vector 
        moveDir.y = 0;                                                                      //Removes y component from movedir
        moveAmount = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);                 // Amount the player moves 
        moveAmount = Mathf.Clamp01(moveAmount);                                             //Player's Move amount normalized 

        //Movement when not in cover
        if (!inCover)
        {
            if (moveDir == Vector3.zero)
            {
                moveDir = transform.forward;
            }

            if(AimLock == false)
            {
                Quaternion tr = Quaternion.LookRotation(moveDir);
                Quaternion targetRotation;
                targetRotation = Quaternion.Lerp(transform.rotation, tr, Time.deltaTime * moveAmount * rotationSpeed);
                anim.applyRootMotion = false;
                transform.rotation = targetRotation;
            }

            float currentMoveSpeed;

            ///If player is moving daigonaly while aiming
            if(aimlockHorizontal > 0.5 && aimlockVeritcal > 0.5
                || aimlockHorizontal < -0.5 && aimlockVeritcal < -0.5
                || aimlockHorizontal > 0.5 && aimlockVeritcal < -0.5
                || aimlockHorizontal < -0.5 && aimlockVeritcal > 0.5 &&
                AimLock == true)
            {
                currentMoveSpeed = diagonalMoveSpeed;
            }
            else if(runInput)
                currentMoveSpeed = runSpeed;
            else
                currentMoveSpeed = walkSpeed;

            if (moveAmount > 0)
                rb.velocity = moveDir * currentMoveSpeed;
        }
    }

    /// <summary>
    /// All player wall cover logic
    /// </summary>
    void PlayerCoverMovement()
    {
        if (inCover)
        {
            if (coverInput && !wallCoverDetection.coverEdge)
            {
                getOutOfCover = true;
            }

            if(coverInput && wallCoverDetection.coverEdge && verticalInput == 0)
            {
                coverTransition = true;
            }

            if(wallCoverDetection.coverEdge && horizontalInput != 0 && AimLock)
            {
                coverAim = true;
                equipBowInput = false;
                equipBowParameter = true;
                CallUntill(() =>
                {
                    if (horizontalInput > 0 && coverMovingRight)
                    {
                        coverToAimPosition = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                        backToCoverPosition = transform.position;
                    }
                    else if (horizontalInput < 0 && coverMovingLeft)
                    {
                        coverToAimPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                        backToCoverPosition = transform.position;
                    }
                },coverTransition == false);
                
                if(Vector3.Distance(transform.position, coverToAimPosition) > 0.01)
                {
                    transform.position = Vector3.Lerp(transform.position, coverToAimPosition,coverPositionLerpSpeed);
                }
                return;
            }

            if (!coverAim)
            {
                equipBowInput = false;
                equipBowParameter = false;
            }

            if (coverAim && !AimLock)
            {
                equipBowInput = false;
                CoverPositionAndRotationLerp(backToCoverPosition, coverPositionAndDirection.Item1, isPlayerParallelToCoverWall);
            }

            if(coverTransition)
            {
                if(wallCoverDetection.coverTransitionNewPos == null)
                {
                    coverTransition = false;
                    return;
                }

                if(coverMovingRight)
                    CoverPositionAndRotationLerp(wallCoverDetection.coverTransitionNewPos.RightCoverTransform.position, 
                        wallCoverDetection.coverTransitionNewPos.RightCoverTransform, true,true);
                else
                    CoverPositionAndRotationLerp(wallCoverDetection.coverTransitionNewPos.LeftCoverTransform.position,
                        wallCoverDetection.coverTransitionNewPos.LeftCoverTransform, true, true);
            }

            if (!getOutOfCover && !coverTransition && !AimLock && !coverAim)
            {
                Vector3 combinedVector = mainCamera.transform.forward + mainCamera.transform.up;                 
                Vector3 camForward = combinedVector / combinedVector.magnitude;
                transform.eulerAngles = coverEulerAngles;
                if (!wallCoverDetection.coverEdge && horizontalInput > 0)
                {
                    if(Vector3.Dot(camForward, transform.forward) > 0.5)
                    {
                        coverMovingRight = false;
                        coverMovingLeft = true;
                    }
                    else
                    {
                        coverMovingRight = true;
                        coverMovingLeft = false;
                    }
                }
                if (!wallCoverDetection.coverEdge && horizontalInput < 0)
                {
                    if((Vector3.Dot(camForward, transform.forward) <= 0))
                    {
                        coverMovingRight = false;
                        coverMovingLeft = true;
                    }
                    else
                    {
                        coverMovingRight = true;
                        coverMovingLeft = false;
                    }
                }

                if (Vector3.Dot(camForward, transform.forward) >= 0.5)
                {
                    coverHorizontalInput = -horizontalInput;
                }

                else if(Vector3.Dot(camForward,transform.forward) < 0.5)
                {
                    coverHorizontalInput = horizontalInput;
                }

                if (wallCoverDetection.coverEdge && coverMovingLeft)
                    coverHorizontalInput = Mathf.Clamp(coverHorizontalInput, 0, 1);
                else if (wallCoverDetection.coverEdge && coverMovingRight)
                    coverHorizontalInput = Mathf.Clamp(coverHorizontalInput, -1, 0);

                rb.velocity = transform.right * -coverHorizontalInput * walkSpeed;
            }

            else if(!coverTransition && getOutOfCover)
            {
                Vector3 getOutOfCoverRotation;
                if (Vector3.Dot(transform.forward.normalized, coverPositionAndDirection.Item1.forward.normalized) < 0)
                {
                    getOutOfCoverRotation = new Vector3(transform.localEulerAngles.x, coverPositionAndDirection.Item1.transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
                else
                {
                    getOutOfCoverRotation = new Vector3(transform.localEulerAngles.x, coverPositionAndDirection.Item1.transform.localEulerAngles.y + 180, transform.localEulerAngles.z);
                }
                transform.eulerAngles = coverEulerAngles;
                inCover = false;
                getIntoCover = false;
                coverPositionAndDirection = null;
                return;
            }
        }

        if (!inCover)
        {
            if (coverInput)
            {
                coverPositionAndDirection = wallCoverDetection.CoverCheck();
                if (coverPositionAndDirection == null)
                    return;
                transform.position = new Vector3(coverPositionAndDirection.Item2.x, transform.position.y, coverPositionAndDirection.Item2.z);
                isPlayerParallelToCoverWall = Vector3.Dot(transform.forward.normalized, coverPositionAndDirection.Item1.forward.normalized) < 0;
                getIntoCover = true;
                getOutOfCover = false;
            }
            if (getIntoCover)
            {
                CoverPositionAndRotationLerp(coverPositionAndDirection.Item2, coverPositionAndDirection.Item1, isPlayerParallelToCoverWall);
            }
        }
    }

    private void CoverPositionAndRotationLerp(Vector3 position, Transform forward, bool dotProduct, bool iscoverTransition = false)
    {
        blockAllInput = true;
        coverPosition = new Vector3(position.x, transform.position.y, position.z);
        transform.position = Vector3.Lerp(transform.position, coverPosition, coverPositionLerpSpeed * Time.deltaTime);
        if (dotProduct && !iscoverTransition)
        {
            coverEulerAngles = new Vector3(transform.localEulerAngles.x, forward.localEulerAngles.y, transform.localEulerAngles.z);
        }
        else
        {
            coverEulerAngles = new Vector3(transform.localEulerAngles.x, forward.localEulerAngles.y + 180, transform.localEulerAngles.z);
        }

        if(iscoverTransition)
        {
            coverEulerAngles = new Vector3(transform.localEulerAngles.x, forward.localEulerAngles.y - 90, transform.localEulerAngles.z);
        }

        transform.eulerAngles = coverEulerAngles;

        // Check Angle equivalence for all Quadrants(0-360 and 360+ converted to 0-360)
        if (Vector3.Distance(coverPosition, transform.position) < 0.1f)
        {
            inCover = true;
            coverTransition = false;
            coverAim = false;
            blockAllInput = false;
            if (equipBowInput)
                equipBowInput = false;
        }
    }

    /// <summary>
    /// Updates animation parameters
    /// </summary>
    void UpdateAnimationState()
    {
        if (animatorStateInfo.IsName(bowEquipedRunStateInfo))
        {
            drawArrowInput = false;
        }

        anim.SetFloat(horizontalFloatAnimParameter, angleRotate);
        anim.SetFloat(coverHorizontalAnimParameter, coverHorizontalInput);
        anim.SetFloat(speedAimParameter, moveAmount,.05f,Time.deltaTime);
        anim.SetBool(coverTransitionAnimParameter, coverTransition);
        anim.SetBool(coverAimAnimParameter, coverAim);

        if(getIntoCover)
            anim.SetBool(coverAnimParameter, true);

        if (getOutOfCover)
            anim.SetBool(coverAnimParameter, false);

        //BowStates
        Vector3 relativeRot = transform.InverseTransformDirection(moveDir);
        aimlockHorizontal = relativeRot.x;
        aimlockVeritcal = relativeRot.z;
        anim.SetFloat(verticalFloatAnimParameter, aimlockVeritcal);
        anim.SetFloat(strafeAnimParameter, aimlockHorizontal);
        anim.SetBool(aimLockAnimParameter, AimLock);
        anim.SetBool(equipBowAnimParameter, equipBowParameter);
        anim.SetBool(drawArrowAnimParameter, drawArrowInput);
        
        anim.SetBool(runAnimParameter, runInput);      
    }

    public void DoDamage(float damage)
    {
        if (isDead)
            return;
        anim.Play(playerDamageAnim);
        health -= damage;
    }

    void EnableRagdoll()
    {
        for (int i = 0; i < rigidRagdoll.Count; i++)
        {
            rigidRagdoll[i].isKinematic = false;
            rigidCollider[i].isTrigger = false;
        }

        Collider col = rb.GetComponent<Collider>();
        col.isTrigger = true;
        rb.isKinematic = true;

        anim.enabled = false;
    }

    IEnumerable disableAnim()
    {
        yield return new WaitForEndOfFrame();
        anim.enabled = false;
        this.enabled = false;
    }

    void IsDead()
    {
        isDead = true;
        EnableRagdoll();
    }

    void UpdateSates()
    {
        playerCombat.equipBow = equipBowInput;
        if (equipBowInput)
            equipBowParameter = !equipBowParameter;
    }
    /// <summary>
    /// Sets up player position for execution
    /// </summary>
    /// <param name="lookAtTransform">Transform to look at while in execution</param>
    /// <param name="relocatePosition">Position relocation during the execution</param>
    public void PlayExecution(Transform lookAtTransform, Vector3 relocatePosition)
    {
        inExecution = true;
        rb.MovePosition(new Vector3(relocatePosition.x,transform.position.y,relocatePosition.z));
        transform.LookAt(lookAtTransform.position);
        anim.Play(playerExecutionAnim);
    }

    /// <summary>
    /// Call some action for one frame
    /// </summary>
    /// <param name="action"></param>
    void CallForOneFrame(Action action)
    { 
        frameCount++;
        if(frameCount > 1)
        {
            return;
        }
        else
        {
            action.Invoke();
        }
    }

    /// <summary>
    /// Call some action until the condition is met
    /// </summary>
    /// <param name="action"></param>
    /// <param name="condition"></param>
    void CallUntill(Action action, bool condition)
    {
        if(!condition)
        {
            action.Invoke();
        }
    }
    #endregion
}
