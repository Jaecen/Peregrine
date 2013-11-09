using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Peregrine.Data;

namespace Peregrine.Service
{
	public class RoundManager
	{
		public Round GenerateFirstRound(Tournament tournament)
		{
			// Generate initial order by hashing tournament key + name
			return new Round
			{
				Number = 1,
				Matches = tournament
					.Players
					.OrderBy(p => String.Format("{0}{1}{2}", tournament.Key, p.Name, tournament.Players.Count).ComputeMd5())
					.PartitionBy(2)
					.Select((partition, index) => new Match
					{
						Number = index + 1,
						Players = partition.ToArray(),
					})
					.ToArray()
			};
		}
	}
}