using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MyCharacterController
{
    [SerializeField] private ScreenTouchController input;
    [SerializeField] private ShootController shootController;
    private  List<Transform> enemies = new List<Transform>();
    private bool _isShooting;
    private int enemyAmount;

    private void FixedUpdate()
    {
        var direction = new Vector3(input.Direction.x, 0f, input.Direction.y);
       Move(direction);
    }
    private void Start()
    {
        enemyAmount = FindObjectsOfType<EnemyController>().Length;
          
    }
    private void Update()
    {
        if (enemies.Count>0)
        {
            transform.LookAt(enemies[0]);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag($"Enemy"))
        {
            Dead();
        }
        if (collision.transform.CompareTag("FinishArea"))
        {
           
            Win();
        }
    }
    private void OnTriggerStay(Collider other)
    {
       
        if (other.transform.CompareTag($"Enemy"))
        {
            
            if (!enemies.Contains(other.transform))
            {
                enemies.Add(other.transform);
                AutoShoot();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag($"Enemy"))
        {
            enemies.Remove(other.transform);
        }
    }
    private void AutoShoot()
    {
        IEnumerator Do()
        {
            Debug.Log("IEnumerator içi");
            while (enemies.Count>0)
            {
                
                var enemy = enemies[0];
                var direction = enemy.transform.position - transform.position;
                direction.y = 0;
                direction = direction.normalized;
                shootController.Shoot(direction, transform.position);
               // transform.LookAt(enemy.transform);
                enemies.RemoveAt(0);
                yield return new WaitForSeconds(shootController.Delay);
            }

            _isShooting = false;

        }
        if (!_isShooting)
        {
            
            _isShooting = true;
            StartCoroutine(Do());

        }
    }
    private void Dead()
    {
      //  Debug.Log("Dead");
        Time.timeScale = 0f;
    }

    private void Win()
    {
        //Debug.Log("Win");
        //Time.timeScale = 0;
        var current = FindObjectsOfType<EnemyController>().Length;
        var result = current / enemyAmount;
        var success = Mathf.Lerp(100, 0, result);
        Debug.Log($"Completed => %{success}");
        
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1);
            //Time.timeScale = 1;
        }
        else
        {
            //Time.timeScale = 1;
            SceneManager.LoadScene(0);

        }
        
    }
}