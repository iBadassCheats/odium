namespace Odium.Wrappers;

internal class WorldWrapper
{
	public static bool IsInRoom()
	{
		return RoomManager.field_Internal_Static_ApiWorld_0 != null && RoomManager.field_Private_Static_ApiWorldInstance_0 != null;
	}
}
