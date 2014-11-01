#Asyncexec (NAnt)

- What is this?

This is a port of Asyncexec task from below library. (All credits goes to the original developer. I have only done some trimming and modifications to the original work to make it work in small form.)  
https://code.google.com/p/ci-factory/

I couldn't/didn't wanted to get the full library working. So I ported only this task.  This task can be used to run NAnt targets in parallel.  The logs of async targets can be redirected to console using property "redirectoutput".   To wait for all the async tasks to finish - we also have another task here .  Moreover, please note that as the Async tasks are running in parellel - the logs from different tasks are intermixed in this task.)


How to use it:-

```xml
<target name="mytarget" >
    <asyncexec program="${PathToMyExe}" redirectoutput="true" taskname="batch1_task"  >
      <arg line="/param1:paramvalue" />
    </asyncexec>
    <asyncexec program="${PathToMyExe}" redirectoutput="true" taskname="batch2_task" >
      <arg line="/param1:paramvalue" />
    </asyncexec>
	<waitforexitasync>
		<tasknames>
			<string value="batch1_task" />
			<string value="batch2_task" />
		</tasknames>
	</waitforexitasync>
</target>
```