using System;

public static class EventManager
{
    // Eventos universales para cualquier interrupción del juego
    public static Action OnInteractionStarted;
    public static Action OnInteractionEnded;
}