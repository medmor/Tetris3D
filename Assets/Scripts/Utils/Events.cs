
using UnityEngine.Events;

public class Events
{
    [System.Serializable] public class EventGameState : UnityEvent<Enums.GameState, Enums.GameState> { }
    [System.Serializable] public class ControlsInput : UnityEvent<Enums.ControlsEvents> { }
    [System.Serializable] public class SwipInput : UnityEvent<Enums.Directions, int> { }

}
