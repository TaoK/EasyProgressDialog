
### (Original) Introduction

When developing Windows Forms applications, you (or at least I) typically implement your business classes, hook them up to your forms / UI classes, and only afterwards start thinking about the <u>timing</u> of your program and user experience - which processes are going to take a fraction of a second, which processes 5-10 seconds, and which processes are going to take a couple of minutes.

If you find that you do have processes that run longer than a second or two, you rapidly need to start thinking about threading (dedicated thread, threadpool, backgroundworker?), progress bars, the consequences to the rest of your UI while some thread is doing stuff and you display progress / status somewhere, etc.

Then, as you start to hook it all up, you (or at least I) realize that you will need to put all sorts of checks in the classes actually doing the work, and your code starts to feel "dirty" as your "work" code contains more and more concessions to progress display...

None of that should be required with a simple effective progress dialog solution. You should be able to just call a work method in your UI handler, and have your progress dialog automatically block the UI from the user, giving them an idea of what is going on and hopefully some control. This should be as little disruptive as possible to your existing code, and not require it to be aware of multiple UI classes.

You do a quick search for a simple-to-implement progress dialog solution in .Net, and find... nothing? Well, hopefully no longer! The simple dialog presented here should enable you to do just that - pass in a method, and the progress dialog will display until the method completes. Add some checkpoints in your work method, and your user can potentially cancel partway and/or see what is happening! 

Here's an example "Action Shot":

![Progress Dialog Example](http://taok.github.com/EasyProgressDialog/img/progressdialogsample.png)

#### References

Some of the places I looked when I was working on this, a long long time ago:

* http://www.mikedub.net/mikeDubSamples/SafeReallySimpleMultithreadingInWindowsForms20/SafeReallySimpleMultithreadingInWindowsForms20.htm
* http://www.pcreview.co.uk/forums/generic-progress-bar-dialog-backgroundworker-t3224601.html
* http://www.codeproject.com/Articles/31906/Progress-O-Doom
* http://www.aspnet-answers.com/microsoft/NET-Windows-Forms/30616342/a-generic-progress-bar-dialog-with-backgroundworker.aspx


### Disclaimer / Caveat

These days, Microsoft is pushing asynchronous UI development pretty hard, and there are all sorts of frameworks and tutorials out there to help you make better UIs. The latest Outlook, for example, is a pleasure to use compared to its predecessors, primarily because of its higher responsiveness resulting from many minor async-handling additions. I must sadly admit that I simply haven't looked into this stuff at all yet. I'll update this area with better advice at some later time (although [this looks promising](http://lukhezo.com/2012/04/15/writing-multithreaded-programs-using-async-await-in-c/)). For now, I need to publih this to at a minimum support other projects that I'm checking in!

