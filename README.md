
Repro for https://github.com/mono/mono/issues/21032 and https://github.com/dotnet/runtime/issues/51864

```console
$ cd X
$ dotnet build

$ dotnet run -f net6.0 # should succeed

$ mono bin/Debug/net472/X.exe  # should hang

```

Stack trace (`Mono JIT compiler version 6.12.0.114 (2020-02/5e9cb6d1c1d Thu Dec 10 04:55:48 EST 2020)`)

```
(lldb) bt all
* thread #1, name = 'Main-Thread', queue = 'com.apple.main-thread', stop reason = signal SIGSTOP
  * frame #0: 0x00007fff2040d8e2 libsystem_kernel.dylib`__psynch_cvwait + 10
    frame #1: 0x00007fff2043fe6f libsystem_pthread.dylib`_pthread_cond_wait + 1254
    frame #2: 0x0000000105b036b0 mono`mono_monitor_try_enter_inflated [inlined] mono_os_cond_wait(cond=0x00007fa132d082e0, mutex=0x00007fa132d08260) at mono-os-mutex.h:219:8 [opt]
    frame #3: 0x0000000105b036a3 mono`mono_monitor_try_enter_inflated at mono-coop-mutex.h:91 [opt]
    frame #4: 0x0000000105b03685 mono`mono_monitor_try_enter_inflated(obj=0x0000000106000a48, ms=4294967295, allow_interruption=<unavailable>, id=64) at monitor.c:934 [opt]
    frame #5: 0x0000000105b0223e mono`mono_monitor_try_enter_loop_if_interrupted(obj=0x0000000106000a48, ms=4294967295, allow_interruption=<unavailable>, lockTaken=<unavailable>, error=<unavailable>) at monitor.c:1198:9 [opt]
    frame #6: 0x0000000105b021ee mono`mono_monitor_enter_internal(obj=<unavailable>) at monitor.c:1055:9 [opt]
    frame #7: 0x0000000105dfa839
    frame #8: 0x0000000105df1b1f
    frame #9: 0x00000001058859d2 mono`mono_jit_runtime_invoke(method=<unavailable>, obj=<unavailable>, params=0x00007ffeea38b8d0, exc=0x0000000106000a48, error=<unavailable>) at mini-runtime.c:3191:12 [opt]
    frame #10: 0x0000000105a96972 mono`mono_runtime_try_invoke [inlined] do_runtime_invoke(method=<unavailable>, obj=0x0000000105e70130, params=0x00007ffeea38b8d0, exc=0x00007ffeea38b8c0, error=0x00007ffeea38b988) at object.c:3052:11 [opt]
    frame #11: 0x0000000105a96936 mono`mono_runtime_try_invoke(method=0x00007fa153813248, obj=0x0000000105e70130, params=0x00007ffeea38b8d0, exc=<unavailable>, error=0x00007ffeea38b988) at object.c:3161 [opt]
    frame #12: 0x00000001059fb212 mono`mono_try_assembly_resolve_handle(alc=<unavailable>, fname=MonoStringHandle @ r15, requesting=0x00007fa152c2bcf0, refonly=0, error=<unavailable>) at appdomain.c:1447:11 [opt]
    frame #13: 0x00000001059fb019 mono`mono_try_assembly_resolve(alc=<unavailable>, fname_raw="D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", requesting=0x00007fa152c2bcf0, refonly=0, error=0x00007ffeea38b988) at appdomain.c:1407:11 [opt]
    frame #14: 0x00000001059f8924 mono`mono_domain_assembly_postload_search(alc=<unavailable>, requesting=<unavailable>, aname=<unavailable>, refonly=<unavailable>, postload=<unavailable>, user_data=<unavailable>, error_out=0x00007ffeea38ba20) at appdomain.c:1510:13 [opt]
    frame #15: 0x0000000105a03881 mono`mono_assembly_invoke_search_hook_internal(alc=0x0000000000000000, requesting=0x00007fa152c2bcf0, aname=0x00007ffeea38bd50, refonly=0, postload=1) at assembly.c:2029:11 [opt]
    frame #16: 0x0000000105a06ae2 mono`mono_assembly_request_byname(aname=<unavailable>, req=<unavailable>, status=<unavailable>) at assembly.c:4765:12 [opt]
    frame #17: 0x0000000105a03037 mono`mono_assembly_load_reference [inlined] load_reference_by_aname_loadfrom_asmctx at assembly.c:1583:14 [opt]
    frame #18: 0x0000000105a02fee mono`mono_assembly_load_reference(image=0x00007fa153033a00, index=<unavailable>) at assembly.c:1818 [opt]
    frame #19: 0x0000000105a0ad1d mono`mono_class_from_typeref_checked(image=0x00007fa153033a00, type_token=<unavailable>, error=0x00007ffeea38cb18) at class.c:206:3 [opt]
    frame #20: 0x0000000105a0ef3e mono`mono_class_get_checked(image=0x00007fa153033a00, type_token=16777228, error=0x00007ffeea38cb18) at class.c:2741:11 [opt]
    frame #21: 0x0000000105a71bc2 mono`mono_metadata_parse_type_internal at metadata.c:4052:11 [opt]
    frame #22: 0x0000000105a71974 mono`mono_metadata_parse_type_internal(m=<unavailable>, container=0x0000000000000000, opt_attrs=0, transient=<unavailable>, ptr=<unavailable>, rptr=<unavailable>, error=<unavailable>) at metadata.c:1907 [opt]
    frame #23: 0x0000000105a7577f mono`mono_metadata_parse_generic_inst [inlined] mono_metadata_parse_type_checked(m=0x00007fa153033a00, container=0x0000000000000000, opt_attrs=0, transient=1, ptr="\x121\x02\x13", rptr=<unavailable>, error=0x00007ffeea38cb18) at metadata.c:1961:9 [opt]
    frame #24: 0x0000000105a7575e mono`mono_metadata_parse_generic_inst(m=0x00007fa153033a00, container=0x0000000000000000, count=<unavailable>, ptr="\x121\x02\x13", rptr=0x00007ffeea38c178, error=0x00007ffeea38cb18) at metadata.c:3641 [opt]
    frame #25: 0x0000000105a71df7 mono`mono_metadata_parse_type_internal [inlined] do_mono_metadata_parse_generic_class(container=0x0000000000000000, ptr="\x121\x02\x13") at metadata.c:3685:9 [opt]
    frame #26: 0x0000000105a71dc1 mono`mono_metadata_parse_type_internal [inlined] do_mono_metadata_parse_type(transient=<unavailable>, ptr="\x12\f\x01\x121\x02\x13", error=<unavailable>) at metadata.c:4105 [opt]
    frame #27: 0x0000000105a71dc1 mono`mono_metadata_parse_type_internal(m=<unavailable>, container=0x0000000000000000, opt_attrs=0, transient=<unavailable>, ptr=<unavailable>, rptr=<unavailable>, error=<unavailable>) at metadata.c:1907 [opt]
    frame #28: 0x0000000105a79387 mono`mono_type_create_from_typespec_checked [inlined] mono_metadata_parse_type_checked(m=0x00007fa153033a00, container=0x0000000000000000, opt_attrs=0, transient=1, rptr=<unavailable>, error=0x00007ffeea38cb18) at metadata.c:1961:9 [opt]
    frame #29: 0x0000000105a79365 mono`mono_type_create_from_typespec_checked(image=0x00007fa153033a00, type_spec=452984833, error=0x00007ffeea38cb18) at metadata.c:6496 [opt]
    frame #30: 0x0000000105a0ef4e mono`mono_class_get_checked [inlined] mono_type_retrieve_from_typespec(image=0x00007fa153033a00, type_spec=452984833, error=0x00007ffeea38cb18) at class.c:2099:16 [opt]
    frame #31: 0x0000000105a0ef40 mono`mono_class_get_checked [inlined] mono_class_create_from_typespec at class.c:2132 [opt]
    frame #32: 0x0000000105a0ef40 mono`mono_class_get_checked(image=0x00007fa153033a00, type_token=452984833, error=0x00007ffeea38cb18) at class.c:2744 [opt]
    frame #33: 0x0000000105a16858 mono`mono_class_create_from_typedef(image=0x00007fa153033a00, type_token=<unavailable>, error=<unavailable>) at class-init.c:498:12 [opt]
    frame #34: 0x0000000105a0eeec mono`mono_class_get_checked(image=0x00007fa153033a00, type_token=33554434, error=0x00007ffeea38cb18) at class.c:2738:11 [opt]
    frame #35: 0x0000000105a0fb8b mono`mono_class_from_name_checked_aux(image=<unavailable>, name_space=<unavailable>, name="B", visited_images=<unavailable>, case_sensitive=<unavailable>, error=<unavailable>) at class.c:3253:10 [opt]
    frame #36: 0x0000000105a0af6a mono`mono_class_from_name_checked(image=<unavailable>, name_space=<unavailable>, name=<unavailable>, error=<unavailable>) at class.c:3279:10 [opt]
    frame #37: 0x0000000105ad3db5 mono`mono_reflection_get_type_internal(alc=0x0000000000000000, rootimage=0x00007fa143008200, image=0x00007fa153033a00, info=0x00007ffeea38ca58, ignorecase=0, search_mscorlib=1, error=<unavailable>) at reflection.c:2008:11 [opt]
    frame #38: 0x0000000105ad17d0 mono`mono_reflection_get_type_with_rootimage(alc=0x0000000000000000, rootimage=0x00007fa143008200, image=0x00007fa153033a00, info=0x00007ffeea38ca58, ignorecase=0, search_mscorlib=1, type_resolve=<unavailable>, error=0x00007ffeea38cb18) at reflection.c:2249:10 [opt]
    frame #39: 0x0000000105a32d87 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name [inlined] type_from_parsed_name(ignoreCase=<unavailable>, error=0x00007ffeea38cb18) at icall.c:1830:10 [opt]
    frame #40: 0x0000000105a32cf2 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name(name=<unavailable>, stack_mark=<unavailable>, callerAssembly=<unavailable>, throwOnError=<unavailable>, ignoreCase='\0', reflectionOnly=<unavailable>, error=0x00007ffeea38cb18) at icall.c:1886 [opt]
    frame #41: 0x0000000105a4f8a0 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name_raw(a0=0x00007ffeea38cbf0, a1=0x00007ffeea38ccb0, a2=<unavailable>, a3='\0', a4='\0', a5=<unavailable>) at icall-def.h:908:1 [opt]
    frame #42: 0x0000000105df1517
    frame #43: 0x00000001079a1e5d mscorlib.dll.dylib`System_RuntimeType_GetType_string_bool_bool_bool_System_Threading_StackCrawlMark_ + 93
    frame #44: 0x000000010796d28e mscorlib.dll.dylib`System_Type_GetType_string + 62
    frame #45: 0x0000000105df0d23
  thread #2, name = 'SGen worker'
    frame #0: 0x00007fff2040d8e2 libsystem_kernel.dylib`__psynch_cvwait + 10
    frame #1: 0x00007fff2043fe6f libsystem_pthread.dylib`_pthread_cond_wait + 1254
    frame #2: 0x0000000105b54c0e mono`thread_func [inlined] mono_os_cond_wait(mutex=<unavailable>) at mono-os-mutex.h:219:8 [opt]
    frame #3: 0x0000000105b54bfb mono`thread_func at sgen-thread-pool.c:165 [opt]
    frame #4: 0x0000000105b54bed mono`thread_func(data=0x0000000000000000) at sgen-thread-pool.c:196 [opt]
    frame #5: 0x00007fff2043f950 libsystem_pthread.dylib`_pthread_start + 224
    frame #6: 0x00007fff2043b47b libsystem_pthread.dylib`thread_start + 15
  thread #3, name = 'Finalizer'
    frame #0: 0x00007fff2040aeba libsystem_kernel.dylib`semaphore_wait_trap + 10
    frame #1: 0x0000000105b017ea mono`finalizer_thread [inlined] mono_os_sem_wait(flags=MONO_SEM_FLAGS_ALERTABLE) at mono-os-semaphore.h:84:8 [opt]
    frame #2: 0x0000000105b017df mono`finalizer_thread at mono-coop-semaphore.h:41 [opt]
    frame #3: 0x0000000105b017c5 mono`finalizer_thread(unused=<unavailable>) at gc.c:965 [opt]
    frame #4: 0x0000000105abfdad mono`start_wrapper [inlined] start_wrapper_internal at threads.c:1233:3 [opt]
    frame #5: 0x0000000105abfd6a mono`start_wrapper(data=0x00007fa152c11040) at threads.c:1308 [opt]
    frame #6: 0x00007fff2043f950 libsystem_pthread.dylib`_pthread_start + 224
    frame #7: 0x00007fff2043b47b libsystem_pthread.dylib`thread_start + 15
  thread #4, name = 'Conflict-Thread'
    frame #0: 0x00007fff2040d8e2 libsystem_kernel.dylib`__psynch_cvwait + 10
    frame #1: 0x00007fff2043fea2 libsystem_pthread.dylib`_pthread_cond_wait + 1305
    frame #2: 0x0000000105b65ed4 mono`mono_os_cond_timedwait(cond=0x00007fa15400ca30, mutex=0x00007fa15400c9f0, timeout_ms=1000) at mono-os-mutex.c:44:8 [opt]
    frame #3: 0x0000000105acc669 mono`mono_w32handle_timedwait_signal_handle at mono-coop-mutex.h:103:8 [opt]
    frame #4: 0x0000000105acc63c mono`mono_w32handle_timedwait_signal_handle [inlined] mono_w32handle_timedwait_signal_naked(poll=0, alerted=<unavailable>) at w32handle.c:652 [opt]
    frame #5: 0x0000000105acc63c mono`mono_w32handle_timedwait_signal_handle(handle_data=<unavailable>, timeout=<unavailable>, poll=0, alerted=<unavailable>) at w32handle.c:767 [opt]
    frame #6: 0x0000000105acc531 mono`mono_w32handle_wait_one(handle=<unavailable>, timeout=<unavailable>, alertable=<unavailable>) at w32handle.c:892:13 [opt]
    frame #7: 0x0000000105b02c6c mono`ves_icall_System_Threading_Monitor_Monitor_wait [inlined] mono_monitor_wait(allow_interruption='\x01') at monitor.c:1438:8 [opt]
    frame #8: 0x0000000105b02bf4 mono`ves_icall_System_Threading_Monitor_Monitor_wait(obj_handle=<unavailable>, ms=1000, error=<unavailable>) at monitor.c:1499 [opt]
    frame #9: 0x0000000105a50c31 mono`ves_icall_System_Threading_Monitor_Monitor_wait_raw(a0=0x000070000498a930, a1=1000) at icall-def.h:999:1 [opt]
    frame #10: 0x0000000105df04b5
    frame #11: 0x00000001079ff3b2 mscorlib.dll.dylib`System_Threading_Monitor_Wait_object_int_bool + 66
    frame #12: 0x00000001079ff461 mscorlib.dll.dylib`System_Threading_Monitor_Wait_object_int + 49
    frame #13: 0x00000001079f23c9 mscorlib.dll.dylib`System_Threading_ManualResetEventSlim_Wait_int_System_Threading_CancellationToken + 825
    frame #14: 0x00000001079f207d mscorlib.dll.dylib`System_Threading_ManualResetEventSlim_Wait_int + 61
    frame #15: 0x0000000105df06a3
    frame #16: 0x0000000107a000fb mscorlib.dll.dylib`System_Threading_ThreadHelper_ThreadStart_Context_object + 171
    frame #17: 0x00000001079fd9aa mscorlib.dll.dylib`System_Threading_ExecutionContext_RunInternal_System_Threading_ExecutionContext_System_Threading_ContextCallback_object_bool + 426
    frame #18: 0x00000001079fd7b3 mscorlib.dll.dylib`System_Threading_ExecutionContext_Run_System_Threading_ExecutionContext_System_Threading_ContextCallback_object_bool + 67
    frame #19: 0x00000001079fd728 mscorlib.dll.dylib`System_Threading_ExecutionContext_Run_System_Threading_ExecutionContext_System_Threading_ContextCallback_object + 104
    frame #20: 0x0000000107a00283 mscorlib.dll.dylib`System_Threading_ThreadHelper_ThreadStart + 67
    frame #21: 0x0000000105deff21
    frame #22: 0x00000001058859d2 mono`mono_jit_runtime_invoke(method=<unavailable>, obj=<unavailable>, params=0x000070000498af68, exc=0x0000000106000630, error=<unavailable>) at mini-runtime.c:3191:12 [opt]
    frame #23: 0x0000000105a94f97 mono`mono_runtime_invoke_checked [inlined] do_runtime_invoke(method=<unavailable>, obj=0x0000000106000d70, params=0x000070000498af68, exc=<unavailable>, error=0x000070000498af18) at object.c:3052:11 [opt]
    frame #24: 0x0000000105a94f62 mono`mono_runtime_invoke_checked(method=0x00007fa143021eb0, obj=0x0000000106000d70, params=0x000070000498af68, error=0x000070000498af18) at object.c:3220 [opt]
    frame #25: 0x0000000105a9b840 mono`mono_runtime_delegate_try_invoke(delegate=0x0000000106000d70, params=0x000070000498af68, exc=0x0000000000000000, error=0x000070000498af18) at object.c:4434:7 [opt]
    frame #26: 0x0000000105abfded mono`start_wrapper at threads.c:1255:3 [opt]
    frame #27: 0x0000000105abfd6a mono`start_wrapper(data=0x00007fa142c0f7e0) at threads.c:1308 [opt]
    frame #28: 0x00007fff2043f950 libsystem_pthread.dylib`_pthread_start + 224
    frame #29: 0x00007fff2043b47b libsystem_pthread.dylib`thread_start + 15
  thread #5, name = 'Load-C-Thread'
    frame #0: 0x00007fff2040d0c6 libsystem_kernel.dylib`__psynch_mutexwait + 10
    frame #1: 0x00007fff2043d2c5 libsystem_pthread.dylib`_pthread_mutex_firstfit_lock_wait + 81
    frame #2: 0x00007fff2043b1bc libsystem_pthread.dylib`_pthread_mutex_firstfit_lock_slow + 211
    frame #3: 0x0000000105a5aeb9 mono`mono_loader_lock [inlined] mono_os_mutex_lock(mutex=<unavailable>) at mono-os-mutex.h:105:8 [opt]
    frame #4: 0x0000000105a5aead mono`mono_loader_lock at mono-coop-mutex.h:57 [opt]
    frame #5: 0x0000000105a5ae74 mono`mono_loader_lock at loader.c:144 [opt]
    frame #6: 0x0000000105a1665f mono`mono_class_create_from_typedef(image=0x00007fa14303b800, type_token=33554434, error=0x0000700004b8d8f8) at class-init.c:438:2 [opt]
    frame #7: 0x0000000105a0eeec mono`mono_class_get_checked(image=0x00007fa14303b800, type_token=33554434, error=0x0000700004b8d8f8) at class.c:2738:11 [opt]
    frame #8: 0x0000000105a0fb8b mono`mono_class_from_name_checked_aux(image=<unavailable>, name_space=<unavailable>, name="C", visited_images=<unavailable>, case_sensitive=<unavailable>, error=<unavailable>) at class.c:3253:10 [opt]
    frame #9: 0x0000000105a0af6a mono`mono_class_from_name_checked(image=<unavailable>, name_space=<unavailable>, name=<unavailable>, error=<unavailable>) at class.c:3279:10 [opt]
    frame #10: 0x0000000105ad3db5 mono`mono_reflection_get_type_internal(alc=0x0000000000000000, rootimage=0x00007fa143008200, image=0x00007fa14303b800, info=0x0000700004b8d838, ignorecase=0, search_mscorlib=1, error=<unavailable>) at reflection.c:2008:11 [opt]
    frame #11: 0x0000000105ad17d0 mono`mono_reflection_get_type_with_rootimage(alc=0x0000000000000000, rootimage=0x00007fa143008200, image=0x00007fa14303b800, info=0x0000700004b8d838, ignorecase=0, search_mscorlib=1, type_resolve=<unavailable>, error=0x0000700004b8d8f8) at reflection.c:2249:10 [opt]
    frame #12: 0x0000000105a32d87 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name [inlined] type_from_parsed_name(ignoreCase=<unavailable>, error=0x0000700004b8d8f8) at icall.c:1830:10 [opt]
    frame #13: 0x0000000105a32cf2 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name(name=<unavailable>, stack_mark=<unavailable>, callerAssembly=<unavailable>, throwOnError=<unavailable>, ignoreCase='\0', reflectionOnly=<unavailable>, error=0x0000700004b8d8f8) at icall.c:1886 [opt]
    frame #14: 0x0000000105a4f8a0 mono`ves_icall_System_RuntimeTypeHandle_internal_from_name_raw(a0=0x0000700004b8d9d0, a1=0x0000700004b8da90, a2=<unavailable>, a3='\0', a4='\0', a5=<unavailable>) at icall-def.h:908:1 [opt]
    frame #15: 0x0000000105df1517
    frame #16: 0x00000001079a1e5d mscorlib.dll.dylib`System_RuntimeType_GetType_string_bool_bool_bool_System_Threading_StackCrawlMark_ + 93
    frame #17: 0x000000010796d28e mscorlib.dll.dylib`System_Type_GetType_string + 62
    frame #18: 0x0000000105df0d23
    frame #19: 0x00000001079fd7b3 mscorlib.dll.dylib`System_Threading_ExecutionContext_Run_System_Threading_ExecutionContext_System_Threading_ContextCallback_object_bool + 67
    frame #20: 0x00000001079fd728 mscorlib.dll.dylib`System_Threading_ExecutionContext_Run_System_Threading_ExecutionContext_System_Threading_ContextCallback_object + 104
    frame #21: 0x0000000107a00283 mscorlib.dll.dylib`System_Threading_ThreadHelper_ThreadStart + 67
    frame #22: 0x0000000105deff21
    frame #23: 0x00000001058859d2 mono`mono_jit_runtime_invoke(method=<unavailable>, obj=<unavailable>, params=0x0000700004b8df68, exc=0x0000000000000000, error=<unavailable>) at mini-runtime.c:3191:12 [opt]
    frame #24: 0x0000000105a94f97 mono`mono_runtime_invoke_checked [inlined] do_runtime_invoke(method=<unavailable>, obj=0x0000000106002078, params=0x0000700004b8df68, exc=<unavailable>, error=0x0000700004b8df18) at object.c:3052:11 [opt]
    frame #25: 0x0000000105a94f62 mono`mono_runtime_invoke_checked(method=0x00007fa143021eb0, obj=0x0000000106002078, params=0x0000700004b8df68, error=0x0000700004b8df18) at object.c:3220 [opt]
    frame #26: 0x0000000105a9b840 mono`mono_runtime_delegate_try_invoke(delegate=0x0000000106002078, params=0x0000700004b8df68, exc=0x0000000000000000, error=0x0000700004b8df18) at object.c:4434:7 [opt]
    frame #27: 0x0000000105abfded mono`start_wrapper at threads.c:1255:3 [opt]
    frame #28: 0x0000000105abfd6a mono`start_wrapper(data=0x00007fa152c0ff30) at threads.c:1308 [opt]
    frame #29: 0x00007fff2043f950 libsystem_pthread.dylib`_pthread_start + 224
    frame #30: 0x00007fff2043b47b libsystem_pthread.dylib`thread_start + 15
  thread #6
    frame #0: 0x00007fff2040c53e libsystem_kernel.dylib`__workq_kernreturn + 10
    frame #1: 0x00007fff2043c4fd libsystem_pthread.dylib`_pthread_wqthread + 414
    frame #2: 0x00007fff2043b467 libsystem_pthread.dylib`start_wqthread + 15
```
