using System.Web.UI;

using NUnit.Framework;

using nJupiter.Web.UI;

namespace nJupiter.Web.Tests.Unit.UI {
	
	[TestFixture]
	public class ControlFinderTests {

		[Test]
		public void FindControl_AddControlOnFirstLevel_ReturnsControl() {
			var rootControl = new Control();
			var control = new TestableControl("uniqueId");

			rootControl.Controls.Add(control);

			var result = ControlFinder.Instance.FindControl(rootControl, "uniqueId");

			Assert.AreSame(control, result);
		}

		[Test]
		public void FindControl_AddControlOnSecondLevel_ReturnsControl() {
			var rootControl = new Control();
			var parentControl = new Control();
			var control = new TestableControl("uniqueId");

			rootControl.Controls.Add(parentControl);
			parentControl.Controls.Add(control);

			var result = ControlFinder.Instance.FindControl(rootControl, "uniqueId");

			Assert.AreSame(control, result);
		}

		[Test]
		public void FindControl_AddControlOnFirstLevelButWithOtherId_ReturnsNull() {
			var rootControl = new Control();
			var control = new TestableControl("anotherUniqueId");

			rootControl.Controls.Add(control);

			var result = ControlFinder.Instance.FindControl(rootControl, "uniqueId");

			Assert.IsNull(result);
		}

		[Test]
		public void FindControl_DoNotEnsureChildControls_FindControlIsNotCalled() {
			var rootControl = new TestableControl();

			ControlFinder.Instance.FindControl(rootControl, "uniqueId", false);

			Assert.IsFalse(rootControl.FindControlIsCalled);
		}		
		
		[Test]
		public void FindControl_EnsureChildControls_FindControlIsCalled() {
			var rootControl = new TestableControl();

			ControlFinder.Instance.FindControl(rootControl, "uniqueId", true);

			Assert.IsTrue(rootControl.FindControlIsCalled);
		}

		[Test]
		public void FindFirstControlOnType_EnsureChildControls_FindControlIsCalled() {
			var rootControl = new TestableControl();

			ControlFinder.Instance.FindFirstControlOnType<IAnyInterface>(rootControl, true);

			Assert.IsTrue(rootControl.FindControlIsCalled);
		}

		[Test]
		public void FindFirstControlOnType_AddControlInheritingFromInterface_ReturnsControl() {
			var rootControl = new Control();
			var control = new TestableControlInheritingInterface("uniqueId");

			rootControl.Controls.Add(control);

			var result = ControlFinder.Instance.FindFirstControlOnType<IAnyInterface>(rootControl);

			Assert.AreSame(control, result);
		}

		[Test]
		public void FindFirstControlOnType_AddTwoControlsInheritingFromInterface_ReturnsFirstControl() {
			var rootControl = new Control();
			var firstControl = new TestableControlInheritingInterface("firstControl");
			var secondControl = new TestableControlInheritingInterface("secondControl");

			rootControl.Controls.Add(firstControl);
			rootControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindFirstControlOnType<IAnyInterface>(rootControl);

			Assert.AreSame(firstControl, result);
		}


		[Test]
		public void FindControlsOnType_AddTwoControlsOnSameLevelInheritingFromInterface_ReturnsControls() {
			var rootControl = new Control();
			var firstControl = new TestableControlInheritingInterface("firstControl");
			var secondControl = new TestableControlInheritingInterface("secondControl");

			rootControl.Controls.Add(firstControl);
			rootControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindControlsOnType<IAnyInterface>(rootControl);

			Assert.Contains(firstControl, result);
			Assert.Contains(secondControl, result);
		}

		[Test]
		public void FindControlsOnType_AddTwoControlsInDiefferentLevelsInheritingFromInterface_ReturnsControls() {
			var rootControl = new Control();
			var firstControl = new TestableControlInheritingInterface("firstControl");
			var secondControl = new TestableControlInheritingInterface("secondControl");

			rootControl.Controls.Add(firstControl);
			firstControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindControlsOnType<IAnyInterface>(rootControl);

			Assert.Contains(firstControl, result);
			Assert.Contains(secondControl, result);
		}


		[Test]
		public void FindFirstControlOnType_AddControlOfOtherType_ReturnsControl() {
			var rootControl = new Control();
			var control = new TestableControlOfOtherType("uniqueId");

			rootControl.Controls.Add(control);

			var result = ControlFinder.Instance.FindFirstControlOnType(rootControl, typeof(TestableControlOfOtherType));

			Assert.AreSame(control, result);
		}

		[Test]
		public void FindFirstControlOnType_AddTwoControlsOfOtherType_ReturnsFirstControl() {
			var rootControl = new Control();
			var firstControl = new TestableControlOfOtherType("firstControl");
			var secondControl = new TestableControlOfOtherType("secondControl");

			rootControl.Controls.Add(firstControl);
			rootControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindFirstControlOnType(rootControl, typeof(TestableControlOfOtherType));

			Assert.AreSame(firstControl, result);
		}


		[Test]
		public void FindControlsOnType_AddTwoControlsOnSameLevelOfOtherType_ReturnsControls() {
			var rootControl = new Control();
			var firstControl = new TestableControlOfOtherType("firstControl");
			var secondControl = new TestableControlOfOtherType("secondControl");

			rootControl.Controls.Add(firstControl);
			rootControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindControlsOnType(rootControl, typeof(TestableControlOfOtherType));

			Assert.Contains(firstControl, result);
			Assert.Contains(secondControl, result);
		}

		[Test]
		public void FindControlsOnType_AddTwoControlsInDiefferentLevelsOfOtherType_ReturnsControls() {
			var rootControl = new Control();
			var firstControl = new TestableControlOfOtherType("firstControl");
			var secondControl = new TestableControlOfOtherType("secondControl");

			rootControl.Controls.Add(firstControl);
			firstControl.Controls.Add(secondControl);

			var result = ControlFinder.Instance.FindControlsOnType(rootControl, typeof(TestableControlOfOtherType));

			Assert.Contains(firstControl, result);
			Assert.Contains(secondControl, result);
		}

		public class TestableControl : Control {
			private readonly string uniqueId;
			private bool findControlIsCalled;

			public TestableControl() {}

			public TestableControl(string uniqueId) {
				this.uniqueId = uniqueId;
			}

			public bool FindControlIsCalled { get { return findControlIsCalled; } }

			public override string UniqueID {
				get {
					return uniqueId;
				}
			}

			public override Control FindControl(string id) {
				findControlIsCalled = true;
				return null;
			}
			
		}

		public class TestableControlInheritingInterface : TestableControl, IAnyInterface {
			public TestableControlInheritingInterface(string uniqueId) : base(uniqueId) {}
		}


		public class TestableControlOfOtherType : TestableControl {
			public TestableControlOfOtherType(string uniqueId) : base(uniqueId) {}
		}

		public interface IAnyInterface {}


	}
}
