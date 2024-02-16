using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct CameraFollowPlayerShipSystem : ISystem
{
    public static readonly float3 k_cameraPositionOffset = new(0f, 10f, -15f);

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamInGame>();

        state.RequireForUpdate<PlayerShip>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var camera = UnityEngine.Camera.main;

        foreach (var (localToWorld, cube) in 
            SystemAPI.Query< RefRO<LocalToWorld>, RefRO<PlayerShip> >()
                .WithAll<GhostOwnerIsLocal>())
        {
            camera.transform.position = 
                localToWorld.ValueRO.Position + k_cameraPositionOffset;
        }
    }
}