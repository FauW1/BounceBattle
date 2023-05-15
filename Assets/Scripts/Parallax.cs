using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = this.transform.position.x;
        //Debug.Log(this.name + " , " + startpos);
        //ypos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(this.name + " , " + length);
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        //https://www.youtube.com/watch?v=zit45k6CUMk
    }
}