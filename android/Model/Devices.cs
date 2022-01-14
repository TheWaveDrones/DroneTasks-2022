using System;
using System.Collections.Generic;

namespace Model
{
    public class Devices : List<string>
    {
        public bool Exists(string id)
        {
            if(Find(item => item.Equals(id)) == null)
            {
                return false;
            }
            return true;
        }
    }
}
