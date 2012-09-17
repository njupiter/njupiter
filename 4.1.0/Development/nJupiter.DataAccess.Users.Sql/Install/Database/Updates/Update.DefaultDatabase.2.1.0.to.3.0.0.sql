UPDATE p
SET PropertyValue = RIGHT('0000000000' + CAST(CAST(PropertyValue AS BIGINT) + 2147483648 AS VARCHAR(10)), 10)
FROM USER_Property p JOIN
	USER_PropertySchema ps ON p.PropertyID = ps.PropertyID
WHERE ps.DataTypeID = 1

UPDATE p
SET PropertyValue = RIGHT('0000000000000000000' + PropertyValue, 19)
FROM USER_Property p JOIN
	USER_PropertySchema ps ON p.PropertyID = ps.PropertyID
WHERE ps.DataTypeID = 3