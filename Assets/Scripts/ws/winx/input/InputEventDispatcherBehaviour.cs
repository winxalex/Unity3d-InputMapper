using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.input
{
    public class InputEventDispatcherBehaviour : MonoBehaviour {
            public Dictionary<int, Delegate[]> Events;
            public EventArgs args;

            public bool atOnce;

            void Awake()
            {
                UnityEngine.Object.DontDestroyOnLoad(this);
            }

            void onEnable()
            {

            }

            void onDisable()
            {
               
            }


            void Update()
            {
                Delegate[] delegates;

                foreach (KeyValuePair<int, Delegate[]> pair in Events)
                {
                    
                    if(pair.Value[0]!=null && InputManager.GetInput(pair.Key,false)){
                        delegates= pair.Value[0].GetInvocationList();
                        foreach(Delegate d in delegates)
                            ((EventHandler)d).BeginInvoke(this, args, EndAsyncEvent, null);
                    }

                    if (pair.Value[1] != null && InputManager.GetInputUp(pair.Key))
                    {
                        delegates = pair.Value[1].GetInvocationList();
                        foreach (Delegate d in delegates)
                            ((EventHandler)d).BeginInvoke(this, args, EndAsyncEvent, null);
                    }

                    if (pair.Value[2] != null && InputManager.GetInputDown(pair.Key))
                    {
                        delegates = pair.Value[2].GetInvocationList();
                        foreach (Delegate d in delegates)
                            ((EventHandler)d).BeginInvoke(this, args, EndAsyncEvent, null);
                    }

                  

                }
            }



            private void EndAsyncEvent(IAsyncResult iar)
            {
                var ar = (System.Runtime.Remoting.Messaging.AsyncResult)iar;
                var invokedMethod = (EventHandler)ar.AsyncDelegate;

                try
                {
                    invokedMethod.EndInvoke(iar);
                }
                catch
                {
                    // Handle any exceptions that were thrown by the invoked method
                    Console.WriteLine("An event listener went kaboom!");
                }
            }



        }
}
