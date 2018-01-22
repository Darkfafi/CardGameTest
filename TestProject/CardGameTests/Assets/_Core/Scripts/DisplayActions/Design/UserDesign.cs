using UnityEngine;

public class UserDesign : ScriptableObject
{
	public string Name { get { return username; } }
    public int Health { get { return health; } }
    public int Attack { get { return attack; } }

    [SerializeField]
    private string username;

    [SerializeField]
    private int health;

    [SerializeField]
    private int attack;


}
