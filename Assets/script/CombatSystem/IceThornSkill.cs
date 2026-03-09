using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThornSkill : Skill
{
    public override void Launch(Transform transform)
    {
        if (transform.rotation.y < 0)
            Instantiate(summonObject, transform.position + new Vector3(-0.7f, 0, 0), transform.rotation);
        else
            Instantiate(summonObject, transform.position + new Vector3(0.7f, 0, 0), transform.rotation);
        SubtitleManager.Instance.ShowSubtitles("°¢¶û¡ªÐÞÂê£¡");
    }
}
