using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// WASD / Arrow 키로 맵(또는 지정한 Transform)을 기울이는 스크립트입니다.
/// 새 Input System을 사용하여 호환성 문제를 해결했습니다.
/// - W: 앞쪽으로 기울이기 (Vertical +)
/// - S: 뒤쪽으로 기울이기 (Vertical -)
/// - A: 왼쪽으로 기울이기 (Horizontal -)
/// - D: 오른쪽으로 기울이기 (Horizontal +)
/// 인스펙터에서 `mapRoot`, `maxTiltAngle`, `tiltSpeed`, `smoothSpeed` 등을 조정하세요.
/// </summary>
public class MapTilt : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("기울일 대상 Transform. 비어있으면 이 컴포넌트가 붙은 GameObject를 사용합니다.")]
    public Transform mapRoot;

    [Header("Tilt Settings")]
    [Tooltip("최대 기울기 각도(도) — X축(앞/뒤)과 Z축(좌/우)에 동일하게 적용됩니다.")]
    public float maxTiltAngle = 12f;
    [Tooltip("입력에 대한 속도(각도/초). 값이 클수록 입력에 빠르게 반응합니다.")]
    public float tiltSpeed = 60f;
    [Tooltip("스무딩 계수. 값이 클수록 더 즉시 회전합니다.")]
    public float smoothSpeed = 8f;

    [Header("Inversion")]
    public bool invertVertical = false;
    public bool invertHorizontal = false;

    private InputAction moveAction;
    private Vector2 currentAngles = Vector2.zero; // x = pitch, y = roll
    private Vector2 targetAngles = Vector2.zero;
    Rigidbody mapRootRb;

    void Awake()
    {
        if (mapRoot == null) mapRoot = transform;

        // Create InputAction for movement (WASD / Arrow keys)
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // Ensure mapRoot has a kinematic Rigidbody so collider movement is handled by physics
        mapRootRb = mapRoot.GetComponent<Rigidbody>();
        if (mapRootRb == null)
        {
            // add a kinematic Rigidbody to the map root so MoveRotation can be used
            mapRootRb = mapRoot.gameObject.AddComponent<Rigidbody>();
            mapRootRb.isKinematic = true;
            mapRootRb.interpolation = RigidbodyInterpolation.Interpolate;
            mapRootRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // Prevent position changes - only rotation
            mapRootRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }

        // Initialize from current local rotation
        Vector3 e = mapRoot.localEulerAngles;
        e = NormalizeEuler(e);
        currentAngles.x = e.x;
        currentAngles.y = e.z;
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        ReadInput();
        // Smooth towards targetAngles
        currentAngles = Vector2.Lerp(currentAngles, targetAngles, Time.deltaTime * smoothSpeed);
    }

    void FixedUpdate()
    {
        ApplyRotation(currentAngles);
    }

    void ReadInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (invertHorizontal) input.x = -input.x;
        if (invertVertical) input.y = -input.y;

        // Map: vertical controls X (pitch), horizontal controls Z (roll)
        float targetX = Mathf.Clamp(input.y * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        float targetZ = Mathf.Clamp(-input.x * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

        targetAngles.x = Mathf.MoveTowardsAngle(targetAngles.x, targetX, tiltSpeed * Time.deltaTime);
        targetAngles.y = Mathf.MoveTowardsAngle(targetAngles.y, targetZ, tiltSpeed * Time.deltaTime);
    }

    void ApplyRotation(Vector2 angles)
    {
        // angles.x => rotation around X (pitch), angles.y => rotation around Z (roll)
        Quaternion localTarget = Quaternion.Euler(angles.x, 0f, angles.y);
        if (mapRootRb != null)
        {
            // Rigidbody.MoveRotation expects world rotation. Convert local -> world if necessary.
            Quaternion worldTarget = (mapRoot.parent != null) ? mapRoot.parent.rotation * localTarget : localTarget;
            mapRootRb.MoveRotation(worldTarget);
        }
        else
        {
            mapRoot.localRotation = localTarget;
        }
    }

    // normalize euler from 0..360 to -180..180 style for smooth interpolation start
    Vector3 NormalizeEuler(Vector3 e)
    {
        e.x = (e.x > 180f) ? e.x - 360f : e.x;
        e.y = (e.y > 180f) ? e.y - 360f : e.y;
        e.z = (e.z > 180f) ? e.z - 360f : e.z;
        return e;
    }

    /// <summary>
    /// 외부에서 기울기 초기화(0,0) 호출용
    /// </summary>
    public void ResetTilt()
    {
        targetAngles = Vector2.zero;
        currentAngles = Vector2.zero;
        ApplyRotation(currentAngles);
    }

    void OnValidate()
    {
        if (maxTiltAngle < 0f) maxTiltAngle = 0f;
        if (smoothSpeed < 0f) smoothSpeed = 0f;
        if (tiltSpeed < 0f) tiltSpeed = 0f;
    }
}
