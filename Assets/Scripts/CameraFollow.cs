using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Distancia y altura")]
    public float distancia = 7f;
    public float alturaExtra = 1.5f;

    [Header("Sensibilidad del ratón")]
    public float sensibilidadX = 3f;
    public float sensibilidadY = 2f;

    [Header("Límites verticales")]
    public float anguloMinY = -10f;
    public float anguloMaxY = 60f;

    [Header("Suavizado")]
    public float suavizado = 12f;

    private float _yaw;
    private float _pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        _yaw += Input.GetAxis("Mouse X") * sensibilidadX;
        _pitch -= Input.GetAxis("Mouse Y") * sensibilidadY;
        _pitch = Mathf.Clamp(_pitch, anguloMinY, anguloMaxY);

        Quaternion rotacion = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 puntoOrbita = target.position + Vector3.up * alturaExtra;
        Vector3 posDeseada = puntoOrbita - rotacion * Vector3.forward * distancia;

        transform.position = Vector3.Lerp(transform.position, posDeseada, suavizado * Time.deltaTime);
        transform.LookAt(puntoOrbita);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public float GetYaw() => _yaw;
}
