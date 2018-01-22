using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class DisplayAction
{
    public int Channel { get; private set; }
    public DisplayComplete DisplayCompleteStatus { get; private set; }

    public bool HasCompleted { get { return (DisplayCompleteStatus == null) ? false : DisplayCompleteStatus.HasCompleted; } }
    private Action<DisplayComplete> displayMethod;

    public DisplayAction(Action<DisplayComplete> displayMethod, int channel = DisplayExecuter.DEFAULT_CHANNEL)
    {
        Channel = channel;
        this.displayMethod = displayMethod;
        DisplayExecuter.Instance.SubmitActionDisplay(this);
    }
    
    public void InvokeDisplayAction()
    {
        DisplayCompleteStatus = new DisplayComplete();
        this.displayMethod(DisplayCompleteStatus);
        if(DisplayCompleteStatus.CompleteSetPermission == DisplayComplete.CompleteSetType.EndOfMethodSet)
        {
            DisplayCompleteStatus.SetCompletePermission(DisplayComplete.CompleteSetType.SelfSet);
            DisplayCompleteStatus.SetCompleteStatus(true);
        }
    }
}

public class DisplayComplete
{
    public enum CompleteSetType
    {
        EndOfMethodSet,
        SelfSet
    }

    public bool HasCompleted { get; private set; }
    public CompleteSetType CompleteSetPermission { get; private set; }

    public DisplayComplete()
    {
        CompleteSetPermission = CompleteSetType.EndOfMethodSet;
        HasCompleted = false;
    }

    public void SetCompleteStatus(bool status)
    {
        if (CompleteSetPermission != CompleteSetType.SelfSet) { return; }
        HasCompleted = status;
    }

    public void SetCompletePermission(CompleteSetType permission)
    {
        CompleteSetPermission = permission;
    }
}