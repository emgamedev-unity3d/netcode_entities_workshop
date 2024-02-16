using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShipAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerShipAuthoring>
    {
        public override void Bake(PlayerShipAuthoring authoring)
        {
            PlayerShip component = default(PlayerShip);
            AddComponent(component);
        }
    }
}