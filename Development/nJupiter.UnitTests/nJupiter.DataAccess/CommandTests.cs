using System.Data;

using FakeItEasy;

using nJupiter.DataAccess;

using NUnit.Framework;

namespace nJupiter.UnitTests.nJupiter.DataAccess {

	[TestFixture]
	public class CommandTests {

		[Test]
		public void Constructor_CreatesConnection_ReturnsCommandWithCorrectTransaction() {
			var dataSource = A.Fake<IDataSource>();
			var dbCommand = A.Fake<IDbCommand>();
			var dbTransaction = TransactionFactory.BeginTransaction(dataSource);
			var command = new Command(dbCommand, dbTransaction, null);
			Assert.AreEqual(dbTransaction, command.Transaction);
			Assert.AreEqual(dbTransaction.Connection, dbCommand.Connection);
			Assert.AreEqual(dbTransaction, dbCommand.Transaction);
		}

		[Test]
		public void Constructor_CreatesConnectionWithParameters_ReturnsCommandWithRithtNumberOfParameters() {
			var dbCommand = A.Fake<IDbCommand>();
			var dbTransaction = A.Fake<IDbTransaction>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);

			var parameters = new IDataParameter[12];
			
			new Command(dbCommand, dbTransaction, parameters);

			A.CallTo(() => parameterCollection.Add(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Times(12));

		}

		[Test]
		public void AddInParameter_CreateParameterWithNameAndType_CorrectParameterAdded() {
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);
			
			IDataParameter parameter = null;

			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).WithAnyArguments().Invokes(x => parameter = x.GetArgument<IDataParameter>(0)).Returns(1);

			var command = new Command(dbCommand, null, null);

			command.AddInParameter("MyParameter", DbType.AnsiString);

			Assert.IsNotNull(parameter);
			Assert.AreEqual("MyParameter", parameter.ParameterName);
			Assert.AreEqual(DbType.AnsiString, parameter.DbType);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);

		}


		[Test]
		public void AddInParameter_CreateParameterWithNameAndObject_CorrectParameterAdded() {
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);
			
			IDataParameter parameter = null;

			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).WithAnyArguments().Invokes(x => parameter = x.GetArgument<IDataParameter>(0)).Returns(1);

			var command = new Command(dbCommand, null, null);

			var dummyObject = A.Dummy<MyDummyClass>();

			command.AddInParameter("MyParameter", DbType.Date, dummyObject);

			Assert.IsNotNull(parameter);
			Assert.AreEqual("MyParameter", parameter.ParameterName);
			Assert.AreEqual(DbType.Date, parameter.DbType);
			Assert.AreEqual(dummyObject, parameter.Value);
			Assert.AreEqual(ParameterDirection.Input, parameter.Direction);

		}

		[Test]
		public void AddOutParameter_CreateParameterWithNameAndParameter_CorrectParameterAdded() {
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);
			
			IDataParameter parameter = null;

			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).WithAnyArguments().Invokes(x => parameter = x.GetArgument<IDataParameter>(0)).Returns(1);

			var command = new Command(dbCommand, null, null);

			command.AddOutParameter("MyParameter", DbType.Decimal, 123);

			Assert.IsTrue(parameter is IDbDataParameter);
			Assert.IsNotNull(parameter);
			Assert.AreEqual("MyParameter", parameter.ParameterName);
			Assert.AreEqual(DbType.Decimal, parameter.DbType);
			Assert.AreEqual(ParameterDirection.Output, parameter.Direction);
			Assert.AreEqual(123, ((IDbDataParameter)parameter).Size);

		}

		[Test]
		public void AddParameter_CreateParameter_CorrectParameterAdded() {
			var dbCommand = A.Fake<IDbCommand>();
			var parameterCollection = A.Fake<IDataParameterCollection>();
			A.CallTo(() => dbCommand.Parameters).Returns(parameterCollection);
			
			IDataParameter parameter = null;

			A.CallTo(() => parameterCollection.Add(A<object>.Ignored)).WithAnyArguments().Invokes(x => parameter = x.GetArgument<IDataParameter>(0)).Returns(1);

			var command = new Command(dbCommand, null, null);

			var dummyObject = A.Dummy<MyDummyClass>();

			command.AddParameter("MyParameter", DbType.Guid, 242, ParameterDirection.InputOutput, 1, 1, "myColumn", DataRowVersion.Original, dummyObject);
			IDbDataParameter dbParamter = parameter as IDbDataParameter;
			Assert.IsNotNull(dbParamter);
			Assert.AreEqual("MyParameter", dbParamter.ParameterName);
			Assert.AreEqual(DbType.Guid, dbParamter.DbType);
			Assert.AreEqual(ParameterDirection.InputOutput, dbParamter.Direction);
			Assert.AreEqual(242, dbParamter.Size);
			Assert.AreEqual(DataRowVersion.Original, dbParamter.SourceVersion);
			Assert.AreEqual(1, dbParamter.Precision);
			Assert.AreEqual(1, dbParamter.Scale);
			Assert.AreEqual("myColumn", dbParamter.SourceColumn);
			Assert.AreEqual(dummyObject, dbParamter.Value);
		}

		class MyDummyClass{}

	}
}