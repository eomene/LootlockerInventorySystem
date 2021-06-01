using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField]
        float speed = 30;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, 0, speed);
        }
    }
}