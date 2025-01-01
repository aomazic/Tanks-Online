using UnityEngine;

public class MinTrigger : MonoBehaviour
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

        tower.enemiesInMaxRange.Remove(enemy);
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

        tower.enemiesInMaxRange.Add(enemy);
    }
}
