using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;
using UnityEngine.UI;

namespace KinematicCharacterController.Walkthrough.ClimbingLadders
{
    public enum CharacterState
    {
        Default,
        Climbing,
    }

    public enum ClimbingState
    {
        Anchoring,
        Climbing,
        DeAnchoring
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool ClimbLadder;
        public bool DashDown; // Dash tuþu
    }

    public class MyCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        private bool _wasGrounded = false;

        private bool _hasLanded = false;

        private Animator _animator;

        public AudioClip dashSound;
        public AudioClip jumpSound;
        private AudioSource audioSource;
        private bool _dashSoundPlayed = false;

        private bool _controlsDisabled = false;
        private float _controlsDisabledUntil = 0f;

        [Header("Bounce Settings")]
        [Tooltip("Yere çarptýktan sonra karakterin ne kadar zýplayacaðýný belirler (0-1 arasý). 1 = tam ters yönde ayný hýzda zýplar.")]
        public float BounceFactor = 0.5f;

        [Tooltip("Bounce uygulanabilmesi için gereken minimum düþey hýz (negatif deðer).")]
        public float BounceThreshold = -5f;

        [Tooltip("Bounce sonrasý ivmede yumuþama oraný (damping).")]
        public float BounceDamping = 0.8f;

        [Header("Explosion/Enemy Hit Settings")]
        public float ExplosionImpulse = 20f; // Dilediðiniz þiddet deðeri
        [Header("Explosion/Enemy2 Hit Settings")]
        public float ExplosionImpulse2 = 40f; // Dilediðiniz þiddet deðeri


        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15;
        public float OrientationSharpness = 10;
        public float MaxStableDistanceFromLedge = 5f;
        [Range(0f, 180f)]
        public float MaxStableDenivelationAngle = 180f;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;

        [Header("Dash Settings")]
        public float DashSpeed = 20f;         // Dash sýrasýnda hýz
        public float DashDuration = 0.2f;     // Dash süresi (saniye)
        public float DashCooldown = 1f;       // Dash tekrar kullanmak için bekleme süresi

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public bool AllowDoubleJump = false;
        public bool AllowWallJump = false;
        public float JumpSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Ladder Climbing")]
        public float ClimbingSpeed = 4f;
        public float AnchoringDuration = 0.25f;
        public LayerMask InteractionLayer;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public bool OrientTowardsGravity = false;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;

        // Karakterin o anki durumunu tutar (Default, Climbing)
        public CharacterState CurrentCharacterState { get; private set; }

        // Ýç deðiþkenler
        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _doubleJumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;

        // Ladder (týrmanma) deðiþkenleri
        private float _ladderUpDownInput;
        private MyLadder _activeLadder { get; set; }
        private ClimbingState _internalClimbingState;
        private ClimbingState _climbingState
        {
            get { return _internalClimbingState; }
            set
            {
                _internalClimbingState = value;
                _anchoringTimer = 0f;
                _anchoringStartPosition = Motor.TransientPosition;
                _anchoringStartRotation = Motor.TransientRotation;
            }
        }
        private Vector3 _ladderTargetPosition;
        private Quaternion _ladderTargetRotation;
        private float _onLadderSegmentState = 0;
        private float _anchoringTimer = 0f;
        private Vector3 _anchoringStartPosition = Vector3.zero;
        private Quaternion _anchoringStartRotation = Quaternion.identity;
        private Quaternion _rotationBeforeClimbing = Quaternion.identity;

        // DASH ile ilgili deðiþkenler
        private bool _isDashing = false;
        private float _dashTimeLeft = 0f;
        private float _lastDashTime = -999f;
        private Vector3 _dashDirection = Vector3.zero;

        public Slider hPBar;
        private void Start()
        {

            _animator = GetComponent<Animator>();

            // Motor referansýný al (Inspector'da atalý ise bu satýr gereksiz, ama güvenli)
            Motor = GetComponent<KinematicCharacterMotor>();

            // KinematicCharacterMotor'a bu script'i tanýt
            Motor.CharacterController = this;

            // Ýlk durum Default olsun
            TransitionToState(CharacterState.Default);

            audioSource = GetComponent<AudioSource>();
        }
  
        /// <summary> State deðiþimi </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary> State'e girerken </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        _rotationBeforeClimbing = Motor.TransientRotation;

