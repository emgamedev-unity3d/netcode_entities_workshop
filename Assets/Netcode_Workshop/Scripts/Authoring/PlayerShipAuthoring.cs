using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShipAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerShipAuthoring>
    {
        public override void Bake(PlayerShipAuthoring authoring)
        {
            var playerShipEntity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

            PlayerShip component = default(PlayerShip);
            AddComponent(playerShipEntity, component);
        }
    }
}