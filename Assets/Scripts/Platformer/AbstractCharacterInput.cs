using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharacterInput : MonoBehaviour
{
    public abstract bool Left { get; }
    public abstract bool Right { get; }
    public abstract bool Jump { get; }
}
