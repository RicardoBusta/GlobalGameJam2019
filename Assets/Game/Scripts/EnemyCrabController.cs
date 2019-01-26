using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    [RequireComponent(typeof(CrabController))]
    public class EnemyCrabController : MonoBehaviour
    {
        public double threshold;

        private CrabController controller;
        private Vector3 playerPos;
        private Tweener tweener;
        private int[] directions = {0, -1, 0, 1};
        private int directionIndex;

        private IEnumerable<Shell> shellList = new List<Shell>();
        private double distanceToShell = float.MaxValue;
        private Vector3 closestShellPos = Vector3.zero;

        private void Awake()
        {
            controller = GetComponent<CrabController>();
            directionIndex = Random.Range(0, 3);
        }

        private void Update()
        {
            Vector3 targetPos;
            playerPos = PlayerController.Instance.transform.position;

            if (!controller.hasShell)
            {
                distanceToShell = GetMinimumDistanceToShell();
            }

            double distance;
            var distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            if (distanceToShell < distanceToPlayer)
            {
                distance = distanceToShell;
                targetPos = closestShellPos;
            }
            else
            {
                distance = distanceToPlayer;
                targetPos = playerPos;
            }

            if (distance < threshold)
            {
                AggroMovement(targetPos);
            }
            else
            {
                IdleMovement();
            }
        }

        private void AggroMovement(Vector3 targetPos)
        {
            tweener?.Kill();

            controller.Speed = 0.9f;
            var pos = transform.position;
            var horizontal = pos.x - targetPos.x > 0 ? -1 : 1;
            controller.Handle(horizontal, 0);
        }

        private float GetMinimumDistanceToShell()
        {
            var minDistance = float.MaxValue;
            foreach (var shell in shellList)
            {
                if (shell.shellCollider.enabled) continue;

                var distance = Vector3.Distance(transform.position, shell.transform.position);
                if (!(distance < minDistance)) continue;

                minDistance = distance;
                closestShellPos = shell.transform.position;
            }

            return minDistance;
        }

        private void IdleMovement()
        {
            if (tweener != null)
            {
                return;
            }

            controller.Speed = 0.6f;
            directionIndex = (directionIndex + 1) % 4;

            var duration = 1f + Random.Range(-0.4f, 0.4f);
            tweener = DOVirtual.Float(1, 0, duration, speed => { controller.Handle(directions[directionIndex], 0); });

            tweener.onComplete += () => tweener = null;
        }
    }
}