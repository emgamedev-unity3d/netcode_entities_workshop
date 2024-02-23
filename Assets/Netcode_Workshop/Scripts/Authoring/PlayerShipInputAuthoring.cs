using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShipInputAuthoring : MonoBehaviour
{
    class Baking : Unity.Entities.Baker<PlayerShipInputAuthoring>
    {
        public override void Bake(PlayerShipInputAuthoring authoring)
        {
            var playerShipEntity = GetEntity(
                    authoring.gameObject,
                    Unity.Entities.TransformUsageFlags.Dynamic);

            AddComponent(playerShipEntity, new PlayerShipInput());
        }
    }
}