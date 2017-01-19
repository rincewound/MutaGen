# MutaGen
A Testsystem for permutation based testing

Build: ![BuildState](https://travis-ci.org/rincewound/MutaGen.svg?branch=master)


[Public Trello Board](https://trello.com/b/N6uAVONt/mutagen)

##Introduction
The idea for MutaGen arose from the need to automatically test a wide range of configurations of a software that should exhibit similar behavior. After first investigating Fitnesse and Cucumber a couple of new concepts were developed to facilitate permutations in testcases.

##Concepts
A facette contains a configurable value, e.g. a range of values the testsystem should apply to a testcase. A facette can contain any type of value (or indeed lists of values).

A Scenario is what would commonly be called a testcase. Each scenario consists of a list of facettes that are to be applied and the actual testcase. The latter can be, in its simplest form, a list of testable (i.e. "true/false" statements). The runtime environment will create all possible combinations for each facette, that was included in the scenario and run the scenario for all these combinations ("bindings").

##Architecture
The architecture is inspired by fitnesse, albeit I chose an open frontend for the Scenarios, i.e. a fixed runtime, that will use a backend for the SUT/DUT, that has to be provided by the user. The runtime is invoked by an arbitrary frontend (i.e. a testcase provider!). So, we end up with something like this:

Scenario --- Interpreted by ---> Frontend --- Invokes ---> Runtime --- Invokes ---> Backend
[<--------------Lua-------------------------------------->][<-- C# --------------->][Any .Net, user code]

##Implementing a new backend
A backend is basically any .Net Class, that implements ITestharness and is publicly visible.
The backend is made available to the frontend by the runtime.

So, what is a backend?
A backend is a bit of code, that is used to drive the SUT/DUT. If we were to test
a webservice, that exposes the method "TestMethod", the backend should provide
means for the testcase to call this method.

##Current state
As of now, I don't have a working version yet. Expect the first bits of code within
Jan '17 and - hopefully - something buildable a couple of weeks later.

* Jan '17: Moved to Github, decided to use Lua instead of Scheme, as originally
  planned. Started cursing at Mono, because FakeItEasy won't work with it.
  Started cursing at NUnit, because a "Failure" won't fail a test (as opposed
  to an error.).
* Dec '16: Started implementation on runtime, experiments for scenarios
