using UnityEngine;
using System.Collections;


// This implementation of the state machine gives the responsiblity of updating the states to it's derived state classes
public abstract class StateMachine : MonoBehaviour
{
    protected State State;

    public void SetState(State state)
    {
        State = state;
        StartCoroutine(State.Execute());
    }


}
