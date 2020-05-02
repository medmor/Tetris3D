
using UnityEngine.Events;

public class Events
{
    [System.Serializable] public class EventGameState : 
        UnityEvent<Enums.GameState, Enums.GameState> { }
}
