using UnityEngine;
using ZagaCore;

namespace FunZaga
{
    [CreateAssetMenu(fileName = nameof(AppRootInstaller), menuName = "Services/" + nameof(AppRootInstaller))]
    public class AppRootInstaller : ServiceInstaller
    {
        [SerializeField]
        private GameObject prefab;

        public override void Init()
        {
            Instantiate(prefab, null, false);
            Refs.Instance.WaitFor<AppRoot>().Then((_) => {
                Promise.Resolve(); });
        }
    }
}