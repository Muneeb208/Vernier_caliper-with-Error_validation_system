using UnityEngine;

public enum ValidationStatus
{
    Acceptable,  // error is within tolerance
    Warning,     // error is slightly over tolerance
    Failed       // error is significantly over tolerance
}