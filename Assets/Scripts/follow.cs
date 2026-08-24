using UnityEngine;

public class follow : MonoBehaviour
{

    public Rigidbody2D player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, transform.position.y, -10);
    }
}
