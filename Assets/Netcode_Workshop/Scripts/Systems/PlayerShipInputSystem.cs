using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerShipInputSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //bool left = m_keyboard.aKey.isPressed || m_keyboard.leftArrowKey.isPressed;
        //bool right = m_keyboard.dKey.isPressed || m_keyboard.rightArrowKey.isPressed;
        //bool down = m_keyboard.sKey.isPressed ||m_keyboard.downArrowKey.isPressed;
        //bool up = m_keyboard.wKey.isPressed || m_keyboard.upArrowKey.isPressed;

        bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        bool isQkeyPressed = Input.GetKey(KeyCode.Q);
        bool isEkeyPressed = Input.GetKey(KeyCode.E);


        foreach (var playerInput in 
            SystemAPI.Query<RefRW<PlayerShipInput>>().WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW = default;

            // check for movement input
            if (left)
                playerInput.ValueRW.Horizontal -= 1;
            if (right)
                playerInput.ValueRW.Horizontal += 1;
            if (down)
                playerInput.ValueRW.Vertical -= 1;
            if (up)
                playerInput.ValueRW.Vertical += 1;

            // check for rotation input
            if(isQkeyPressed)
            {
                playerInput.ValueRW.rotationInput -= 1;
            }
            if (isEkeyPressed)
            {
                playerInput.ValueRW.rotationInput += 1;
            }

            //float2 lookDelta = float2.zero;
            //lookDelta = new float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //playerInput.ValueRW.Pitch = 
            //    math.clamp(
            //        playerInput.ValueRW.Pitch + lookDelta.y,
            //        -math.PI / 2f,
            //        math.PI / 2f);

            //playerInput.ValueRW.Yaw = 
            //    math.fmod(
            //        playerInput.ValueRW.Yaw + lookDelta.x,
            //        2f * math.PI);
        }
    }
}