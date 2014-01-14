using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEntities
{
    [Serializable]
    class A
    {
         B bval = new B();
         C cval = new C();
         string msg = "hello";
    }

    [Serializable]
    class B
    {
         string str = "bye";
    }

    [Serializable]
    class C
    {
        string[] info = new string[] { "hello", "world" };
    }
}
