using UnityEngine;

public class LegacyMove : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float sensitivity = 1f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        transform.Translate(dir * speed * Time.deltaTime);
        
        float mouseX = Input.GetAxis("Mouse X");
        Debug.Log("Mouse X: " + mouseX);
        transform.Rotate(Vector3.up, mouseX * sensitivity * 500f * Time.deltaTime);
    }
}