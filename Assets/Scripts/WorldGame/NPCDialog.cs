using UnityEngine;
using TMPro; // Es obligatorio para manejar el componente de texto interactivo

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Configuración del Diálogo")]
    [Tooltip("Escribe aquí cada línea de texto que dirá el NPC una por una")]
    [SerializeField] private string[] lineasDialogo;
    
    [Header("UI de Diálogo")]
    [Tooltip("Arrastra aquí el panel contenedor del cuadro de diálogo")]
    [SerializeField] private GameObject panelDialogo;
    
    [Tooltip("Arrastra aquí el componente de texto TextMeshProUGUI interno")]
    [SerializeField] private TextMeshProUGUI textoDialogo; 

    [Header("Efectos Visuales")]
    [Tooltip("Arrastra aquí el objeto hijo del borde amarillo")]
    [SerializeField] private GameObject bordeVisual;

    private int indiceActual;
    private bool estaHablando = false;

    public void Interact()
    {
        // Si el jugador ya está en medio de la conversación, evitamos que se reinicie
        if (estaHablando) return;

        // Validación de seguridad por si olvidaste escribir texto en el Inspector
        if (lineasDialogo == null || lineasDialogo.Length == 0) return;

        // Iniciamos la secuencia de conversación
        estaHablando = true;
        indiceActual = 0;
        MostrarLineaActual();
        
        if (panelDialogo != null) panelDialogo.SetActive(true);

        // Congelamos las físicas y controles del jugador usando tu EventManager
        EventManager.OnInteractionStarted?.Invoke();
    }

    private void Update()
    {
        if (!estaHablando) return;

        // Avanzamos el texto con la barra ESPACIO (coincidiendo con la lógica de tus menús)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AvanzarDialogo();
        }
    }

    private void AvanzarDialogo()
    {
        indiceActual++;

        // Si todavía quedan frases por decir, las mostramos
        if (indiceActual < lineasDialogo.Length)
        {
            MostrarLineaActual();
        }
        else
        {
            // Si ya no hay más texto, cerramos la interacción
            TerminarDialogo();
        }
    }

    private void MostrarLineaActual()
    {
        if (textoDialogo != null)
        {
            textoDialogo.text = lineasDialogo[indiceActual];
        }
    }

    private void TerminarDialogo()
    {
        estaHablando = false;
        if (panelDialogo != null) panelDialogo.SetActive(false);

        // Liberamos al jugador para que vuelva a caminar y saltar por el campus
        EventManager.OnInteractionEnded?.Invoke();
    }

    // --- TUS MÉTODOS DEL BORDE AMARILLO ---
    public void ActivarBorde()
    {
        if (bordeVisual != null) bordeVisual.SetActive(true);
    }

    public void DesactivarBorde()
    {
        if (bordeVisual != null) bordeVisual.SetActive(false);
    }
}
