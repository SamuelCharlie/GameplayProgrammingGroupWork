using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform playerTransform;

    public LayerMask what_is_ground, what_is_player;

    public Vector3 walk_point;
    bool walk_point_set;
    public float walk_point_range;

    public float attack_interval;
    bool already_attacked;

    public float sight_range, attack_range;
    public bool player_in_sight_range, player_in_attack_range;

    [SerializeField] private float starting_obj_hp;
    public float object_health;
    public float player_health;

    public Animator player_animator;

    [SerializeField] GameObject slime;
    public GameObject slime_cam;
    public GameObject player_object;

    private void Awake()
    {
        playerTransform = GameObject.Find("RPG-Character").transform;
        agent = GetComponent<NavMeshAgent>();

        if (object_health == 2)
        {
            transform.localScale /= 2;
        }
        else if (object_health == 1)
        {
            transform.localScale /= 2;
        }
    }

    void Update()
    {
        player_in_sight_range = Physics.CheckSphere(transform.position, sight_range, what_is_player);
        player_in_attack_range = Physics.CheckSphere(transform.position, attack_range, what_is_player);

        if (!player_in_sight_range && !player_in_attack_range)
        {
            PatrolArea();
        }
        if (player_in_sight_range && !player_in_attack_range)
        {
            ChasePlayer();
        }
        if (player_in_sight_range && player_in_attack_range)
        {
            AttackPlayer();
            if (PlayerController.is_attacking)
            {
                StartCoroutine(PlayerAttackInterval());
            }
        }        
    }

    private void PatrolArea()
    {
        if (!walk_point_set)
        {
            SearchWalkPoint();
        }
        if (walk_point_set)
        {
            agent.SetDestination(walk_point);
        }

        Vector3 distance_to_walk_point = transform.position - walk_point;
        if (distance_to_walk_point.magnitude < 1.0f)
        {
            walk_point_set = false;
        }
    }

    private void SearchWalkPoint()
    {
        float random_x = Random.Range(-walk_point_range, walk_point_range);
        float random_z = Random.Range(-walk_point_range, walk_point_range);

        walk_point = new Vector3(transform.position.x + random_x, transform.position.y, transform.position.z + random_z);

        if (Physics.Raycast(walk_point, -transform.up, 2.0f, what_is_ground))
        {
            walk_point_set = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(playerTransform.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(playerTransform);
        if (player_health != 0)
        {
            StartCoroutine(EnemyAttackInterval());
        }
        else if (player_health == 0)
        {
            Destroy(player_object);
            slime_cam.SetActive(true);
        }
    }

    private void ResetAttack()
    {
        already_attacked = false;
    }

    public void ObjectHit(int damage_dealt)
    {
        object_health -= damage_dealt;

        if (object_health <= 0)
        {
            if (starting_obj_hp > 1)
            {
                starting_obj_hp--;
                attack_range--;
                object_health = starting_obj_hp;

                Instantiate(slime, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), 
                    Quaternion.identity);
                Instantiate(slime, new Vector3(transform.position.x - 2, transform.position.y, transform.position.z), 
                    Quaternion.identity);
            }
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    IEnumerator PlayerAttackInterval()
    {
        PlayerController.is_attacking = false;
        yield return new WaitForSeconds(0.5f);
        slime.GetComponent<Rigidbody>().AddForce(new Vector3((transform.position.x - player_object.transform.position.x), 2.0f, 
            (transform.position.z - player_object.transform.position.z)).normalized, ForceMode.Impulse);
        ObjectHit(1);
    }

    IEnumerator EnemyAttackInterval()
    {
        if (!already_attacked)
        {
            player_health -= 1;

            already_attacked = true;
            yield return new WaitForSeconds(1.5f);
            ResetAttack();
        }
    }
}
