using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Movement : MonoBehaviour
{
    public float speed = 3.0f;               
    public Transform[] waypoints;             
    private int currentTarget = 0;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (waypoints.Length == 0)
        {
            return;
        }

        transform.position = waypoints[0].position;
        currentTarget = 1;

        PlayMoveAnimation((waypoints[currentTarget].position - transform.position).normalized);
        PlayMoveSound();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Vector3 targetPos = waypoints[currentTarget].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        // 移动
        transform.position += direction * speed * Time.deltaTime;

        // 到达目标点
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            transform.position = targetPos; // 修正
            currentTarget = (currentTarget + 1) % waypoints.Length;

            Vector3 nextDir = (waypoints[currentTarget].position - transform.position).normalized;
            PlayMoveAnimation(nextDir);
        }
    }

    void PlayMoveAnimation(Vector3 dir)
    {
        if (animator == null) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                animator.Play("Right");
            else
                animator.Play("Left");
        }
        else
        {
            if (dir.y > 0)
                animator.Play("Back");
            else
                animator.Play("Front");
        }
    }

    void PlayMoveSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
