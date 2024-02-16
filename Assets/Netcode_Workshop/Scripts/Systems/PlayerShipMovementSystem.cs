using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

// system that can read the CubeInput and move the player.
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerShipMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var speed = SystemAPI.Time.DeltaTime * 15f;
        var rotationSpeed = SystemAPI.Time.DeltaTime * 3f;

        foreach (var (input, transform) in 
            SystemAPI.Query<RefRO<PlayerShipInput>, RefRW<LocalTransform>>()
                .WithAll<Simulate>())
        {
            // process movement
            var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
            moveInput = math.normalizesafe(moveInput) * speed;

            transform.ValueRW.Position += new float3(moveInput.x, 0f, moveInput.y);

            // process rotation
            var currentRotation = input.ValueRO.rotationInput * rotationSpeed;

            transform.ValueRW.Rotation = 
                math.mul(
                    transform.ValueRO.Rotation,
                    quaternion.AxisAngle(math.up(), currentRotation));
        }
    }
}
