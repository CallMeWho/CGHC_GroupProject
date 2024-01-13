using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float RetrieveHorizontalMoveInput();
    public abstract float RetrieveVerticalMoveInput();
    public abstract bool RetrieveInteractInput();
}
