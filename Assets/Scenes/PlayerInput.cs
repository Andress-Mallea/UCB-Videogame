using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Propiedades públicas de solo lectura. 
    // Otros scripts pueden saber qué botón se apretó, pero no pueden modificarlo.
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool InteracctionPress {get; private set;}
    public bool InteractPressed { get; private set; }
    private void Update()
    {
        // GetAxisRaw devuelve estrictamente -1, 0 o 1. 
        // A diferencia de GetAxis, no tiene "suavizado". Esto es vital para el "Game Feel" 
        // de un Metroidvania, ya que queremos que el personaje responda al instante.
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        
        // Detecta si se presionó la tecla "E" exclusivamente en este frame
        InteractPressed = Input.GetKeyDown(KeyCode.E);
        // GetButtonDown registra el frame exacto en que se presionó el botón de salto.
        JumpPressed = Input.GetButtonDown("Jump");
    }
}