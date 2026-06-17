using UnityEngine;
using Metroidvania.Minigames;
using TMPro;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuración del Minijuego")]
    [SerializeField] private GameObject minigamePrefab;
    [SerializeField] private ScriptableObject levelData;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshPro resultText;

    [Header("Efectos Visuales (TU BORDE)")]
    public GameObject bordeVisual; 

    private MinigameBase activeMinigameInstance;

    public void Interact()
    {
        if (minigamePrefab != null)
        {
            GameObject minigameInstance = Instantiate(minigamePrefab);
            activeMinigameInstance = minigameInstance.GetComponent<MinigameBase>();
            
            if (activeMinigameInstance != null)
            {
                if (levelData != null) activeMinigameInstance.SetLevelData(levelData);
                activeMinigameInstance.OnMinigameFinished += HandleMinigameFinished;
                activeMinigameInstance.InitializeAndStart();
            }
            EventManager.OnInteractionStarted?.Invoke();
        }
    }

    private void HandleMinigameFinished(MinigameResult result)
    {
        if (activeMinigameInstance != null)
        {
            activeMinigameInstance.OnMinigameFinished -= HandleMinigameFinished;
            activeMinigameInstance = null;
        }
        
        if (result.isCompletedSuccessfully)
        {
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(result.finalScore);

            if (resultPanel != null && resultText != null)
            {
                resultText.text = $"¡HACKEO EXITOSO!\n\nTiempo: {result.timeElapsed:F1}s\nMovimientos: {result.movesCount}\n\nPuntaje Obtenido: {result.finalScore}\n\n[ Presiona ESPACIO para continuar ]";
                resultPanel.SetActive(true);
            }
            else
            {
                EventManager.OnInteractionEnded?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            EventManager.OnInteractionEnded?.Invoke();
        }
    }

    private void Update()
    {
        if (resultPanel != null && resultPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            resultPanel.SetActive(false);
            EventManager.OnInteractionEnded?.Invoke();
            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        if (activeMinigameInstance != null) activeMinigameInstance.OnMinigameFinished -= HandleMinigameFinished;
    }

    // --- TUS MÉTODOS VISUALES ---
    public void ActivarBorde() { if (bordeVisual != null) bordeVisual.SetActive(true); }
    public void DesactivarBorde() { if (bordeVisual != null) bordeVisual.SetActive(false); }
}