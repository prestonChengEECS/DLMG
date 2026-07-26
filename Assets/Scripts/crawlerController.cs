using UnityEngine;

public class CrawlerController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Arm Bones")]
    public Transform leftShoulderBone;
    public Transform rightShoulderBone;

    [Header("Crawl Settings")]
    public float crawlMoveSpeed = 1.2f;
    public float armReachSpeed = 6f;
    public float armSwingDegrees = 30f;

    void Update()
    {
        // 1. Slide whole object along the ground towards player
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                crawlMoveSpeed * Time.deltaTime
            );
        }

        // 2. Animate arm reaches
        AnimateArms();
    }

    void AnimateArms()
    {
        // Smooth back-and-forth wave
        float swing = Mathf.Sin(Time.time * armReachSpeed) * armSwingDegrees;

        // Front arm reaches forward, back arm pulls back
        if (leftShoulderBone != null)
            leftShoulderBone.localRotation = Quaternion.Euler(0, 0, swing);

        if (rightShoulderBone != null)
            rightShoulderBone.localRotation = Quaternion.Euler(0, 0, -swing);
    }
}