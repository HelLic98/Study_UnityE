using UnityEngine;

/// <summary>
/// Rigidbody 기반 구체가 '미끄러지지 않고' 자연스럽게 굴러가도록
/// 선형 속도에 맞춰 각속도를 계산해 적용합니다.
/// - FixedUpdate에서 접지 노멀과 접촉 방향의 속도를 사용하여
///   무미끄럼(rolling without slipping) 각속도(omega)를 계산합니다.
/// - Rigidbody.angularVelocity를 부드럽게 보간해서 강제 회전으로 인한
///   이질감을 완화합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RollingController : MonoBehaviour
{
    [Tooltip("자동으로 Rigidbody를 가져옵니다.")]
    public Rigidbody rb;

    [Tooltip("구의 반지름(m). 비워두면 SphereCollider에서 자동 계산합니다.")]
    public float radius = 0f;

    [Tooltip("땅과 닿는 것으로 간주할 최대 거리 (radius + offset)")]
    public float groundCheckOffset = 0.05f;

    [Tooltip("각속도 보간 속도. 값이 클수록 즉시 각속도가 목표값으로 맞춰집니다.")]
    public float angularLerp = 8f;

    [Tooltip("지면으로 인식할 레이어. 기본은 모든 레이어.")]
    public LayerMask groundLayers = ~0;

    [Tooltip("속도가 이 값보다 작으면 회전을 0으로 점진적으로 줄입니다.")]
    public float stopVelocityThreshold = 0.05f;

    SphereCollider sc;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Improve collision detection when object moves fast
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        sc = GetComponent<SphereCollider>();
        if (radius <= 0f && sc != null)
        {
            // account for lossy scale on X (assume uniform scale)
            radius = sc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Raycast down to detect ground normal
        Ray ray = new Ray(rb.worldCenterOfMass, -transform.up);
        float dist = radius + groundCheckOffset;
        if (Physics.Raycast(ray, out RaycastHit hit, dist, groundLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 normal = hit.normal.normalized;

            // Tangential component of linear velocity (remove normal component)
            Vector3 v = rb.linearVelocity;
            Vector3 vTangent = v - Vector3.Dot(v, normal) * normal;

            // For pure rolling without slipping: omega = (normal x vTangent) / r
            // (units: (m/s) / m => 1/s (rad/s))
            Vector3 omegaTarget = Vector3.Cross(normal, vTangent) / Mathf.Max(radius, 1e-6f);

            // If speed is tiny, reduce target to zero to avoid tiny jitter
            if (vTangent.sqrMagnitude < stopVelocityThreshold * stopVelocityThreshold)
            {
                omegaTarget = Vector3.zero;
            }

            // Smoothly lerp current angular velocity toward target
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, omegaTarget, Mathf.Clamp01(angularLerp * Time.fixedDeltaTime));
        }
        else
        {
            // Not grounded: let physics handle free rotation (optionally damp small rotations)
        }
    }
}
