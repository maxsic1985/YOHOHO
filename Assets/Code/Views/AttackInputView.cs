using System;
using UnityEngine;


namespace MSuhininTestovoe.B2B
{
    public sealed class AttackInputView : BaseView
    {
        public Action<bool> RayAction;
        [SerializeField] private GameObject _attackBtn;
        public GameObject AttackBtn => _attackBtn;
        

        private void OnDestroy()
        {
            RayAction -= ShowIconAttack;
        }
        private void Awake()
        {
            RayAction += ShowIconAttack;
            ShowIconAttack(false);
        }

        public void ShowIconAttack(bool show)
        {
            if (show) _attackBtn.SetActive(true);
            else _attackBtn.SetActive(false);
        }
    }
}