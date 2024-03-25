using System.Collections;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Loading;

namespace FunZaga.Audio
{
    public class SplineFollower : MonoBehaviour
    {
        [SerializeField] private AudioController3d audioController3d;
        [SerializeField] private Spline spline;
        [SerializeField] private Transform target;

        [SerializeField] private int shortRateFPS;
        [SerializeField] private int longRateFPS;

        private Transform myTransform;

        private WaitForSeconds shortDelay;
        private WaitForSeconds longDelay;

        private AudioService audioService;

        private void Awake()
        {
            myTransform = transform;

            shortDelay = new WaitForSeconds(1.0f / shortRateFPS);
            longDelay = new WaitForSeconds(1.0f / longRateFPS);

            var eventService = Refs.Instance.Get<EventService>();
            // TODO add this to an update tick queue
            // AddFutureQueue
            // Action, Time
            eventService.OnNextDispatch<AllServicesLoaded>(OnAllServicesLoaded);
        }

        private void OnAllServicesLoaded()
        {
            Debug.Log("SplineFollower:OnAllServicesLoaded");
            audioService = Refs.Instance.Get<AudioService>();
            StartCoroutine(TickUpdate());
        }

        private IEnumerator TickUpdate()
        {
            var isFar = true;
            while (gameObject.activeSelf)
            {
                if (isFar)
                {
                    var closestPosition = spline.ClosestPosition(target.position);
                    if ((target.position - closestPosition).magnitude <= audioController3d.MaxDistance)
                    {
                        isFar = false;
                        myTransform.position = closestPosition;
                        if (!audioController3d.IsPlaying)
                        {
                            audioService.PlaySound(audioController3d, transform);
                        }
                        yield return null;
                    }
                    yield return longDelay;
                }
                else
                {
                    var closestPosition = spline.ClosestPosition(target.position);
                    if ((target.position - closestPosition).magnitude > audioController3d.MaxDistance)
                    {
                        isFar = true;
                        if (audioController3d.IsPlaying)
                        {
                            // stop audio source
                            audioService.Stop(audioController3d, transform);
                        }
                        yield return null;
                    }
                    myTransform.position = closestPosition;
                    yield return shortDelay;
                }
            }
        }
    }
}