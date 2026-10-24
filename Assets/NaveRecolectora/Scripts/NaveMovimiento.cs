using UnityEngine;
using UnityEngine.InputSystem;

// Movimiento de la nave con las flechas
// acelera de a poco y al soltar frena de a poco para que se vea suave
public class NaveMovimiento : MonoBehaviour
{
    public float velocidad = 7f;
    public float aceleracion = 25f;
    public float frenado = 12f;
    public float inclinacion = 20f; // cuanto se inclina la nave al ir a los lados
    public float margen = 0.5f;     // para que no se salga del borde

    Rigidbody2D rb;
    Camera cam;
    Vector2 direccion;
    Vector2 vel;
    float angulo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // en el espacio no hay gravedad
        cam = Camera.main;
    }

    void Update()
    {
        direccion = Vector2.zero;

        // con Input.GetKey me salia error en Unity 6, por eso uso Keyboard.current
        if (Keyboard.current.rightArrowKey.isPressed) direccion.x = 1;
        if (Keyboard.current.leftArrowKey.isPressed) direccion.x = -1;
        if (Keyboard.current.upArrowKey.isPressed) direccion.y = 1;
        if (Keyboard.current.downArrowKey.isPressed) direccion.y = -1;

        // en diagonal iba mas rapido, con esto se arregla
        direccion = direccion.normalized;
    }

    void FixedUpdate()
    {
        if (direccion != Vector2.zero)
            vel = Vector2.MoveTowards(vel, direccion * velocidad, aceleracion * Time.fixedDeltaTime);
        else
            vel = Vector2.MoveTowards(vel, Vector2.zero, frenado * Time.fixedDeltaTime);

        Vector2 pos = rb.position + vel * Time.fixedDeltaTime;

        // limites de la pantalla
        float alto = cam.orthographicSize - margen;
        float ancho = cam.orthographicSize * cam.aspect - margen;

        if (pos.x < -ancho || pos.x > ancho) vel.x = 0;
        if (pos.y < -alto || pos.y > alto) vel.y = 0;

        pos.x = Mathf.Clamp(pos.x, -ancho, ancho);
        pos.y = Mathf.Clamp(pos.y, -alto, alto);

        rb.MovePosition(pos);

        // inclinar la nave segun para donde va
        angulo = Mathf.Lerp(angulo, -vel.x / velocidad * inclinacion, 8f * Time.fixedDeltaTime);
        rb.angularVelocity = 0;
        rb.MoveRotation(angulo);
    }
}
