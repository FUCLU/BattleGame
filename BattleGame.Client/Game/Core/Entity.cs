using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Game.Core;

public class Entity
{
    public int Id { get; }
    private readonly Dictionary<Type, IComponent> _components = new();

    public Entity(int id) => Id = id;

    public void Add<T>(T component) where T : IComponent
        => _components[typeof(T)] = component;

    public T Get<T>() where T : IComponent
        => (T)_components[typeof(T)];

    public bool Has<T>() where T : IComponent
        => _components.ContainsKey(typeof(T));
}