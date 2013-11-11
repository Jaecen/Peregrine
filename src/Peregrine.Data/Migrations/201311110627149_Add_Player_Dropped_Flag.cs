namespace Peregrine.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class Add_Player_Dropped_Flag : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Players", "Dropped", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Players", "Dropped");
		}
	}
}
