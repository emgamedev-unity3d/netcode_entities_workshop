using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShipInputAuthoring : MonoBehaviour
{
    class Baking : Unity.Entities.Baker<PlayerShipInputAuthoring>
    {
        public override void Bake(PlayerShipInputAuthoring authoring)
        {
            AddComponent<PlayerShipInput>();
        }
    }
}