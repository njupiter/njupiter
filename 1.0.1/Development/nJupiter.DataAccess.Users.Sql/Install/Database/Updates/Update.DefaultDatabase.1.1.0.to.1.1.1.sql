SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


/*
Name:       	USER_UpdateField
Description:	Updates values in USER_fieldValue
Input:		guidUserID
		intFieldID
		chvnFieldValue
		txtnExtFieldValue
		chvContext
Output:		-
*/
ALTER  PROCEDURE dbo.USER_UpdateProperty
	@guidUserID UNIQUEIDENTIFIER,
	@chvProperty VARCHAR(255),
	@chvnPropertyValue NVARCHAR(4000),
	@txtnExtPropertyValue NTEXT,
	@chvContext VARCHAR(255) = NULL
AS
	IF EXISTS(
	  SELECT p.PropertyID, UserID 
	  FROM	USER_Property p INNER JOIN 
		USER_PropertySchema ps ON p.PropertyID = ps.PropertyID LEFT JOIN
		USER_Context c ON p.ContextID = c.ContextID 
	  WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND 
		   ((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext))
		UPDATE USER_Property
		SET PropertyValue = @chvnPropertyValue, 
		ExtendedPropertyValue = @txtnExtPropertyValue
		FROM 	USER_Property p INNER JOIN 
			USER_PropertySchema ps ON p.PropertyID = ps.PropertyID LEFT JOIN
			USER_Context c ON p.ContextID = c.ContextID
		WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND 
			((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext)
	ELSE
		INSERT INTO USER_Property(PropertyID, UserID, PropertyValue, ExtendedPropertyValue, ContextID)
		SELECT PropertyID, @guidUserID, @chvnPropertyValue, @txtnExtPropertyValue, ISNULL((SELECT ContextID FROM USER_Context WHERE ContextName = @chvContext), 0)
		FROM USER_PropertySchema ps
		WHERE PropertyName = @chvProperty

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO