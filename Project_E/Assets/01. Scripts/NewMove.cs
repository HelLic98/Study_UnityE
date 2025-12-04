using UnityEngine;
using UnityEngine.InputSystem;

public class NewMove : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    private Vector2 moveInput;

    void Update()
    {
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(dir * speed * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}