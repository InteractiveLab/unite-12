using UnityEngine;
using System.Collections;

public class ZigEngageSplitScreen : MonoBehaviour {
    public GameObject LeftPlayer;
    public GameObject RightPlayer;

    public ZigTrackedUser leftTrackedUser { get; private set; }
    public ZigTrackedUser rightTrackedUser { get; private set; }

    Bounds leftRegion = new Bounds(new Vector3(-500, 0, -3000), new Vector3(1000, 3000, 2000));
    Bounds rightRegion = new Bounds(new Vector3(500, 0, -3000), new Vector3(1000, 3000, 2000));

    bool AllUsersEngaged { get { return null != leftTrackedUser && null != rightTrackedUser; } }

    ZigTrackedUser LookForTrackedUserInRegion(ZigInput zig, Bounds region) {
        foreach (ZigTrackedUser trackedUser in zig.TrackedUsers.Values) {
            if (trackedUser.SkeletonTracked && region.Contains(trackedUser.Position) && trackedUser != leftTrackedUser && trackedUser != rightTrackedUser) {
                return trackedUser;
            }
        }
        return null;
    }

    void Zig_Update(ZigInput zig) {
        bool areTheyEngaged = AllUsersEngaged;
        // left user
        if (null == leftTrackedUser) {
            leftTrackedUser = LookForTrackedUserInRegion(zig, leftRegion);
            if (null != leftTrackedUser) {
                leftTrackedUser.AddListener(LeftPlayer);
                SendMessage("UserEngagedLeft", this, SendMessageOptions.DontRequireReceiver);
            }
        }
        // right user
        if (null == rightTrackedUser) {
            rightTrackedUser = LookForTrackedUserInRegion(zig, rightRegion);
            if (null != rightTrackedUser) {
                rightTrackedUser.AddListener(RightPlayer);
                SendMessage("UserEngagedRight", this, SendMessageOptions.DontRequireReceiver);
            }
        }
        if (!areTheyEngaged && AllUsersEngaged) {
            SendMessage("AllUsersEngaged", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Zig_UserLost(ZigTrackedUser user) {
        if (user == leftTrackedUser) {
            leftTrackedUser = null;
            SendMessage("UserDisengagedLeft", this, SendMessageOptions.DontRequireReceiver);
        }
        if (user == rightTrackedUser) {
            rightTrackedUser = null;
            SendMessage("UserDisengagedRight", this, SendMessageOptions.DontRequireReceiver);
        }
    }
}

