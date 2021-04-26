using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSky : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        player = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_AspectRatio", (float)Screen.width / Screen.height);
        Shader.SetGlobalFloat("_SkyAngle_y", player.transform.eulerAngles.x / 180f);
        Shader.SetGlobalFloat("_SkyAngle_x", player.transform.eulerAngles.y / 180f);
    }
}
