using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Threading;

using UnityEngine.Events;
using ws.winx.unity.attributes;


namespace ws.winx.input
{
	[System.Serializable]
    public class InputEvent
    {

		public int state;

		public UnityEvent onUP=new UnityEvent();
		public UnityEvent onDOWN=new UnityEvent();
		public UnityEvent onHOLD=new UnityEvent();



        protected int _stateNameHash;

		public int stateNameHash {
			get {
				if(_stateNameHash==0 && Convert.ToInt32(state)!=0) _stateNameHash=Convert.ToInt32(state);
				return _stateNameHash;
			}
		}

	

        public InputEvent(int stateNameHash)
        {
            _stateNameHash = stateNameHash;

           
        }

        public InputEvent(string stateName):this(Animator.StringToHash(stateName))
        {
          
        }




		public event UnityAction HOLD
		{
			add
			{

				onHOLD.AddListener(value);
				
			}
			remove
			{
				onHOLD.RemoveListener(value);
			}
		}

       
        public event UnityAction UP
        {
            add
            {

				onUP.AddListener(value);

            }
            remove
            {
				onUP.RemoveListener(value);
            }
        }



        public event UnityAction DOWN
        {
            add
            {
				onDOWN.AddListener(value);
              
            }
            remove
            {
				onDOWN.RemoveListener(value);
            }
        }


       



        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="inx">0-Continuios events,1-Up,2-Down events</param>
//        protected void AddHandler(int key, Delegate value,uint inx)
//        {
//
//            Delegate[] d;
//
//
//            if (Events.TryGetValue(key, out d))
//            {
//                if (d[inx] != null)
//                    d[inx] = Delegate.Combine(d[inx], value);
//                else
//                    Events[key][inx] = value;
//            }
//            else
//            {
//                Events[key]=new Delegate[3];
//                Events[key][inx] = value;
//
//            }
//
//
//
//
//        }


//        protected void RemoveHandler(int key, Delegate value,uint inx)
//        {
//            Delegate[] d;
//
//            if (Events.TryGetValue(key, out d))
//            {
//                Events[key][inx] = Delegate.Remove(d[inx], value);
//            }
//            // else... no error for removal of non-existant delegate
//            //
//        }




        public void Dispose()
        {
       

			this.onUP.RemoveAllListeners ();
			this.onHOLD.RemoveAllListeners ();
			this.onDOWN.RemoveAllListeners ();

            
        }



      

    }
}
