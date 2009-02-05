﻿using System;
using Xunit;
using Moq;

namespace Moq.Tests
{
	public class VerifyFixture
	{
		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalledWithMessage()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable("Kaboom!");

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Contains("Kaboom!", mex.Message);
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(expectedArg.Substring(0, 5)))
				.Returns("ack")
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".Execute(""lorem"")"), "Contains evaluated expected argument.");
			}
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Returns("ack")
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".Execute(Is<String>(s => IsNullOrEmpty(s)))"), "Contains evaluated expected argument.");
			}
		}

		[Fact]
		public void VerifiesNoOpIfNoVerifiableExpectations()
		{
			var mock = new Mock<IFoo>();

			mock.Verify();
		}

		[Fact]
		public void ThrowsIfVerifyAllNotMet()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit());

			try
			{
				mock.VerifyAll();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Submit());
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesVoidMethodWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			mock.Verify(f => f.Submit());
		}

		[Fact]
		public void ThrowsIfVerifyReturningMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"));
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesReturningMethodWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("ping");

			mock.Verify(f => f.Execute("ping"));
		}

		[Fact]
		public void VerifiesPropertyGetWithExpression()
		{
			var mock = new Mock<IFoo>();

			var v = mock.Object.Value;

			mock.VerifyGet(f => f.Value);
		}

		[Fact]
		public void VerifiesReturningMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Execute should have been invoked with 'ping'"));
				Assert.True(me.Message.Contains("f.Execute(\"ping\")"));
			}
		}

		[Fact]
		public void VerifiesVoidMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Submit(), "Submit should be invoked");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Submit should be invoked"));
				Assert.True(me.Message.Contains("f.Submit()"));
			}
		}

		[Fact]
		public void VerifiesPropertyGetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifyGet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void VerifiesPropertySetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void VerifiesPropertySetValueWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, 5, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesReturningMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Execute should have been invoked with 'ping'"));
				Assert.True(me.Message.Contains("f.Execute(\"ping\")"));
			}
		}

		[Fact]
		public void AsInferfaceVerifiesVoidMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.Verify(f => f.Submit(), "Submit should be invoked");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Submit should be invoked"));
				Assert.True(me.Message.Contains("f.Submit()"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesPropertyGetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.VerifyGet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetValueWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, 5, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void ThrowsIfVerifyPropertyGetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifyGet(f => f.Value);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesPropertySetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Value = 5;

			mock.VerifySet(f => f.Value);
		}

		[Fact]
		public void ThrowsIfVerifyPropertySetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesSetterWithExpression()
		{
			var mock = new Mock<IFoo>();
			mock.SetupSet(m => m.Value, 5);

			mock.Object.Value = 1;
			mock.Object.Value = 5;

			mock.VerifySet(m => m.Value);
			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value, 2));
			mock.VerifySet(m => m.Value, 5);
		}

		[Fact]
		public void VerifiesRefWithExpression()
		{
			var mock = new Mock<IFoo>();
			var expected = "ping";

			Assert.Throws<MockException>(() => mock.Verify(m => m.EchoRef(ref expected)));

			mock.Object.EchoRef(ref expected);

			mock.Verify(m => m.EchoRef(ref expected));
		}

		[Fact]
		public void VerifiesOutWithExpression()
		{
			var mock = new Mock<IFoo>();
			var expected = "ping";

			Assert.Throws<MockException>(() => mock.Verify(m => m.EchoOut(out expected)));

			mock.Object.EchoOut(out expected);

			mock.Verify(m => m.EchoOut(out expected));
		}

		[Fact]
		public void VerifiesTimes()
		{
			var mock = new Mock<IFoo>();

			//Assert.Throws<MockException>(mock.Verify(f => f.Execute("ping"), 1));

			//mock.Object.Execute("ping");

			//mock.Verify(f => f.Execute("ping"), 1);
		}



		public interface IFoo
		{
			int? Value { get; set; }
			void EchoRef<T>(ref T value);
			void EchoOut<T>(out T value);
			int Echo(int value);
			void Submit();
			string Execute(string command);
		}
	}
}

namespace SomeNamespace
{
	public class VerifyExceptionsFixture
	{
		[Fact]
		public void RendersReadableMessageForVerifyFailures()
		{
			var mock = new Mock<Moq.Tests.VerifyFixture.IFoo>();

			mock.Setup(x => x.Submit());
			mock.Setup(x => x.Echo(1));
			mock.Setup(x => x.Execute("ping"));

			try
			{
				mock.VerifyAll();
				Assert.True(false, "Should have thrown");
			}
			catch (Exception ex)
			{
				Assert.True(ex.Message.Contains("x => x.Submit()"));
				Assert.True(ex.Message.Contains("x => x.Echo(1)"));
				Assert.True(ex.Message.Contains("x => x.Execute(\"ping\")"));
			}
		}
	}
}