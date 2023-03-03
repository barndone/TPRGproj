using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Interface for decision tree implementaiton
public interface IDecision
{ 
    IDecision MakeDecision(Unit unit);
}

