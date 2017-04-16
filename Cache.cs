using System;
using System.Collections.Generic;

namespace eng_back
{
    public class TempCache<Key,Val>
    {
        protected Dictionary<Key,Val> caobj=new Dictionary<Key,Val>();
        
        public TempCache()
        {

        }
    }
}