using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerShipSpawnerAuthoring : MonoBehaviour
{
    public GameObject PlayerShipPrefab;

    class Baker : Baker<PlayerShipSpawnerAuthoring>
    {
        public override void Bake(PlayerShipSpawnerAuthoring authoring)
        {
            PlayerShipSpawner component = default;
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            component.PlayerShipPrefab = GetEntity(authoring.PlayerShipPrefab); //obsolete :(
            AddComponent(component); //obsolete :(

            //Doesn't work...
                //CubeSpawner component = default(CubeSpawner);
                //component.Cube = GetEntity(authoring.Cube, TransformUsageFlags.Dynamic);
                //
                //var entity = GetEntity(TransformUsageFlags.Dynamic);
                //AddComponent(entity, component);
        }
    }
}