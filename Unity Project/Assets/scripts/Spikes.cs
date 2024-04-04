using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int damage;
    private Animator animator;
    private Bow bow;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        bow = GameObject.FindGameObjectWithTag("bow").GetComponent<Bow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            animator.SetTrigger("damage");
            GetTopMostParent(other.transform).GetComponent<Enemy>().Damage(damage, bow.healing, 0f);
        }
    }

    public Transform GetTopMostParent(Transform child)
    {
        Transform topmost_parent = child;
        int x = 0;
        while (x < 100)
        {
            x++;
            if (topmost_parent.parent != null)
            {
                topmost_parent = topmost_parent.parent;
            }
            else
            {
                break;
            }
        }
        return topmost_parent;
    }
}
