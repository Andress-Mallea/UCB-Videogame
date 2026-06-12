using UnityEngine;
using TMPro; // Obligatorio para controlar componentes de texto modernos

public class Logica_NPC : MonoBehaviour
{
    [Header("Sub-Paneles de las Fases")]
    public GameObject panelNombre;
    public GameObject panelNumeros;
    public GameObject panelResultado;

    [Header("Componentes de Entrada / Salida")]
    public TMP_InputField entradaNombre;
    public TMP_Text textoResultado;

    // Variables internas para guardar lo que elija el jugador
    private string nombreJugador;
    private int numeroFavorito;
    private bool enFaseFinal = false;

    void Start()
    {
        // Al arrancar, nos aseguramos de que solo la primera fase esté encendida
        panelNombre.SetActive(true);
        panelNumeros.SetActive(false);
        panelResultado.SetActive(false);
        enFaseFinal = false;
    }

    void Update()
    {
        // Únicamente si estamos en la última fase, permitimos cerrar con Espacio
        if (enFaseFinal && Input.GetKeyDown(KeyCode.Space))
        {
            CerrarConversacionNPC();
        }
    }

    // Se ejecuta al presionar el botón de la Fase 1
    public void RegistrarNombre()
    {
        if (entradaNombre != null && !string.IsNullOrEmpty(entradaNombre.text))
        {
            nombreJugador = entradaNombre.text;

            // Avanzamos a la Fase 2
            panelNombre.SetActive(false);
            panelNumeros.SetActive(true);
        }
    }

    // Se ejecuta al hacer clic en cualquiera de los 5 botones numéricos
    // Pasaremos el número directamente desde el Inspector de Unity
    public void SeleccionarNumero(int numero)
    {
        numeroFavorito = numero;

        // Construimos el mensaje dinámico combinando los datos recopilados
        if (textoResultado != null)
        {
            textoResultado.text = $"Parece que tu número favorito es el {numeroFavorito}.\n\n[ Presiona ESPACIO para salir ]";
        }

        // Avanzamos a la Fase Final
        panelNumeros.SetActive(false);
        panelResultado.SetActive(true);
        enFaseFinal = true;
    }

    void CerrarConversacionNPC()
    {
        // Apagamos el panel resultado para dar por terminada la interacción
        panelResultado.SetActive(false);
        enFaseFinal = false;
        Debug.Log("Conversación con el NPC finalizada con éxito.");
    }
}