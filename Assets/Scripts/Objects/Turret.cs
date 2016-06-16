using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Turret : NetworkBehaviour
{
    [SerializeField]
    private float shotsPerSecond;
    private float inversedShotsPerSecond;
    [SerializeField]
    private float shootRange, lifeTime;

    private bool reloading;

    [SerializeField]
    private GameObject bulletPrefab;

    // Targeting
    private List<GameObject> AvaibleEnemies = new List<GameObject>();
    private GameObject target;

    // Refrences
    private Animator aniRef;
    // Use this for initialization
    void Start()
    {
        if (!isServer)
        {
            return;
        }
        aniRef = GetComponent<Animator>();
        inversedShotsPerSecond = 1 / shotsPerSecond;
        StartCoroutine(NetworkDestroy(lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
        // Server check
        if (!isServer)
        {
            return;
        }

        // Are we reloading
        if (!reloading)
        {
            if (!target)
            {
                target = getTarget();
                Debug.Log(target.name);
                return;
            }

            // Is target within range
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) >= shootRange)
            {
                target = getTarget();
                return;
            }            
            Vector2 direction = GetDirection(transform, target.transform);
            CmdShoot(direction);
            aniRef.SetTrigger("attack");
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    // Network spawn bullet
    [Command]
    void CmdShoot(Vector2 direction)
    {
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + new Vector3(direction.x, direction.y, 0).normalized * 1f, transform.rotation);
        bullet.GetComponent<Bullet>().Go(direction, "Player");
        bullet.transform.SetParent(transform, true);
        NetworkServer.Spawn(bullet);
    }

    /// <summary>
    /// Get a Direction to shoot in
    /// </summary>
    /// <param name="a"> First transform</param>
    /// <param name="b"> Second transform</param>
    /// <returns>Direction vector</returns>
    Vector2 GetDirection(Transform a, Transform b)
    {
        Vector2 dir = new Vector2(b.position.x - a.position.x, b.position.y - a.position.y);
        return dir;
    }

    // Reloading coroutine
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(inversedShotsPerSecond);
        reloading = false;
    }

    // Get the enemy target
    GameObject getTarget()
    {
        GameObject[] AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in AllEnemies)
        {
            if ( enemy.GetComponent<Entity>().healthPoints > 0)
            {
                AvaibleEnemies.Add(enemy);
            }
        }

        if (AvaibleEnemies.Count == 0)
        {
            return null;
        }

        GameObject optimalTarget = AvaibleEnemies[0];
        foreach (GameObject enemy in AvaibleEnemies)
        {
            if( Vector3.Distance(optimalTarget.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position))
            {
                optimalTarget = enemy;
            }
        }
        AvaibleEnemies.Clear();
        return optimalTarget;
    }

    IEnumerator NetworkDestroy(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        aniRef.SetTrigger("death");
        yield return new WaitForSeconds(1f);
        CmdDestroyGameObject();
    }

    [Command]
    void CmdDestroyGameObject()
    {

        NetworkServer.Destroy(gameObject);
    }

    void Ondestroy()
    {
        if (isServer)
            FindObjectOfType<InputOracle>().RemovePosFromUsed(transform.position);
    }
}
