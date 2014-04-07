namespace Peregrine.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Active_Round_Number : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "ActiveRoundNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "ActiveRoundNumber");
        }
    }
}
