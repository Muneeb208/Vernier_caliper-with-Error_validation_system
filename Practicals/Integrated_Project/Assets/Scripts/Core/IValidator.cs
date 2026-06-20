using UnityEngine;

// Interface for ALL validators — Dependency Inversion Principle
// Every new validator MUST implement this contract
public interface IValidator
{
    bool IsValid { get; }
    string ErrorMessage { get; }
    void Validate();
}