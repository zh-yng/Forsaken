using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; // Measure of how much a layer will move. 0 - don't move, 1 - move fully with camera, 0.5 - half
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceX = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distanceX, transform.position.y, transform.position.z);
    }
}
