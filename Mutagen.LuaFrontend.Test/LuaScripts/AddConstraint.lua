CreateFacette("F02", {1, 3, 5, 6})
CreateFacette("F01", {7, 8})

--function F01Constraint(valuesOfF01)
--{
--    return false
--}

-- Params:
-- Constraint Source
-- Constraint Target
-- Guard
-- Values to constrain to
CreateConstraint("F01", "F02", "F01Constraint", {1} )