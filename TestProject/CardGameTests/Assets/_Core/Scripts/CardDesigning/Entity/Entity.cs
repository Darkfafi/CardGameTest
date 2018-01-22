using System.Collections.Generic;
using System;

public abstract class BaseEntity
{
    private object NO_ARGS_ADDED = new object();
    private List<EntityPart> parts = new List<EntityPart>();

    public T RequireEntityPart<T>() where T : EntityPart, new()
    {
        return RequireEntityPart<T>(NO_ARGS_ADDED);
    }

    public T RequireEntityPart<T>(params object[] args) where T : EntityPart
    {
        T c = GetEntityPart<T>();
        if(c == null)
            c = AddEntityPart<T>(args);

        return c;
    }

    public T AddEntityPart<T>() where T : EntityPart, new()
    {
        return AddEntityPart<T>(NO_ARGS_ADDED);
    }

    public T AddEntityPart<T>(params object[] args) where T : EntityPart
    {
        bool canAdd = true;
        Type t = typeof(T);
        if(t.IsDefined(typeof(UniquePartAttribute), true))
        {
            if(HasEntityPart(t))
            {
                canAdd = false;
            }
        }

        if(canAdd)
        {
            T part;
            part = (args.Length == 1 && args[0] == NO_ARGS_ADDED) ? Activator.CreateInstance<T>() : (T)Activator.CreateInstance(t, args);
            parts.Add(part);

            if(part.Parent != null)
            {
                part.Parent.RemoveEntityPart(part.GetType());
            }

            part.GetType().GetProperty("Parent").SetValue(part, this, null);
            part.GetType().GetMethod("OnAdd", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(part, null);

            return part;
        }

        return null;
    }

    public EntityPart RemoveEntityPart(EntityPart partInstance)
    {
        if (partInstance == null) { return null; }
        if(parts.Contains(partInstance))
        {
            parts.Remove(partInstance);
            partInstance.GetType().GetMethod("OnRemove", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(partInstance, null);
            partInstance.GetType().GetProperty("Parent").SetValue(partInstance, null, null);
            return partInstance;
        }

        return null;
    }

    public T RemoveEntityPart<T>() where T : EntityPart
    {
        return (T)RemoveEntityPart(typeof(T));
    }

    public EntityPart RemoveEntityPart(Type entityPartType)
    {
        EntityPart ep = GetEntityPart(entityPartType);
        return RemoveEntityPart(ep);
    }

    public T GetEntityPart<T>() where T : EntityPart
    {
        return (T)GetEntityPart(typeof(T));
    }

    public EntityPart GetEntityPart(Type entityPartType)
    {
        for(int i = 0; i < parts.Count; i++)
        {
            if(parts[i].GetType().IsAssignableFrom(entityPartType))
            {
                return parts[i];
            }
        }

        return null;
    }

    public bool HasEntityPart<T>() where T : EntityPart
    {
        return HasEntityPart(typeof(T));
    }

    public bool HasEntityPart(Type entityPartType)
    {
        return GetEntityPart(entityPartType) != null;
    }
}

public abstract class EntityPart
{
    public BaseEntity Parent { get; private set; }

    protected abstract void OnAdd();
    protected abstract void OnRemove();
}


[AttributeUsage(AttributeTargets.Class)]
public class UniquePartAttribute : Attribute { }