                        // Týrmanma baþlarken çarpýþma çözümlemeyi kapat
                        Motor.SetMovementCollisionsSolvingActivation(false);
                        Motor.SetGroundSolvingActivation(false);
                        _climbingState = ClimbingState.Anchoring;

                        // Merdiven konum/rotasyon hesapla
                        if (_activeLadder)
                        {
                            _ladderTargetPosition = _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition, out _onLadderSegmentState);
                            _ladderTargetRotation = _activeLadder.transform.rotation;
                        }
                        break;
                    }
            }
        }

        /// <summary> State'ten çýkarken </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        // Týrmanma bitince çarpýþma çözümlemeyi aç
                        Motor.SetMovementCollisionsSolvingActivation(true);
                        Motor.SetGroundSolvingActivation(true);
                        break;
                    }
            }
        }

        /// <summary> MyPlayer vb. her kare input gönderir </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {

            // Eðer kontroller devre dýþýysa, tüm inputlarý sýfýrla
            if (_controlsDisabled)
            {
                inputs.MoveAxisForward = 0f;
                inputs.MoveAxisRight = 0f;
                inputs.JumpDown = false;
                inputs.CrouchDown = false;
                inputs.CrouchUp = false;
                inputs.ClimbLadder = false;
                inputs.DashDown = false;

                // Eðer 1 saniye geçtiyse kontrolleri tekrar aç
                if (Time.time >= _controlsDisabledUntil)
                {
                    _controlsDisabled = false;
                }
            }

            // Týrmanma
            _ladderUpDownInput = inputs.MoveAxisForward;
            if (inputs.ClimbLadder)
            {
                // Merdiven var mý diye overlap
                if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, InteractionLayer, QueryTriggerInteraction.Collide) > 0)
                {
                    // Örnek: ilk collider'ý al
                    if (_probedColliders[0] != null)
                    {
                        MyLadder ladder = _probedColliders[0].GetComponent<MyLadder>();
                        if (ladder)
                        {
                            if (CurrentCharacterState == CharacterState.Default)
                            {
                                _activeLadder = ladder;
                                TransitionToState(CharacterState.Climbing);
                            }
                            else if (CurrentCharacterState == CharacterState.Climbing)
                            {
                                // Týrmanmayý býrak
                                _climbingState = ClimbingState.DeAnchoring;
                                _ladderTargetPosition = Motor.TransientPosition;
                                _ladderTargetRotation = _rotationBeforeClimbing;
                            }
                        }
                    }
                }
            }

            // Hareket inputu
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // Kamera yönü
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            // State'e göre
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Yerde/havada normal hareket & bakýþ
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        _lookInputVector = cameraPlanarDirection;

                        // Jump
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        // Crouch
                        if (inputs.CrouchDown)
                        {
                            _shouldBeCrouching = true;
                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                                MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                            }
                        }
                        else if (inputs.CrouchUp)
                        {
                            _shouldBeCrouching = false;
                        }

                        // Dash giriþi (Default durumunda)
                        if (inputs.DashDown)
                        {
                            if (Time.time >= _lastDashTime + DashCooldown)
                            {
                                _isDashing = true;
                                _dashTimeLeft = DashDuration;
                                _lastDashTime = Time.time;

                                // Dash yönü: önce hareket inputu varsa onu al, yoksa bakýþ yönü
                                Vector3 dashInput = cameraPlanarRotation * new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward).normalized;
                                if (dashInput.sqrMagnitude < 0.1f)
                                {
                                    dashInput = _lookInputVector;
                                }
                                _dashDirection = dashInput.normalized;
                            }
                        }

                        break;
                    }
                case CharacterState.Climbing:
                    {
                        // Týrmanma inputu
                        // ...
                        // Burada da dash yapmak isterseniz, ekleyin:
                        if (inputs.DashDown)
                        {
                            if (Time.time >= _lastDashTime + DashCooldown)
                            {
                                _isDashing = true;
                                _dashTimeLeft = DashDuration;
                                _lastDashTime = Time.time;

                                // Dash yönü: týrmanýrken de inputa göre
                                Vector3 dashInput = cameraPlanarRotation * new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward).normalized;
                                if (dashInput.sqrMagnitude < 0.1f)
                                {
                                    dashInput = cameraPlanarDirection; // týrmanýrken bakýþ yönü
                                }
                                _dashDirection = dashInput.normalized;
                            }
                        }
                        break;
                    }
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                        {
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(
                                Motor.CharacterForward,
                                _lookInputVector,
                                1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }
                        if (OrientTowardsGravity)
                        {
                            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
                        }
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                if (_activeLadder)
                                {
                                    currentRotation = _activeLadder.transform.rotation;
                                }
                                break;

                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                float anchorT = Mathf.Clamp01(_anchoringTimer / AnchoringDuration);
                                currentRotation = Quaternion.Slerp(_anchoringStartRotation, _ladderTargetRotation, anchorT);

                                // Eðer de-anchoring sona ermiþse rotasyonu sabitle
                                if (_anchoringTimer >= AnchoringDuration && _climbingState == ClimbingState.DeAnchoring)
                                {
                                    currentRotation = _rotationBeforeClimbing;
                                }

                                break;
                        }
                        break;
                    }
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // 1) Dash kontrolü
                        if (_isDashing)
                        {
                            currentVelocity = _dashDirection * DashSpeed;
                            _dashTimeLeft -= deltaTime;

                            // >>>>>> Dash sesini sadece bir kere çal
                            if (!_dashSoundPlayed && dashSound != null)
                            {
                                audioSource.PlayOneShot(dashSound);
                                _dashSoundPlayed = true;
                            }

                            if (_dashTimeLeft <= 0f)
                            {
                                _isDashing = false;
                                _dashSoundPlayed = false; // sonraki dash için sýfýrla
                            }

                            return; // dash bitene kadar normal hareketi pas geç
                        }

                        // 2) Normal yerde/havada hareket
                        Vector3 targetMovementVelocity = Vector3.zero;
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized
                                                      * _moveInputVector.magnitude;
                            targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        else
                        {
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(
                                        Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
                                        Motor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                                }
                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                            }

                            currentVelocity += Gravity * deltaTime;
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }

                        

                        // Jump
                        {
                            _jumpedThisFrame = false;
                            _timeSinceJumpRequested += deltaTime;
                            if (_jumpRequested)
                            {
                                if (AllowDoubleJump)
                                {
                                    if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                                    {
                                        Motor.ForceUnground(0.1f);
                                        currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                        _jumpRequested = false;
                                        _doubleJumpConsumed = true;
                                        _jumpedThisFrame = true;
                                    }
                                }

                                if (_canWallJump ||
                                    (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                                     || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
                                {
                                    Vector3 jumpDirection = Motor.CharacterUp;
                                    if (_canWallJump)
                                    {
                                        jumpDirection = _wallJumpNormal;
                                    }
                                    else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                    {
                                        jumpDirection = Motor.GroundingStatus.GroundNormal;
                                    }
                                    Motor.ForceUnground(0.1f);
                                    currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                    _jumpRequested = false;
                                    _jumpConsumed = true;
                                    _jumpedThisFrame = true;

                                    if (jumpSound != null)
                                    {
                                        audioSource.PlayOneShot(jumpSound);
                                    }
                                }
                            }
                          
                            _canWallJump = false;
                        }

                        // AddVelocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }

                case CharacterState.Climbing:
                    {
                        // Dash týrmanma sýrasýnda da çalýþsýn mý?
                        if (_isDashing)
                        {
                            currentVelocity = _dashDirection * DashSpeed;
                            _dashTimeLeft -= deltaTime;
                            if (_dashTimeLeft <= 0f)
                            {
                                _isDashing = false;
                            }
                            return;
                        }

                        // Týrmanma velocity
                        currentVelocity = Vector3.zero;
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                if (_activeLadder)
                                {
                                    Vector3 climbDirection = _activeLadder.WorldClimbDirection;
                                    currentVelocity = (_ladderUpDownInput * climbDirection).normalized * ClimbingSpeed;
                                }
                                break;
                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                Vector3 tmpPosition = Vector3.Lerp(_anchoringStartPosition, _ladderTargetPosition, (_anchoringTimer / AnchoringDuration));
                                currentVelocity = Motor.GetVelocityForMovePosition(Motor.TransientPosition, tmpPosition, deltaTime);
                                break;
                        }
                        break;
                    }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {

                        // Jump timing
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }
                        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                        {
                            if (!_jumpedThisFrame)
                            {
                                _doubleJumpConsumed = false;
                                _jumpConsumed = false;
                            }
                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            _timeSinceLastAbleToJump += deltaTime;
                        }

                        bool isNowGrounded = Motor.GroundingStatus.IsStableOnGround;

                        if (!isNowGrounded)
                        {
                            // Karakter havadaysa bayraðý sýfýrla, böylece bir sonraki iniþte tetiklenebilir
                            _hasLanded = false;
                        }
                        else
                        {
                            // Eðer karakter yerde ve henüz landing tetiklenmemiþse, tetikleyelim
                            if (!_hasLanded)
                            {
                                _animator.SetTrigger("Land");
                                _hasLanded = true;
                            }
                        }

                        // Crouch'tan çýkma
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Ayakta duracak boyuta geç
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                // Engel varsa geri küçül
                                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                            }
                            else
                            {
                                // Engel yoksa normal boy
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }

                case CharacterState.Climbing:
                    {
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                if (_activeLadder)
                                {
                                    _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition, out _onLadderSegmentState);
                                    if (Mathf.Abs(_onLadderSegmentState) > 0.05f)
                                    {
                                        // Ladder'ýn üstü veya altý
                                        _climbingState = ClimbingState.DeAnchoring;
                                        if (_onLadderSegmentState > 0)
                                        {
                                            _ladderTargetPosition = _activeLadder.TopReleasePoint.position;
                                            _ladderTargetRotation = _activeLadder.TopReleasePoint.rotation;
                                        }
                                        else
                                        {
                                            _ladderTargetPosition = _activeLadder.BottomReleasePoint.position;
                                            _ladderTargetRotation = _activeLadder.BottomReleasePoint.rotation;
                                        }
                                    }
                                }
                                break;
                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                if (_anchoringTimer >= AnchoringDuration)
                                {
                                    if (_climbingState == ClimbingState.Anchoring)
                                    {
                                        _climbingState = ClimbingState.Climbing;
                                    }
                                    else
                                    {
                                        // Týrmanma bitti
                                        TransitionToState(CharacterState.Default);
                                    }
                                }
                                _anchoringTimer += deltaTime;
                                break;
                        }
                        break;
                    }
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {


            _animator.SetTrigger("Land");



        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // Ground solving’i geçici kapatýn
                _controlsDisabled = true;
                _controlsDisabledUntil = Time.time + 1f;
                Debug.Log("Enemy çarpmasý: Kontroller 1 saniyeliðine devre dýþý býrakýldý.");

                Motor.ForceUnground(0.5f);  // ForceUnground süresini de artýrýn

                Vector3 pushDirection = (hitNormal).normalized;
                float impulse = ExplosionImpulse;
                AddVelocity(pushDirection * impulse);
                Debug.Log("Enemy çarpýþmasý: Uygulanan impulse = " + (pushDirection * impulse));
            }
            if (hitCollider.CompareTag("Enemy2"))
            {
                // Ground solving’i geçici kapatýn
                _controlsDisabled = true;
                _controlsDisabledUntil = Time.time + 1f;
                Debug.Log("Enemy çarpmasý: Kontroller 1 saniyeliðine devre dýþý býrakýldý.");

                Motor.ForceUnground(0.5f);  // ForceUnground süresini de artýrýn

                Vector3 pushDirection = (hitNormal).normalized;
                float impulse = ExplosionImpulse;
                AddVelocity(pushDirection * impulse);
                Debug.Log("Enemy çarpýþmasý: Uygulanan impulse = " + (pushDirection * impulse));
            }


            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Duvar zýplamasý
                        if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                        {
                            _canWallJump = true;
                            _wallJumpNormal = hitNormal;
                        }
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        // Týrmanýrken de bir þey yapmak isterseniz
                        break;
                    }
            }
        }

        public void AddVelocity(Vector3 velocity)
        {
            // Default veya Climbing'de de velocity ekleyebilirsiniz
            _internalVelocityAdd += velocity;
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
                                              Vector3 atCharacterPosition, Quaternion atCharacterRotation,
                                              ref HitStabilityReport hitStabilityReport)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}
