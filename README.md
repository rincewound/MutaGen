# MutaGen
A Testsystem for permutation based testing

Build: ![BuildState](https://travis-ci.org/rincewound/MutaGen.svg?branch=master)


![Public Trello Board](https://trello.com/b/N6uAVONt/mutagen)

##Introduction
The idea for MutaGen arose from the need to automatically test a wide range of configurations of a software that should exhibit similar behavior. After first investigating Fitnesse and Cucumber a couple of new concepts were developed to facilitate permutations in testcases.

##Concepts
A facette contains a configurable value, e.g. a range of values the testsystem should apply to a testcase. A facette can contain any type of value (or indeed lists of values).

A Scenario is what would commonly be called a testcase. Each scenario consists of a list of facettes that are to be applied and the actual testcase. The latter can be, in its simplest form, a list of testable (i.e. "true/false" statements). The runtime environment will create all possible combinations for each facette, that was included in the scenario and run the scenario for all these combinations ("bindings").

##Architecture
The architecture is inspired by fitnesse, albeit I chose an open frontend for the Scenarios, i.e. a fixed runtime, that will use a backend for the SUT/DUT, that has to be provided by the user. The runtime is invoked by an arbitrary frontend (i.e. a testcase provider!). So, we end up with something like this:

Scenario --- Interpreted by ---> Frontend --- Invokes ---> Runtime --- Invokes ---> Backend
[<--------------Scheme----------------------------------->][<-- C# --------------->][Any .Net, user code]

##DSL
The DSL I have in mind is closely related to what Cucumber does with Gherkin, however, when I did some prototyping, I noticed, that I was essentially using something that appears to be very closely related to Lisp. Given that the whole business with facettes is very much listprocessing I opted to create the very first frontend using IronScheme. As I do know, that Lisp has not all that much going for it these days (popularity wise!), I took the approach of seperating the frontend from the backend, thus giving the user the option to use another language to drive the backend.

##Current state
As of now, I don't have a working version yet. Expect the first bits of code within
Jan '17 and - hopefully - something buildable a couple of weeks later.

* Jan '17: Moved to Github
* Dec '16: Started implementation on runtime, experiments for scenarios
