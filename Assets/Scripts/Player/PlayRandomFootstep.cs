using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomFootstep : MonoBehaviour {
    private AudioClip[] footsteps;
    public const int NumFootstepFiles = 6;

    void Start()
    {
        footsteps = new AudioClip[NumFootstepFiles];
        for (int i = 1; i <= NumFootstepFiles; i++)
        {
            footsteps[i - 1] = Resources.Load<AudioClip>("Sound/SFX/Footsteps/" + i);
        }
        
    }

    public void RandomFootstep() {
        int FileToPlay = (int) Random.Range(0, NumFootstepFiles);
        
		AudioSource.PlayClipAtPoint(footsteps[FileToPlay], Camera.main.transform.position);
    }
}
