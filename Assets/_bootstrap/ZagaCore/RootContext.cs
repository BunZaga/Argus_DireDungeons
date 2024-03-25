using RSG;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore.Events.Loading;

namespace ZagaCore
{
    public class RootContext : MonoBehaviour
    {
        private Refs services;
        private EventService eventService;

        // Core Services to be installed before all others
        // These are independent of other services to install
        // 'blockade service'
        // 'back end' URLS, game ID, (items, currencies, account, inventory etc)
        // 
        [SerializeField] private List<ServiceInstaller> coreServiceInstaller = new List<ServiceInstaller>();

        // not core services, things like window manager, audio manager, quest system, combat system, etc.
        [SerializeField] private List<ServiceInstaller> serviceInstaller = new List<ServiceInstaller>();

        private int totalServices;
        private int totalProgress;

        private void Awake()
        {
            Debug.Log("-----------------RootContext . Awake");
            services = Refs.Instance;

            services.WaitFor<EventService>().Then((eventService) =>
            {
                this.eventService = eventService;
            });

            totalServices = coreServiceInstaller.Count + serviceInstaller.Count;
            totalProgress = 0;

            var loadPromise = new List<Promise>();
            Debug.Log("----------------coreServiceInstaller:"+coreServiceInstaller.Count);
            for (int i = 0, ilen = coreServiceInstaller.Count; i < ilen; i++)
            {
                var installer = coreServiceInstaller[i];
                var p = installer.Promise;
                loadPromise.Add(p);

                p.Then(() =>
                {
                    Debug.Log("----------------Done loading "+installer);
                    totalProgress++;
                    coreServiceInstaller[coreServiceInstaller.IndexOf(installer)] = null;
                })
                .Catch((err) =>
                {
                    Debug.Log(err.Message);
                    Debug.Log(err.StackTrace);
                });
                
                installer.Init();
            }

            Promise.All(loadPromise)
                .Then(() =>
                {
                    Debug.Log("--------------------All core installers done.");
                    coreServiceInstaller.Clear();
                    InstallServices();
                }).Catch((err) =>
                {
                    Debug.Log(err.Message);
                    Debug.Log(err.StackTrace);
                });
        }

        private void InstallServices()
        {
            Debug.Log("----------------------InstallServices");
            if (serviceInstaller.Count > 0)
            {
                var loadLocalPromise = new List<Promise>();
                Debug.Log("---------------------services count:"+serviceInstaller.Count);
                for (int i = 0, ilen = serviceInstaller.Count; i < ilen; i++)
                {
                    var installer = serviceInstaller[i];
                    if(installer == null) { continue; }
                    var p = installer.Promise;
                    loadLocalPromise.Add(p);

                    p.Then(() =>
                    {
                        totalProgress++;
                        Debug.Log("---------------------installed service: "+installer);
                        var debugEvnt = eventService.GetPooledEvent<AddToDebugOutput>();
                        debugEvnt.Message = "-------------------Completed installing service:" + installer;
                        eventService.Dispatch(debugEvnt);

                        var evnt = eventService.GetPooledEvent<ServiceInitializedProgress>();
                        evnt.Progress = (float)totalProgress / totalServices;
                        eventService.Dispatch(evnt);
                        serviceInstaller[serviceInstaller.IndexOf(installer)] = null;
                    })
                    .Catch((err) =>
                    {
                        Debug.Log(err.Message);
                        Debug.Log(err.StackTrace);
                    });
                    
                    installer.Init();
                }

                Promise.All(loadLocalPromise)
                    .Then(() =>
                    {
                        Debug.Log("--------------------all done loading.");
                        serviceInstaller.Clear();
                        eventService.OnNextDispatch<AllServicesLoaded>(() => { Destroy(gameObject); });
                        eventService.ClearEventPool<ServiceInitializedProgress>();
                        eventService.Dispatch<AllServicesLoaded>();
                    })
                    .Catch((err) =>
                    {
                        Debug.Log(err.Message);
                        Debug.Log(err.StackTrace);
                    });
            }
            else
            {
                Debug.Log("-----------------No Services, done.");
                eventService.OnNextDispatch<AllServicesLoaded>(() => { Destroy(gameObject); });
                eventService.Dispatch<AllServicesLoaded>();
            }
        }
    }
}