using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MegacityMetro.CameraManagement;
using Unity.NetCode;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation |
       WorldSystemFilterFlags.ClientSimulation)]
public partial struct CameraFollowPlayerShipSystem : ISystem
{
    public static readonly float3 k_cameraPositionOffset = new(0f, 10f, -15f);

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerShip>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        ComponentLookup<LocalToWorld> localToWorldLookup =
            SystemAPI.GetComponentLookup<LocalToWorld>(true);

        foreach (var (localToWorld, localTransform, playerShip) in 
            SystemAPI.Query<
                RefRO<LocalToWorld>,
                RefRO<LocalTransform>,
                RefRO<PlayerShip> >()
                    .WithAll<GhostOwnerIsLocal>())
        {
            if (!HybridCameraManager.Instance.WasInitialized)
            {
                HybridCameraManager.Instance.PlaceCamera(
                    localTransform.ValueRO.Position,
                    localTransform.ValueRO.Rotation);

                HybridCameraManager.Instance.WasInitialized = true;
            }

            var targetPosition =
                localToWorld.ValueRO.Position + k_cameraPositionOffset;

            HybridCameraManager.Instance.SetPlayerTargetPosition(targetPosition);

            HybridCameraManager.Instance.UpdateAimCameraTargetPosition(localToWorld.ValueRO.Position);

            //HybridCameraManager.Instance.UpdateAimCameraTargetPosition(
            //    laser.ValueRO.CalculateLaserEndPoint(
            //    localToWorld.ValueRO.Position,
            //    localToWorld.ValueRO.Rotation,
            //    ref localToWorldLookup,
            //    true));

            HybridCameraManager.Instance.CameraUpdate(deltaTime);
        }
    }
}