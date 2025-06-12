using Unity.Cinemachine;

public class PlayerBundle
{
    public PlayerController Player { get; }
    public CinemachineCamera Camera { get; }
    public PickupMarker PickupMarker { get; }

    public PlayerBundle(PlayerController player, CinemachineCamera camera, PickupMarker pickupMarker)
    {
        Player = player;
        Camera = camera;
        PickupMarker = pickupMarker;
    }

    public class Factory : Zenject.PlaceholderFactory<PlayerBundle> { }
}