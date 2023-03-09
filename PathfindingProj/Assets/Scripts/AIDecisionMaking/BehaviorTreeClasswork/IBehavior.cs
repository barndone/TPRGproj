using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BehaviorResult
{ 
    Success,
    Failure
}

public interface IBehavior
{
    BehaviorResult DoBehavior(Unit agent);
}
