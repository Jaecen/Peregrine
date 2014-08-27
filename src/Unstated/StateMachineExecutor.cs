using System;
using System.Collections.Generic;
using System.Linq;

namespace Unstated
{
	public class StateMachineExecutor<TState, TTrigger, TContext> : IStateMachineExecutor<TState, TTrigger, TContext>
		where TContext: IStateMachineContext<TContext, TState>
	{
		public bool CanFire(IStateMachineDefinition<TState, TTrigger> definition, TContext context, TTrigger trigger)
		{
			return GetTargetStates(
					definition,
					context.GetState(),
					trigger,
					tds => tds
				)
				.Any();
		}

		public TContext Fire(IStateMachineDefinition<TState, TTrigger> definition, TContext context, TTrigger trigger)
		{
			var currentState = context.GetState();
			var targetStates = GetTargetStates(
					definition, 
					currentState, 
					trigger, 
					tds => tds
						.OfType<TriggerDefinition<TState, TTrigger>>()
				);

			if(!targetStates.Any())
				throw new InvalidTriggerException<TState, TTrigger>(currentState, trigger);

			return context.UpdateState(targetStates.First());
		}

		public TContext Fire<TPredicateArg0>(IStateMachineDefinition<TState, TTrigger> definition, TContext context, TTrigger trigger, TPredicateArg0 predicateArg0)
		{
			var currentState = context.GetState();
			var targetStates = GetTargetStates(
					definition,
					currentState,
					trigger,
					tds => tds
						.OfType<TriggerDefinition<TState, TTrigger, TPredicateArg0>>()
						.Where(td => td.Predicate(predicateArg0))
				);

			if(!targetStates.Any())
				throw new InvalidTriggerException<TState, TTrigger>(currentState, trigger);

			return context.UpdateState(targetStates.First());
		}

		IEnumerable<TState> GetTargetStates(IStateMachineDefinition<TState, TTrigger> definition, TState currentState, TTrigger trigger, Func<IEnumerable<TriggerDefinitionBase<TState, TTrigger>>, IEnumerable<TriggerDefinitionBase<TState, TTrigger>>> predicateFilter)
		{
			var prefiltered = definition
				.GetStates()
				.Where(sd => definition.Equals(sd.State, currentState))
				.SelectMany(sd => sd.Triggers)
				.Where(td => definition.Equals(td.Trigger, trigger));

			return predicateFilter(prefiltered)
				.Select(td => td.TargetState);
		}
	}
}
