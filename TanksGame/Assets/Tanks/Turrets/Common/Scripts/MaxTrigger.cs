using UnityEngine;

public class MaxTrigger : MonoBehaviour
{
    private ITower tower;

    private void Start()
    {
        tower = GetComponentInParent<ITower>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        var enemy = other.GetComponent<IEnemy>();
        if (enemy == null)
        {
            return;
        }
        tower.NewEnemyDetected(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        var enemy = other.GetComponent<IEnemy>();
        if (enemy == null)
        {
            return;
        }
        
        tower.EnemyOutOfRange(enemy);
    }
}
