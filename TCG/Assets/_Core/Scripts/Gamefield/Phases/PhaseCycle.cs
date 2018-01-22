using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase
{
    BeginningPhase,
    PreCombatPhase,
    CombatPhase,
    PostCombatPhase,
    EndingPhase
}

public class PhaseCycle : MonoBehaviour
{
    public Phase CurrentPhase { get; private set; }

    public void NextPhase()
    {
        SetPhase(CurrentPhase + 1);
        int l = System.Enum.GetNames(typeof(Phase)).Length;
        if ((int)CurrentPhase > l - 1)
        {
            CurrentPhase = (Phase)(l - (int)CurrentPhase);
        }
    }

    private void SetPhase(Phase phase)
    {
        CurrentPhase = phase;
    }
}
