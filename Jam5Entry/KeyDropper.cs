using UnityEngine;

namespace Jam5Entry
{
    public class KeyDropper : MonoBehaviour
    {
        [SerializeField] private GameObject _keyObject;
        [SerializeField] private Transform _dropPoint;

        public void Start()
        {
            _keyObject.SetActive(false);
        }

        public void DropKey()
        {
            if (_keyObject != null && _dropPoint != null)
            {
                _keyObject.transform.position = _dropPoint.position;
                _keyObject.SetActive(true);
            }
        }
    }
}
