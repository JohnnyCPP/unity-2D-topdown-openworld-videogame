using System.Collections;
using System.Collections.Generic;
using UnityEngine;


abstract public class AIBehaviour : MonoBehaviour
{
    // called from start
    public abstract void InitBehaviourData();
    // called when the ai state changes to this behaviour
    public abstract void StartBehaviour();
    // called when the ai state changes to another behaviour
    public abstract void StopBehaviour();
    
    public abstract void UpdateBehaviour();
}