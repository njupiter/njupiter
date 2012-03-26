namespace nJupiter.DataAccess.Users {
	public enum CompareCondition {
		Equal,
		NotEqual,
		GreaterThan,
		GreaterThanOrEqual,
		NotLessThan = GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotGreaterThan = LessThanOrEqual,
		StartsWith,
		NotStartsWith,
		EndsWith,
		NotEndsWith,
		Contains,
		NotContains,
		ContainsStartsWith
	}
}