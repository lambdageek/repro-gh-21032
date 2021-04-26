using System;
using System.Threading;
using System.Reflection;

namespace X
{
    class Program
    {

	static string basePath;

	static Assembly LoadPlugin (string stem) {
	    var path = System.IO.Path.Combine (basePath, stem + "plugin", stem + ".dll");
	    Console.WriteLine ($" : LoadFrom('{path}') on {Thread.CurrentThread.Name}");
	    return Assembly.LoadFrom(path);
	}
        static void Main(string[] args)
        {

	    Thread.CurrentThread.Name = "Main-Thread";
	    
	    basePath = System.IO.Path.GetDirectoryName (typeof(Program).Assembly.Location);
	    
	    var conflictMonitor = new object ();
	    var waitForD = new ManualResetEventSlim ();
	    var waitForC = new ManualResetEventSlim ();

	    AppDomain.CurrentDomain.AssemblyResolve += (_sender, evtArgs) => {
		    Console.WriteLine ($"Try to load {evtArgs.Name} on behalf of '{evtArgs.RequestingAssembly}' on {Thread.CurrentThread.Name}");
		    var name = evtArgs.Name;
		    if (name.StartsWith ("B,")) {
			return LoadPlugin ("B");
		    }
		    if (name.StartsWith ("D,")) {
			waitForD.Set(); // allow Load-C-Thread to proceed
			Monitor.Enter (conflictMonitor); // this should block, but C should succeed
			while (true) {
			    Console.WriteLine ($"  :: Thread {Thread.CurrentThread.Name} waiting for waitForC for 1s");
			    // blocks until C is successfully loaded
			    if (waitForC.Wait (1000))
				break;
			}
			return LoadPlugin ("D");
		    }
		    if (name.StartsWith ("C,")) {
			var res = LoadPlugin ("C");
			waitForC.Set ();
			return res;
		    }
		    return null;
	    };
	    AppDomain.CurrentDomain.AssemblyLoad += (_sender, evt) => {
		var assm = evt.LoadedAssembly;
		var name = assm.GetName();
		Console.WriteLine ($" : Loaded {name.Name} from '{assm.Location}' on {Thread.CurrentThread.Name}");
	    };

	    
	    var allDone = new ManualResetEventSlim ();
	    var proceed = new ManualResetEventSlim ();

	    var conflictThread = new Thread(() => {
		lock (conflictMonitor) { // make D resolver deadlock.
		    proceed.Set();
		    while (true) {
			Console.WriteLine ($" - {Thread.CurrentThread.Name} waiting for all done for 1s");
			if (allDone.Wait(1000))
			    break;
		    }
		}
	    });
	    conflictThread.Name = "Conflict-Thread";
	    conflictThread.Start();

	    proceed.Wait ();
	    // At this point conflict thread entered conflictMonitor and is waitinf for allDone;

	    var t2 = new Thread(() => {
		Console.WriteLine ($"  :: Thread {Thread.CurrentThread.Name} waiting for waitForD");
		waitForD.Wait (); // don't start anything until we attempt to load D
		var cType = TryLoad ("C.C, C, Version=1.0.0.0, Culture=neutral");
		allDone.Set ();
		Activator.CreateInstance(cType);
	    });
	    t2.Name = "Load-C-Thread";
	    t2.Start();

	    // t2 tries to load C and blocks on waitForD
		  
	    // the main thread tries to load B, which takes the runtime loader lock,
	    // then tries to load D, which sets waitForD and then tries to enter conflictMonitor and blocks.
	    // the C loader proceeds and loads the C assembly, and sets allDone.
	    // The conflict thread gets allDone and continues (while still holding the monitor).
	    // So the main thread will finish loading D (and hence B) once C succeeds.
	    //
	    // If the runtime holds the loader lock while loading B and it prevents the creation of C, the process will deadlock.
	    var tyName = "B.B, B, Version=1.0.0.0, Culture=neutral";
	    TryLoad (tyName);
	    
        }

	private static Type TryLoad (string tyName) {
	    var ty = Type.GetType (tyName);

	    if (ty == null) {
		Console.WriteLine ($"Couldn't load '{tyName}'");
	    } else {
		Console.WriteLine ($"Loaded {ty.FullName} from '{ty.Assembly}'");
	    }
	    return ty;
	}
	
    }
}
