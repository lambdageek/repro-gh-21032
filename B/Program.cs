using System;

namespace B
{
    public class B : BBase<D.D> {
	public B () {}
    }

    public class BBase<T> {
	public BBase() {
	    Console.WriteLine ($"T is '{typeof(T).FullName}'");
	}
    }
}
