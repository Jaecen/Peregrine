using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Unstated;

namespace Unstated.Test
{
	public class StateMachineTests
	{
		[Fact]
		public void When_Trigger_Exists()
		{
			var stateMachine = new StateMachineBuilder().CreateStateMachine<char, int, StateMachineContext<char>>(
					sm => sm.DefineState('a')
						.WithTrigger(1, 'b')
				);

			var originalContext = new StateMachineContext<char>('a');

			var newContext = stateMachine.Fire(originalContext, 1);

			Assert.Equal(newContext.GetState(), 'b');
		}

		[Fact]
		public void When_Trigger_Does_Not_Exist()
		{
			var stateMachine = new StateMachineBuilder().CreateStateMachine<char, int, StateMachineContext<char>>(
					sm => sm.DefineState('a')
						.WithTrigger(1, 'b')
				);

			var originalContext = new StateMachineContext<char>('a');

			Assert.Throws<InvalidTriggerException<char, int>>(() => stateMachine.Fire(originalContext, 2));
		}

		[Fact]
		public void When_Predicate_Triggering_Without_Value()
		{
			var stateMachine = new StateMachineBuilder().CreateStateMachine<char, int, StateMachineContext<char>>(
					sm => sm.DefineState('a')
						.WithTrigger(1, (string x) => x == "yes", 'x')
						.WithTrigger(1, (string x) => x == "no", 'y')
				);

			var originalContext = new StateMachineContext<char>('a');

			Assert.Throws<InvalidTriggerException<char, int>>(() => stateMachine.Fire(originalContext, 1));
		}

		[Fact]
		public void When_Predicate_Triggering_With_Matching_Value()
		{
			var stateMachine = new StateMachineBuilder().CreateStateMachine<char, int, StateMachineContext<char>>(
					sm => sm.DefineState('a')
						.WithTrigger(1, (string x) => x == "yes", 'x')
						.WithTrigger(1, (string x) => x == "no", 'y')
				);

			var originalContext = new StateMachineContext<char>('a');

			Assert.Equal('x', stateMachine.Fire(originalContext, 1, "yes").GetState());
			Assert.Equal('y', stateMachine.Fire(originalContext, 1, "no").GetState());
		}

		[Fact]
		public void When_Predicate_Triggering_With_Non_Matching_Value()
		{
			var stateMachine = new StateMachineBuilder().CreateStateMachine<char, int, StateMachineContext<char>>(
					sm => sm.DefineState('a')
						.WithTrigger(1, (string x) => x == "yes", 'x')
						.WithTrigger(1, (string x) => x == "no", 'y')
				);

			var originalContext = new StateMachineContext<char>('a');

			Assert.Throws<InvalidTriggerException<char, int>>(() => stateMachine.Fire(originalContext, 1, "maybe"));
		}
	}
}
