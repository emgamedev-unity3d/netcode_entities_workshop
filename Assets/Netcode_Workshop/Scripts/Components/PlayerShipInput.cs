using Unity.NetCode;

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerShipInput : IInputComponentData
{
    public int Horizontal;
    public int Vertical;
    public int rotationInput;

    public float Pitch; //look up or down
    public float Yaw; //y-axis rotation
}