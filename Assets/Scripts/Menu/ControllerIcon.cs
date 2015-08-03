using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class ControllerIcon : MonoBehaviour
    {
        [SerializeField] private GameObject[] icons;
        private int imageIndex = 0;

        public void SetIndex(int index)
        {
            icons[imageIndex].SetActive(false);
            icons[index].SetActive(true);
            imageIndex = index;
        }

        public void Deactivate()
        {
            icons[imageIndex].SetActive(false);
        }
    }
}