using UnityEngine;
using System;

public abstract class BaseEffect
{
    public TargetSelectionType TargetingType { get { return targetingType; } }

    [SerializeField]
    protected TargetSelectionType targetingType;

    public abstract void DoEffect(params BaseEntity[] targets);
    public abstract IEffectFilter[] EffectFilters();
}

public class EffectFilter<T> : IEffectFilter where T : EntityPart
{
    public Type PartToCheck { get; private set; }
    public bool ConditionIfPartNotFoundOnItem { get; private set; }
    private Func<BaseEntity[]> conditionCheckMethod;
    
    public EffectFilter(bool noPartFoundCondition, Func<BaseEntity[]> conditionCheckMethod)
    {
        PartToCheck = typeof(T);
        ConditionIfPartNotFoundOnItem = noPartFoundCondition;
        this.conditionCheckMethod = conditionCheckMethod;
    }

    public BaseEntity[] EntitiesConditionMet()
    {
        return conditionCheckMethod();
    }
}

public interface IEffectFilter
{
    Type PartToCheck { get; } // Part to check, so targets can be identified by the part parents!
    bool ConditionIfPartNotFoundOnItem { get; } // If part is not found, condition is automatically put to true or false (Maybe for a "All cards without x" kind of filter)
    BaseEntity[] EntitiesConditionMet(); // Enough life, of correct type, above 8 cost? etc. Simply returns a yes or a no, the condition does not have to be known
}

public enum TargetSelectionType
{
    Automatic, // Card Selects its own targets
    OwnerPlayer, // The Casting Player selects the targets
    TargetPlayer // The Chosen Target Player selects the targets
}