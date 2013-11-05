namespace Peregrine.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_Number_to_Ordinal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Number", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "Number", c => c.Int(nullable: false));
            AddColumn("dbo.Rounds", "Number", c => c.Int(nullable: false));
            DropColumn("dbo.Games", "Ordinal");
            DropColumn("dbo.Rounds", "Ordinal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rounds", "Ordinal", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Ordinal", c => c.Int(nullable: false));
            DropColumn("dbo.Rounds", "Number");
            DropColumn("dbo.Matches", "Number");
            DropColumn("dbo.Games", "Number");
        }
    }
}
