using UnityEngine;

public class Menu_Pause : MonoBehaviour
{

    public GameObject Menu_Guardado;

    public GameObject Menu_Indicaciones;

    public GameObject Boton_Cerrar;

    public GameObject Boton_Pausa;

    public GameObject Globo_Confirmacion_Pausa;

    public GameObject Boton_Salir;

    public GameObject Globo_Confirmacion_Salir;

    public void SalirPartida()
    {
        Debug.Log("Partida Guardada Exitosamente");

        if (Globo_Confirmacion_Salir != null)
        {
            Globo_Confirmacion_Salir.SetActive(true);

            Invoke("OcultarGloboSalir", 4f);
        }
    }

    public void OcultarGloboSalir()
    {
        if (Globo_Confirmacion_Salir != null)
        {
            Globo_Confirmacion_Salir.SetActive(false);
        }
    }

    public void GuardarPartida()
    {
        Debug.Log("Partida Guardada Exitosamente");

        if(Globo_Confirmacion_Pausa != null)
        {
            Globo_Confirmacion_Pausa.SetActive(true);

            Invoke("OcultarGlobo", 4f);
        }
    }

    public void OcultarGlobo()
    {
        if(Globo_Confirmacion_Pausa != null)
        {
            Globo_Confirmacion_Pausa.SetActive(false);
        }
    }

    void Update()
    {
        
        // Detecta si el jugador presionó la tecla F en este fotograma
        if (Input.GetKeyDown(KeyCode.F))
        {
            AlternarMenuPausa();
        }

        
        if (Input.GetKeyDown(KeyCode.H))
        {
            AlternarIndicaciones();
        }
    }

    public void AlternarMenuPausa()
    {
        if (Menu_Guardado != null)
        {
            // Tomamos el estado actual del menú (activo o inactivo)
            bool estaActivo = Menu_Guardado.activeSelf;

            // Invertimos el estado: si estaba encendido se apaga, si estaba apagado se enciende
            Menu_Guardado.SetActive(!estaActivo);

            // [Opcional] Si tu juego tiene movimiento de fondo, aquí podrías pausar el tiempo:
            // Time.timeScale = estaActivo ? 1f : 0f;
        }
    }

    public void AlternarIndicaciones()
    {
        if (Menu_Indicaciones != null)
        {
            // Tomamos el estado actual del menú (activo o inactivo)
            bool estaActivo = Menu_Indicaciones.activeSelf;

            // Invertimos el estado: si estaba encendido se apaga, si estaba apagado se enciende
            Menu_Indicaciones.SetActive(!estaActivo);

            // [Opcional] Si tu juego tiene movimiento de fondo, aquí podrías pausar el tiempo:
            // Time.timeScale = estaActivo ? 1f : 0f;
        }
    }

    public void CerraMenu()
    {
        Menu_Guardado.SetActive(false);
    }
}