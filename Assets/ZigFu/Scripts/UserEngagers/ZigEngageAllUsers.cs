using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZigEngageAllUsers : MonoBehaviour {
	
	public GameObject InstantiatePerUser;
	Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();
	
	void Zig_UserFound(ZigTrackedUser user) 
	{
		GameObject o = Instantiate(InstantiatePerUser) as GameObject;
		objects[user.Id] = o;
		user.AddListener(o);
	}
	
	void Zig_UserLost(ZigTrackedUser user)
	{
		Destroy(objects[user.Id]);
		objects.Remove(user.Id);
	}
}
