# Mutagen Lua Frontend

## Anatomy of Lua Testcase
Constructing a Lua Testcase is a matter of calling a couple of
functions and implementing the TC function, e.g.

```Lua
-- Define the assembly that contains the backend for the TC and the class
-- that implements the backend.
BeginTestCase("TestAssembly", "TestClass")

-- Note, that the facette is added to the active testcase
AddFacette("FacName", 1, 1)

-- As soon as the interpreter comes past this statement
-- it will start the actual test with the current context
-- and call "ExecuteTest" for each facette binding available.
EndTestCase()

-- The function that contains our testlogic.
function ExecuteTest()
  __ASSERT(SomeCondition)
  __ASSERT(SomeOtherCondition)
end;
```
## Accessing the Backend/Testharness
As soon as a testcase was begun, the runtime environment should instanciate
the provided harness and add all exported functions to the scope the script.

Example:
Given, a Testharness thet provides a method called "Foo".

```Lua
-- Throws exception, as Foo is not known yet.
Foo()
BeginTestCase("FooAssembly.dll", "FooClass")

-- Works now, as the Assembly was loaded.
Foo()
```

## Running a Lua Testcase
TBD

## How to deal with testresults
TBD
