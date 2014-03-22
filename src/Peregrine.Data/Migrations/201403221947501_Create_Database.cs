namespace Peregrine.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Database : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Winner_Id = c.Int(),
                        Match_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Winner_Id)
                .ForeignKey("dbo.Matches", t => t.Match_Id)
                .Index(t => t.Winner_Id)
                .Index(t => t.Match_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Dropped = c.Boolean(nullable: false),
                        Match_Id = c.Int(),
                        Tournament_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matches", t => t.Match_Id)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id)
                .Index(t => t.Match_Id)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Round_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rounds", t => t.Round_Id)
                .Index(t => t.Round_Id);
            
            CreateTable(
                "dbo.Rounds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Tournament_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.Guid(nullable: false),
                        Name = c.String(),
                        Seed = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rounds", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.Players", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.Matches", "Round_Id", "dbo.Rounds");
            DropForeignKey("dbo.Players", "Match_Id", "dbo.Matches");
            DropForeignKey("dbo.Games", "Match_Id", "dbo.Matches");
            DropForeignKey("dbo.Games", "Winner_Id", "dbo.Players");
            DropIndex("dbo.Rounds", new[] { "Tournament_Id" });
            DropIndex("dbo.Matches", new[] { "Round_Id" });
            DropIndex("dbo.Players", new[] { "Tournament_Id" });
            DropIndex("dbo.Players", new[] { "Match_Id" });
            DropIndex("dbo.Games", new[] { "Match_Id" });
            DropIndex("dbo.Games", new[] { "Winner_Id" });
            DropTable("dbo.Tournaments");
            DropTable("dbo.Rounds");
            DropTable("dbo.Matches");
            DropTable("dbo.Players");
            DropTable("dbo.Games");
        }
    }
}
