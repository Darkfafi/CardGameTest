using System.Collections.Generic;

public static class EntityFiltering
{
    public static T[] GetEntities<T>(this T[] entityList, IEntityFilter<T>[] entityFilters) where T : BaseEntity
    {
        List<T> returnValue = new List<T>(entityList);

        for(int i = returnValue.Count - 1; i >= 0; i--)
        {
            for(int j = 0; j < entityFilters.Length; j++)
            {
                if (!entityFilters[j].FilterConditionMet(returnValue[i]))
                {
                    returnValue.RemoveAt(i);
                    break;
                }
            }
        }

        return returnValue.ToArray();
    }

    public static T[] GetEntities<T>(this List<T> entityList, IEntityFilter<T>[] entityFilters) where T : BaseEntity
    {
        return GetEntities(entityList.ToArray(), entityFilters);
    }
}

public interface IEntityFilter<T>
{
    bool FilterConditionMet(T entityToTest);
}