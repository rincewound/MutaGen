CreateFacette("F01", {1, 2})
CreateFacette("F02", {3, 4});

BeginTestCase("Mutagen.LuaFrontend.Test.SimpleHarness", "Mutagen.LuaFrontEnd.Test.dll")
AddFacette("F01", 1, 2)
AddFacette("F02", 1, 1)

CommitTestCaseCode()

function ExecuteTest()
  __ASSERT(true)
  __ASSERT(false)
  HarnessPrint("fnord")
end;
