using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A
{
    public A test;
}

public class B:A
{
}

public class PolyologyTest : MonoBehaviour
{
    [ContextMenu("测试多态")]
    private void Test()
    {
        A a = new A();
        B b = new B();
        A baseA = b;
        // a.test = b;
        a.test = baseA;
        // Debug.Log(a.test is B);
        PTest(baseA);
    }

    private void PTest(A test)
    {
        Debug.Log(test is B);
    }

}
